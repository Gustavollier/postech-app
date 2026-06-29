# =============================================================================
# terraform.tfvars — Variáveis locais para desenvolvimento
#
# Este arquivo NÃO deve ser commitado (está no .gitignore).
# Para CI/CD: use GitHub Secrets → TF_VAR_mssql_sa_password, etc.
#
# Mantenha as senhas sincronizadas com o arquivo .env na raiz do projeto.
# =============================================================================

cluster_name      = "postechallenge"
kustomize_overlay = "local"

mssql_sa_password = "TroqueEstaSenha@12345"
app_db_password   = "TroqueEstaSenhaDoUsuarioApp@12345"
jwt_secret_key    = "troque-esta-chave-por-uma-chave-aleatoria-com-mais-de-32-bytes"
