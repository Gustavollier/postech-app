# =============================================================================
# main.tf — Orquestra os módulos de infraestrutura
#
# Ordem de execução (controlada por depends_on):
#   1. kind-cluster  → cria o cluster Kubernetes local
#   2. k8s-base      → aplica os manifests base via kubectl apply -k
# =============================================================================

locals {
  # Caminho absoluto para a raiz do projeto (dois níveis acima de infra/terraform/)
  project_root   = "${path.root}/../.."
  overlay_path   = "${local.project_root}/k8s/overlays/${var.kustomize_overlay}"
  kubectl_context = "kind-${var.cluster_name}"
}

# ─── Módulo 1: Cluster kind ───────────────────────────────────────────────────
module "kind_cluster" {
  source = "./modules/kind-cluster"

  cluster_name = var.cluster_name
}

# ─── Módulo 2: Recursos K8s base ─────────────────────────────────────────────
module "k8s_base" {
  source = "./modules/k8s-base"

  cluster_name      = var.cluster_name
  kubectl_context   = local.kubectl_context
  overlay_path      = local.overlay_path
  mssql_sa_password = var.mssql_sa_password
  app_db_password   = var.app_db_password
  jwt_secret_key    = var.jwt_secret_key
  sql_init_file     = "${local.project_root}/infra/sql/init.sql"
  init_job_manifest = "${local.project_root}/k8s/sqlserver/init-job.yaml"

  # GHCR imagePullSecret — vazio no local, preenchido pelo CI via TF_VAR_*
  ghcr_username = var.ghcr_username
  ghcr_token    = var.ghcr_token

  # Garante que o cluster existe antes de tentar aplicar manifests
  depends_on = [module.kind_cluster]
}
