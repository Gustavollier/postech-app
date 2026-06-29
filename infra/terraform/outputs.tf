output "cluster_name" {
  description = "Nome do cluster kind criado"
  value       = module.kind_cluster.cluster_name
}

output "kubectl_context" {
  description = "Contexto kubectl para usar com --context"
  value       = "kind-${module.kind_cluster.cluster_name}"
}

output "port_forward_command" {
  description = "Comando para acessar a API localmente após o apply"
  value       = "kubectl port-forward svc/postechallenge-api 8080:80 -n postechallenge --context kind-${module.kind_cluster.cluster_name}"
}
