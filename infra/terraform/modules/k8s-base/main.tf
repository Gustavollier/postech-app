# =============================================================================
# modules/k8s-base/main.tf
#
# Aplica os recursos K8s base via kubectl + kustomize.
# Usa null_resource + local-exec para evitar o problema de inicialização do
# provider kubernetes (que requer cluster existente no momento do plan).
#
# O mesmo código funciona para local e CI — só muda o overlay_path e os secrets.
# =============================================================================

locals {
  connection_string = "Server=sqlserver,1433;Database=PosTechChallenge;User Id=appchat;Password=${var.app_db_password};TrustServerCertificate=True;"
  namespace         = "postechallenge"
}

# ─── 1. Criar Secrets K8s (antes de aplicar o overlay) ───────────────────────
# Os secrets são criados via kubectl e não ficam no tfstate em texto plano,
# pois são passados como argumentos de linha de comando (não em arquivos).
resource "null_resource" "k8s_secrets" {
  provisioner "local-exec" {
    # interpreter obrigatório no Windows: cmd.exe não suporta \ como continuação de linha
    # Requer bash no PATH (Git Bash ou WSL no Windows; bash nativo no Linux/macOS/CI)
    interpreter = ["bash", "-c"]
    # Usa environment para isolar o token do log de processo (evita exposição via ps aux)
    environment = {
      GHCR_TOKEN    = var.ghcr_token
      GHCR_USERNAME = var.ghcr_username
    }
    command = <<-EOT
      echo "Criando namespace ${local.namespace}..."
      kubectl create namespace ${local.namespace} \
        --context ${var.kubectl_context} \
        --dry-run=client -o yaml | kubectl apply -f - --context ${var.kubectl_context}

      echo "Criando/atualizando Secrets da aplicação..."
      kubectl create secret generic postechallenge-secrets \
        --namespace=${local.namespace} \
        --context=${var.kubectl_context} \
        --from-literal=MSSQL_SA_PASSWORD=${var.mssql_sa_password} \
        --from-literal=APP_DB_PASSWORD=${var.app_db_password} \
        --from-literal=JWT_SECRET_KEY=${var.jwt_secret_key} \
        --from-literal="ConnectionStrings__DefaultConnection=${local.connection_string}" \
        --dry-run=client -o yaml | kubectl apply -f - --context ${var.kubectl_context}

      # Cria imagePullSecret para GHCR apenas se token for fornecido (CI/CD)
      if [ -n "$GHCR_TOKEN" ]; then
        echo "Criando imagePullSecret ghcr-secret para GHCR..."
        kubectl create secret docker-registry ghcr-secret \
          --namespace=${local.namespace} \
          --context=${var.kubectl_context} \
          --docker-server=ghcr.io \
          --docker-username="$GHCR_USERNAME" \
          --docker-password="$GHCR_TOKEN" \
          --dry-run=client -o yaml | kubectl apply -f - --context ${var.kubectl_context}
        echo "ghcr-secret criado."
      else
        echo "GHCR token não fornecido — ghcr-secret não criado (modo local)."
      fi

      echo "Secrets aplicados."
    EOT
  }

  triggers = {
    # Recria o secret se qualquer senha mudar
    # Nota: o hash é calculado pelo Terraform e não expõe as senhas no state
    secrets_hash = sha256("${var.mssql_sa_password}${var.app_db_password}${var.jwt_secret_key}")
    ghcr_hash    = sha256(var.ghcr_token)
  }
}

# ─── 2. Criar ConfigMap com script SQL (se o arquivo existir) ────────────────
resource "null_resource" "sql_init_configmap" {
  count = fileexists(var.sql_init_file) ? 1 : 0

  depends_on = [null_resource.k8s_secrets]

  provisioner "local-exec" {
    interpreter = ["bash", "-c"]
    command = <<-EOT
      echo "Criando ConfigMap sql-init-script..."
      kubectl create configmap sql-init-script \
        --namespace=${local.namespace} \
        --context=${var.kubectl_context} \
        --from-file=init.sql=${var.sql_init_file} \
        --dry-run=client -o yaml | kubectl apply -f - --context ${var.kubectl_context}
      echo "ConfigMap sql-init-script criado."
    EOT
  }

  triggers = {
    sql_file_hash = fileexists(var.sql_init_file) ? filesha256(var.sql_init_file) : "no-file"
  }
}

# ─── 3. Aplicar overlay Kustomize (infra base: configmap, SQL Server, API) ───
resource "null_resource" "apply_kustomize_overlay" {
  depends_on = [
    null_resource.k8s_secrets,
    null_resource.sql_init_configmap,
  ]

  provisioner "local-exec" {
    interpreter = ["bash", "-c"]
    command = <<-EOT
      echo "Aplicando overlay Kustomize: ${var.overlay_path}"
      kubectl apply -k ${var.overlay_path} --context ${var.kubectl_context}
      echo "Overlay aplicado."
    EOT
  }

  triggers = {
    overlay_path = var.overlay_path
    cluster      = var.cluster_name
    # Re-aplica se qualquer arquivo YAML do overlay mudar
    overlay_hash = sha256(join("", [
      for f in fileset(var.overlay_path, "**/*.yaml") :
      filesha256("${var.overlay_path}/${f}")
    ]))
  }
}

# ─── 4. Aguardar SQL Server ficar pronto ─────────────────────────────────────
resource "null_resource" "wait_sqlserver" {
  depends_on = [null_resource.apply_kustomize_overlay]

  provisioner "local-exec" {
    interpreter = ["bash", "-c"]
    command = <<-EOT
      echo "Aguardando SQL Server ficar pronto (timeout: 3 min)..."
      kubectl rollout status statefulset/sqlserver \
        --namespace=${local.namespace} \
        --context=${var.kubectl_context} \
        --timeout=180s
      echo "SQL Server pronto."
    EOT
  }

  triggers = {
    apply_id = null_resource.apply_kustomize_overlay.id
  }
}

# ─── 5. Rodar Job de init do banco (apenas uma vez) ──────────────────────────
resource "null_resource" "sql_init_job" {
  count = fileexists(var.sql_init_file) ? 1 : 0

  depends_on = [null_resource.wait_sqlserver]

  provisioner "local-exec" {
    interpreter = ["bash", "-c"]
    command = <<-EOT
      echo "Verificando se Job de init já foi concluído..."
      JOB_STATUS=$(kubectl get job sqlserver-init \
        --namespace=${local.namespace} \
        --context=${var.kubectl_context} \
        -o jsonpath='{.status.succeeded}' 2>/dev/null || echo "0")

      if [ "$JOB_STATUS" = "1" ]; then
        echo "Job de init já foi concluído — pulando."
      else
        echo "Executando Job de inicialização do banco..."
        kubectl delete job sqlserver-init \
          --namespace=${local.namespace} \
          --context=${var.kubectl_context} \
          --ignore-not-found=true

        kubectl apply -f ${var.init_job_manifest} --context ${var.kubectl_context}

        kubectl wait job/sqlserver-init \
          --namespace=${local.namespace} \
          --context=${var.kubectl_context} \
          --for=condition=complete \
          --timeout=120s

        echo "Banco inicializado com sucesso."
      fi
    EOT
  }

  triggers = {
    sql_file_hash = fileexists(var.sql_init_file) ? filesha256(var.sql_init_file) : "no-file"
  }
}

# ─── 6. Aguardar API ficar pronta ────────────────────────────────────────────
resource "null_resource" "wait_api" {
  depends_on = [
    null_resource.sql_init_job,
    null_resource.apply_kustomize_overlay,
  ]

  provisioner "local-exec" {
    interpreter = ["bash", "-c"]
    command = <<-EOT
      echo "Aguardando API ficar pronta..."
      kubectl rollout status deployment/postechallenge-api \
        --namespace=${local.namespace} \
        --context=${var.kubectl_context} \
        --timeout=120s
      echo ""
      echo "=================================================="
      echo " Ambiente pronto!"
      echo " Para acessar a API:"
      echo " kubectl port-forward svc/postechallenge-api 8080:80 -n ${local.namespace} --context ${var.kubectl_context}"
      echo "=================================================="
    EOT
  }

  triggers = {
    apply_id = null_resource.apply_kustomize_overlay.id
  }
}
