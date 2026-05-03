# PosTechChallenge — Sistema de Gestão de Oficina Mecânica

MVP de back-end desenvolvido para o **Tech Challenge da Fase 1 — FIAP PosTech**, com foco em gestão de Ordens de Serviço, clientes e peças, aplicando Domain-Driven Design (DDD).

---

## 🎯 Objetivo

Digitalizar o processo de atendimento de uma oficina mecânica de médio porte, substituindo anotações manuais e planilhas por um sistema integrado que permite:

- Gestão completa de Ordens de Serviço (OS) com máquina de estados
- Acompanhamento de status em tempo real via API
- Controle de estoque de peças e insumos
- Autenticação segura com JWT para operações administrativas

---

## 🏗️ Arquitetura

O sistema adota arquitetura **monolítica em camadas** com princípios de **Domain-Driven Design (DDD)**:

```
PosTechChallenge/              ← API Web (Controllers, Middlewares, DTOs)
PosTechChallenge.Applicacao/   ← Camada de Aplicação (Use Cases, Services, Interfaces)
PosTechChallenge.Dominio/      ← Domínio (Entidades, Value Objects, Domain Services)
PosTechChallenge.Infraestrutura/ ← Infraestrutura (Repositórios, Queries SQL, Dapper)
PosTechChallenge.Testes/       ← Testes unitários (xUnit + Moq)
```

### Escolha do Banco de Dados

**SQL Server 2022** foi escolhido pelos seguintes motivos:
- Familiaridade da equipe com o ecossistema Microsoft/.NET
- Suporte nativo a transações ACID para integridade das Ordens de Serviço
- Integração natural com Dapper (micro-ORM utilizado no projeto)
- Disponibilidade de imagem Docker oficial (`mcr.microsoft.com/mssql/server`)

---

## 🧩 Entidades Principais

| Entidade | Descrição |
|---|---|
| Cliente | Pessoa física (CPF) ou jurídica (CNPJ) |
| Funcionário | Colaborador da oficina com cargo e valor/hora |
| Veículo | Veículo do cliente identificado por placa |
| Ordem de Serviço | Ciclo completo de atendimento |
| Peças | Itens de estoque com controle de quantidade |
| Item OS | Mão de obra ou peça vinculada à OS |

### Status da Ordem de Serviço

```
Recebida → Em Diagnóstico → Aguardando Aprovação → Em Execução → Finalizada → Entregue
```

---

## 🚀 Como executar localmente

### Pré-requisitos

- [Docker](https://www.docker.com/get-started) instalado e em execução
- [Docker Compose](https://docs.docker.com/compose/install/) v2+

### 1. Clone o repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd PosTechChallenge
```

### 2. Suba o ambiente completo

Crie um arquivo `.env` a partir do exemplo e troque os valores antes de subir os containers:

```bash
cp .env.example .env
```

```bash
docker compose up -d --build
```

Este comando irá:
1. Baixar e iniciar o **SQL Server 2022**
2. Aguardar o banco ficar saudável e executar o script `infra/sql/init.sql` (criação de tabelas e dados de seed)
3. Fazer o build e iniciar a **API** na porta `8080`

> ⚠️ Na primeira execução, aguarde cerca de 60–90 segundos até o SQL Server inicializar completamente.

### 3. Acesse o Swagger

```
http://localhost:8080/swagger
```

### 4. Execute os testes

```bash
dotnet test PosTechChallenge.Testes/PosTechChallenge.Testes.csproj
```

---

## 🔐 Autenticação

A API utiliza **JWT Bearer Token**. Antes de acessar endpoints protegidos:

Atualizacao de seguranca: o login continua publico, mas `POST /api/v1/autenticacao/criar-senha` agora exige JWT com cargo `Gerente`. Use primeiro um funcionario do seed com a senha `Senha@123`; depois, um gerente pode definir ou redefinir senhas de funcionarios.

### 1. Crie a senha de um funcionário

```
POST /api/v1/autenticacao/criar-senha
```

```json
{
  "cpf": "CPF_DO_FUNCIONARIO",
  "senha": "MinhaS3nha@",
  "confirmacaoSenha": "MinhaS3nha@"
}
```

> O seed do banco (`infra/sql/init.sql`) já cria funcionários com CPF pré-definido para testes. A senha padrão dos seeds é `Senha@123`.

### 2. Faça login

```
POST /api/v1/autenticacao/login
```

```json
{
  "cpf": "CPF_DO_FUNCIONARIO",
  "senha": "Senha@123"
}
```

### 3. Use o token retornado

No Swagger, clique em **Authorize** e insira o token (sem a palavra "Bearer").

---

## 📋 Endpoints disponíveis

| Grupo | Endpoint base | Autenticação |
|---|---|---|
| Autenticação | `/api/v1/autenticacao` | Pública |
| Clientes | `/api/v1/clientes` | JWT |
| Funcionários | `/api/v1/funcionarios` | JWT |
| Veículos | `/api/v1/veiculos` | JWT |
| Peças | `/api/v1/pecas` | JWT |
| Ordens de Serviço | `/api/v1/ordens-servico` | JWT (GET público) |
| Itens de OS | `/api/v1/itens-os` | JWT |
| Monitoramento | `/api/v1/monitoramento` | JWT |

> O endpoint `GET /api/v1/ordens-servico/{id}/status` é público, permitindo que o cliente consulte o status da sua OS sem autenticação.

---

## 🐳 Variáveis de ambiente

A aplicação pode ser configurada via variáveis de ambiente (substituem o `appsettings.json`):

| Variável | Descrição | Valor padrão no Docker |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | String de conexão com o banco | Configurado no compose |
| `Jwt__SecretKey` | Chave secreta para geração de tokens JWT | Configurado no compose |
| `Jwt__Issuer` | Emissor do token | `PosTechChallenge` |
| `Jwt__Audience` | Audiência do token | `PosTechChallenge-API` |
| `Jwt__AccessTokenExpirationMinutes` | Validade do access token (minutos) | `15` |

---

## 📊 Monitoramento

O endpoint `GET /api/v1/monitoramento/tempo-execucao-medio` retorna métricas de tempo médio de execução por rota, coletadas em tempo real via middleware.

---

## 📁 Estrutura do repositório

```
├── Doc/                            # Documentação DDD (Event Storming, Diagramas)
├── PosTechChallenge/               # Projeto da API Web
│   ├── Controllers/                # Controllers REST
│   ├── Dockerfile                  # Build da aplicação
│   ├── Middleware/                 # Middlewares (ex: timing)
│   └── Monitoring/                 # Monitor de tempo de execução
├── PosTechChallenge.Applicacao/    # Camada de aplicação
│   ├── Services/                   # Application services
│   └── UseCases/                   # Use cases por entidade
├── PosTechChallenge.Dominio/       # Camada de domínio
│   ├── Model/                      # Entidades
│   ├── Services/                   # Domain services
│   └── ValueObjects/               # CPF, CNPJ, Senha, Placa
├── PosTechChallenge.Infraestrutura/ # Camada de infraestrutura
│   ├── Querys/                     # SQL queries (Dapper)
│   └── Repositorios/               # Implementações dos repositórios
├── PosTechChallenge.Testes/        # Projeto de testes unitários
│   ├── DomainServices/             # Testes dos Domain Services
│   └── ValueObjects/               # Testes dos Value Objects
├── infra/sql/init.sql              # Script de criação e seed do banco
└── docker-compose.yml              # Orquestração do ambiente completo
```
