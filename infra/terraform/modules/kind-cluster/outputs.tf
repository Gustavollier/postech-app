output "cluster_name" {
  description = "Nome do cluster kind criado"
  value       = kind_cluster.main.name
}

output "cluster_id" {
  description = "ID do cluster kind (usado para trigger de depends_on)"
  value       = kind_cluster.main.id
}

output "kubeconfig" {
  description = "Kubeconfig do cluster (sensível)"
  value       = kind_cluster.main.kubeconfig
  sensitive   = true
}
