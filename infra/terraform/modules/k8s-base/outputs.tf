output "namespace" {
  description = "Namespace Kubernetes criado"
  value       = "postechallenge"
}

output "apply_id" {
  description = "ID do último apply (usado para triggers em recursos dependentes)"
  value       = null_resource.apply_kustomize_overlay.id
}
