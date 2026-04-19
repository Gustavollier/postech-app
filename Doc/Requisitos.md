# POS TECH - Tech Challenge

## 📌 Sobre o Tech Challenge
O **Tech Challenge** é o projeto da fase que engloba os conhecimentos adquiridos em todas as disciplinas.

- Deve ser desenvolvido em grupo (preferencialmente);
- É uma atividade obrigatória;
- Vale **90% da nota da fase**;
- Atenção ao prazo de entrega.

---

## 🚧 Desafio

Uma oficina mecânica de médio porte, especializada em manutenção de veículos, enfrenta dificuldades para expandir seus serviços com qualidade e eficiência.

Atualmente, o processo é desorganizado, utilizando anotações manuais e planilhas, gerando problemas como:

- ❌ Erros na priorização dos atendimentos  
- ❌ Falhas no controle de peças e insumos  
- ❌ Dificuldade em acompanhar o status dos serviços  
- ❌ Perda de histórico de clientes e veículos  
- ❌ Ineficiência no fluxo de orçamentos e autorizações  

### 🎯 Objetivo
Desenvolver um **Sistema Integrado de Atendimento e Execução de Serviços**, permitindo:

- Acompanhamento em tempo real pelo cliente;
- Aprovação de serviços via aplicativo;
- Gestão interna eficiente e segura.

---

## 💡 Proposta

Desenvolver o **MVP do back-end** com foco em:

- Gestão de ordens de serviço (OS)
- Gestão de clientes
- Gestão de peças

### 🧠 Diretrizes técnicas
- Aplicar **Domain-Driven Design (DDD)**
- Garantir boas práticas de:
  - Qualidade de Software
  - Segurança

---

## ⚙️ Funcionalidades Obrigatórias

### 🔄 Fluxos principais

#### 📋 Criação da Ordem de Serviço (OS)

- Identificação do cliente (CPF/CNPJ)
- Cadastro de veículo:
  - Placa
  - Marca
  - Modelo
  - Ano
- Inclusão de serviços (ex: troca de óleo, alinhamento)
- Inclusão de peças e insumos
- Geração automática de orçamento
- Envio para aprovação do cliente

---

#### 🔍 Acompanhamento da OS

##### Status possíveis:

- Recebida
- Em diagnóstico
- Aguardando aprovação
- Em execução
- Finalizada
- Entregue

##### Regras:
- Atualização automática dos status
- Consulta via API para o cliente

---

### 🧾 Gestão administrativa

- CRUD de clientes
- CRUD de veículos
- CRUD de serviços
- CRUD de peças e insumos (com controle de estoque)
- Listagem e detalhamento de OS
- Monitoramento de tempo médio de execução

---

### 🔐 Segurança e Qualidade

- Autenticação via **JWT**
- Validação de dados sensíveis:
  - CPF/CNPJ
  - Placa
- Testes:
  - Unitários
  - Integração

---

## 🧱 Requisitos Técnicos

- Arquitetura **Monolítica**
- Estrutura em **camadas**
- Banco de dados: livre (necessário justificar escolha)
- APIs RESTful documentadas (Swagger ou similar)
- Docker:
  - `Dockerfile`
  - `docker-compose.yml`
- Testes automatizados com **mínimo de 80% de cobertura**
- Execução local simples via `README.md`
- Repositório privado com acesso ao usuário: `soat-architecture`

---

## 📦 Entregáveis - Fase 1

### 🎥 Vídeo
- Até **15 minutos**
- Demonstração completa do sistema

---

### 📊 Documentação DDD

- Event Storming completo:
  - Criação da OS
  - Acompanhamento da OS
  - Gestão de peças e insumos
- Diagramas conforme disciplina
- Linguagem Ubíqua aplicada

---

### 💻 Código-fonte

- APIs implementadas
- Docker configurado
- README completo:
  - Instruções de uso
  - Objetivos do projeto

---

### 🔍 Relatório de Vulnerabilidades

- Análise de segurança do código
- Inclusão de resultados de scans

---

### 📄 Documento Final (PDF)

Deve conter:

- Nome do grupo
- Participantes + usernames no Discord
- Link da documentação
- Link do repositório
- Relatório de vulnerabilidades

---

## 💬 Suporte

Dúvidas podem ser tiradas no **Discord durante todo o Tech Challenge**.

---

## 🚀 Bora construir!