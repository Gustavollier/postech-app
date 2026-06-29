variable "cluster_name" {
  description = "Nome do cluster kind"
  type        = string
}

variable "kubectl_context" {
  description = "Contexto kubectl (ex: kind-postechallenge)"
  type        = string
}

variable "overlay_path" {
  description = "Caminho absoluto para o overlay Kustomize a aplicar (ex: /path/to/k8s/overlays/local)"
  type        = string
}

variable "mssql_sa_password" {
  description = "Senha do SA do SQL Server"
  type        = string
  sensitive   = true
}

variable "app_db_password" {
  description = "Senha do usuário appchat"
  type        = string
  sensitive   = true
}

variable "jwt_secret_key" {
  description = "Chave secreta JWT (mín 32 bytes)"
  type        = string
  sensitive   = true
}

variable "sql_init_file" {
  description = "Caminho absoluto para o script SQL de inicialização (infra/sql/init.sql)"
  type        = string
}

variable "init_job_manifest" {
  description = "Caminho absoluto para o manifest do Job de init SQL"
  type        = string
}

# ─── GHCR imagePullSecret (opcional — usado apenas no CI) ────────────────────
# Local: deixe em branco (imagem carregada diretamente no kind, sem pull secret)
# CI:    passe via TF_VAR_ghcr_token=${{ secrets.GITHUB_TOKEN }}

variable "ghcr_username" {
  description = "Username do GHCR (ex: github.actor). Obrigatório se ghcr_token for definido."
  type        = string
  default     = ""
  sensitive   = false
}

variable "ghcr_token" {
  description = "Token de acesso ao GHCR (ex: GITHUB_TOKEN). Se definido, cria o imagePullSecret 'ghcr-secret'."
  type        = string
  default     = ""
  sensitive   = true
}
