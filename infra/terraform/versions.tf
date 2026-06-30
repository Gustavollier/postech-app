terraform {
  # Backend local por padrão.
  # Para usar o Terraform Cloud (obrigatório em CI), descomente e preencha:
  # cloud {
  #   organization = "<sua-org-no-terraform-cloud>"
  #   workspaces {
  #     name = "postechallenge-infra"
  #   }
  # }

  required_version = ">= 1.6"

  required_providers {
    kind = {
      source  = "tehcyx/kind"
      version = "~> 0.6"
    }
    null = {
      source  = "hashicorp/null"
      version = "~> 3.2"
    }
  }
}

# Provider kind — cria clusters Kubernetes locais usando Docker
provider "kind" {}
