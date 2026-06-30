# =============================================================================
# modules/kind-cluster/main.tf
# Provisiona um cluster Kubernetes local usando kind (Kubernetes in Docker)
# =============================================================================

resource "kind_cluster" "main" {
  name           = var.cluster_name
  wait_for_ready = true

  kind_config {
    kind        = "Cluster"
    api_version = "kind.x-k8s.io/v1alpha4"

    # Control-plane
    node {
      role = "control-plane"
    }

    # Worker node
    node {
      role = "worker"
    }
  }
}

# Instala o metrics-server após o cluster estar pronto.
# O patch --kubelet-insecure-tls é necessário porque kind usa certs self-signed.
resource "null_resource" "install_metrics_server" {
  depends_on = [kind_cluster.main]

  provisioner "local-exec" {
    interpreter = ["bash", "-c"]
    command = <<-EOT
      echo "Instalando metrics-server..."
      kubectl apply \
        -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml \
        --context kind-${var.cluster_name}

      echo "Aplicando patch --kubelet-insecure-tls (necessário para kind)..."
      kubectl patch deployment metrics-server \
        -n kube-system \
        --context kind-${var.cluster_name} \
        --type=json \
        -p '[{"op":"add","path":"/spec/template/spec/containers/0/args/-","value":"--kubelet-insecure-tls"}]'

      echo "metrics-server configurado."
    EOT
  }

  triggers = {
    cluster_id = kind_cluster.main.id
  }
}
