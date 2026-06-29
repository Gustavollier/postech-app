variable "cluster_name" {
  description = "Nome do cluster kind. Também define o contexto kubectl: kind-<cluster_name>"
  type        = string
  default     = "postechallenge"
}

variable "kustomize_overlay" {
  description = "Overlay do Kustomize a aplicar. Use 'local' para desenvolvimento, 'ci' para pipeline."
  type        = string
  default     = "local"

  validation {
    condition     = contains(["local", "ci"], var.kustomize_overlay)
    error_message = "kustomize_overlay deve ser 'local' ou 'ci'."
  }
}

# ─── Secrets — sem valores default para forçar explicitação ──────────────────
# Local: definir em terraform.tfvars (não commitar)
# CI:    definir como GitHub Secrets → TF_VAR_mssql_sa_password, etc.

variable "mssql_sa_password" {
  description = "Senha do SA (System Administrator) do SQL Server. Mínimo: 8 chars, maiúsc, minúsc, número e símbolo."
  type        = string
  sensitive   = true
}

variable "app_db_password" {
  description = "Senha do usuário 'appchat' da aplicação no banco de dados."
  type        = string
  sensitive   = true
}

variable "jwt_secret_key" {
  description = "Chave secreta para assinatura de tokens JWT. Mínimo: 32 bytes (32 caracteres ASCII)."
  type        = string
  sensitive   = true

  validation {
    condition     = length(var.jwt_secret_key) >= 32
    error_message = "jwt_secret_key deve ter pelo menos 32 caracteres."
  }
}

# ─── GHCR (opcional — usado apenas no CI via TF_VAR_*) ───────────────────────
# Local: não precisa definir (imagem carregada no kind com `kind load docker-image`)
# CI:    o workflow define via TF_VAR_ghcr_username e TF_VAR_ghcr_token

variable "ghcr_username" {
  description = "Username do GitHub Container Registry. Deixe em branco para ambiente local."
  type        = string
  default     = ""
  sensitive   = false
}

variable "ghcr_token" {
  description = "Token de acesso ao GHCR (GITHUB_TOKEN do Actions). Deixe em branco para ambiente local."
  type        = string
  default     = ""
  sensitive   = true
}
