# Fase 4 — Rastreio de tarefas

Checklist da Fase 4 (microsserviços, Saga, mensageria). Atualizado conforme as
tarefas fecham, para o time ver onde estamos sem precisar perguntar.

**Início:** 01/11/2026 · **Última atualização:** 03/11/2026
**Decisão de calendário:** o ambiente na Azure só sobe perto do dia da entrega.
Até lá, tudo que não depende dele está pronto.

## Legenda

- `[x]` concluída
- `[ ]` pendente
- **⏳ no dia** — só acontece com o ambiente no ar, perto da gravação

---

## Pendências

### 👤 Gustavo (o que só você consegue fazer)

**Agora**

- [ ] **Nomes e RMs dos participantes** — entram no PDF de entrega
- [ ] **Alerta de orçamento na Azure** — portal → *Gerenciamento de Custos → Orçamentos → Adicionar*, US$ 50, alertas em 50% e 80%. Antes de qualquer `apply`
- [ ] **Proteger a `main`** dos quatro repositórios de serviço — *Settings → Branches → Add rule*: branch `main`, *Require a pull request before merging*, *Require status checks to pass* com **Build, testes e cobertura**
- [ ] **Renovar os dois tokens do Mercado Pago** (vendedor e comprador de teste) — apareceram no chat. O do vendedor, renovado, é o que vai para o Key Vault
- [ ] **Conferir se a conta do Datadog ainda está ativa** — o `postech-infra-k8s` precisa das chaves (`DD_API_KEY`, `DD_APP_KEY`) que estão nos secrets dele; se o trial acabou, o dashboard e o rastreamento do vídeo ficam sem destino
- [ ] **SonarCloud, Identidade:** marcar como *Accept* o S6781 (chave JWT lida da configuração). É falso positivo: a chave vem do Secret do Kubernetes, alimentado pelo Key Vault, e não de arquivo versionado
- [ ] **Revisar e aprovar** as branches `fase4/regiao-canadacentral` do `postech-infra-db` e do `postech-infra-k8s` (não mergear antes do dia: o merge na `main` dispara `apply`)

**⏳ No dia de subir o ambiente**

- [ ] **Gravar no Key Vault os quatro segredos de fora da Azure**, depois do `apply` do `postech-infra-db` (o comando lê o valor sem ele aparecer na tela; passo a passo no dia):
  - `billing-mongo-connection-string` — usuário `billing-svc` do Atlas
  - `execucao-mongo-connection-string` — usuário `execucao-svc` do Atlas
  - `rabbitmq-connection-string` — AMQP URL do CloudAMQP
  - `mercadopago-access-token` — o token do vendedor de teste, **já renovado**
- [ ] **Webhook do Mercado Pago** — na aplicação de teste: *Configuração de webhooks*, tópico **Order**, URL `https://<apim>.azure-api.net/billing/api/v1/pagamentos/webhook`
- [ ] **Gravar o vídeo** seguindo `postech-os-service/docs/roteiro-do-video.md`
- [ ] **Publicar o vídeo** no YouTube ou Vimeo (público ou não listado)
- [ ] **Enviar o PDF** no portal

**Depois da entrega**

- [ ] **Confirmar o teardown** — eu rodo, mas o "pode destruir" é seu

### 🤖 Claude

**Agora**

- [ ] **Rascunho do PDF de entrega** — participantes (com os dados do Gustavo), links dos repositórios e do vídeo, diagrama, estratégia da Saga, justificativa da divisão e das tecnologias
- [ ] **Dashboard do Datadog** — os widgets de eventos ainda procuram nomes do monólito (`OrdemServicoCriada`, `TransicaoStatusOrdemServico`); trocar pelos eventos da saga
- [ ] **Pipelines dos repositórios de infra e da Auth Function** — usam o app OIDC antigo (`postech-13soat-github-oidc`), que ficou **sem papel nenhum** quando a assinatura foi recriada. Decidir com o Gustavo: dar papel a ele ou fazer o `apply` daqui, com o login de Owner (mais simples para uma vez só)

**⏳ No dia de subir o ambiente** (cerca de 1h, metade esperando o APIM)

- [ ] `apply` do `postech-infra-db` — e conceder *Key Vault Secrets Officer* a quem roda, logo depois de o cofre existir (sem isso o Terraform não grava os segredos)
- [ ] Rodar a criação dos usuários contidos `os_app` e `identidade_app` (`sql/usuario-do-servico.sql`)
- [ ] `apply` do `postech-infra-k8s` (APIM **Developer**, ~40 min para subir)
- [ ] Publicar a imagem da Auth Function no ACR
- [ ] Deploy dos quatro serviços (os pipelines detectam o ambiente e deixam de pular)
- [ ] Criar o primeiro funcionário (pasta 0 da coleção, por port-forward)
- [ ] Rodar a coleção do Postman inteira contra o APIM
- [ ] **Verificar no Datadog se o trace atravessa a fila** e liga os quatro serviços — depende do agente instrumentar o cliente do RabbitMQ; se não ligar, o vídeo usa os logs por `correlationId`, que já funcionam

**Depois da entrega**

- [ ] `terraform destroy` dos dois repositórios de infra

### Opcional, se sobrar tempo

- [ ] Code smells que o SonarCloud aponta (4 a 8 por serviço, nenhum afeta o quality gate)
- [ ] Billing chamar o estorno de verdade no Mercado Pago (`POST /v1/orders/{id}/refund`) — hoje o estorno é o evento de domínio

---

## O que já está pronto

### Contas e acessos

- [x] Repositórios criados: `postech-os-service`, `postech-billing-service`, `postech-execucao-service`, `postech-identidade-service`
- [x] Azure: login, resource providers registrados, resource group `pos-tech-fiap` e storage do state do Terraform, em **`canadacentral`** (a assinatura passou a aceitar só brazilsouth, canadacentral, francecentral, italynorth e belgiumcentral)
- [x] OIDC da Fase 4: app `postech-13soat-fase4-oidc` com 12 credenciais federadas (4 repositórios × `main`, `develop`, PR) e **Contributor** no resource group — o login dos pipelines funciona
- [x] Secrets `AZURE_*` e `SONAR_TOKEN` nos quatro repositórios de serviço
- [x] SonarCloud: organização `postech4` (plano Free), os quatro projetos públicos, branch principal `main`, análise no CI com cobertura
- [x] MongoDB Atlas: projeto `postech-fase4`, cluster M0, `0.0.0.0/0` liberado, usuários `billing-svc` (`readWrite` só em `billing-db`) e `execucao-svc` (`readWrite` só em `execucao-db`)
- [x] CloudAMQP: instância criada
- [x] Mercado Pago: aplicação `postech-oficina`, contas de teste vendedor e comprador, **pedido Pix criado com sucesso pela API de Orders**

### Serviços

| Serviço | Banco | Testes | Cobertura | Pipeline |
| --- | --- | --- | --- | --- |
| OS (orquestrador da saga) | Azure SQL | 96, incluindo 5 cenários BDD | 92% | verde |
| Billing | MongoDB | 91 | 88% | verde |
| Execução | MongoDB | 120 | 94% | verde |
| Identidade | Azure SQL | 95 | 95% | verde |

Coberturas como o SonarCloud mede; todos com o quality gate OK.

- [x] Saga orquestrada no OS Service: caminho feliz e as três compensações (orçamento rejeitado, pagamento recusado volta um passo, execução que não assume no prazo estorna e cancela)
- [x] Outbox transacional, idempotência e dead letter nos três serviços que usam a fila
- [x] **BDD com Reqnroll** — o fluxo da saga em Gherkin, em português, contra SQL Server e RabbitMQ reais
- [x] Execução: reserva de peça **por ordem**, atômica no Mongo (corrigiu o defeito do laboratório, em que uma devolução repetida consumia a reserva de outra ordem)
- [x] Identidade: UNIQUE em CPF, CNPJ e placa, CHECK de um documento só, bloqueio de login após 5 erros — as lacunas que a Fase 3 tinha deixado anotadas
- [x] Billing: **cobrança pela API de Orders do Mercado Pago** — a de pagamentos recusava a credencial de teste. Com `MercadoPago__NomeDoPagadorDeTeste=APRO`, o Pix de teste é aprovado sozinho e o webhook chega como num pagamento real
- [x] Webhook do Billing só consulta pedido que nós criamos (corrige a vulnerabilidade S7044 apontada pelo SonarCloud)
- [x] OS e Identidade aplicam o próprio schema na partida — banco novo não tem passo manual

### Qualidade e CI/CD

- [x] Pipeline por serviço: build, testes com Testcontainers, **gate de 80%**, SonarCloud com cobertura, imagem e deploy no AKS
- [x] Deploy se pula com aviso quando o ambiente não existe (e reprova em erro de permissão ou rede)
- [x] Actions de terceiros fixadas por hash; pods sem token de service account montado
- [x] **Fluxo completo validado de ponta a ponta com os quatro serviços reais** (local, com proxy no lugar do APIM e adquirente falso no formato da API de Orders): a saga conclui pelo webhook. Achou e corrigiu um defeito de configuração no Billing que ignorava a URL do Mercado Pago

### Infraestrutura (branches `fase4/regiao-canadacentral`, ainda sem `apply`)

- [x] `postech-infra-db`: `os-db` e `identidade-db` com usuário contido cada, Key Vault, sem `prevent_destroy`. `plan`: **15 a criar**
- [x] `postech-infra-k8s`: uma API no APIM por serviço, um Service público e um Secret por serviço, APIM criado pelo Terraform, Auth Function lendo o `identidade-db`. Tier **Developer** (o Consumption não tem `rate-limit-by-key` nem logs para o Datadog). `plan` parcial: AKS 2× `D2as_v4` e ACR
- [x] Custo estimado com tudo ligado: **~US$ 7,50/dia** (~US$ 4–5 com o AKS parado à noite)

### Documentação e entrega

- [x] ADRs: Saga orquestrada, banco por serviço, CQRS sem infraestrutura nova
- [x] `docs/arquitetura.md` nos quatro repositórios, com diagrama de componentes e da saga (PNG para o PDF)
- [x] READMEs com selos de quality gate, cobertura e CI (evidência de cobertura)
- [x] Swagger em cada serviço e **coleção do Postman** com o fluxo completo (`postech-os-service/docs/postman`)
- [x] **Roteiro do vídeo** (`postech-os-service/docs/roteiro-do-video.md`): sete blocos em 15 minutos, checklist do dia e plano B para cada risco

---

## Decisões registradas

| Decisão | Por quê |
| --- | --- |
| Quatro serviços: Identidade, OS, Execução, Billing | Cada um muda por motivo próprio; Identidade é consultada, não coordenada |
| Saga **orquestrada**, orquestrador no OS Service | Um lugar só para o estado da saga e para a compensação (ADR-0006) |
| SQL no OS e no Identidade, MongoDB no Billing e na Execução | Transação local onde a consistência é transacional; documento onde o agregado é lido inteiro (ADR-0007) |
| RabbitMQ gerenciado (CloudAMQP), fora do cluster | Plano gratuito, e o broker não disputa a quota de 4 vCPU |
| Região `canadacentral` | Política da assinatura; mais barata que brazilsouth |
| APIM **Developer** | Mantém o rate limit por IP e os logs no Datadog, que o Consumption não tem |
| Ambiente só perto da entrega | Crédito de estudante; ~US$ 7,50/dia ligado |
| Mercado Pago pela **API de Orders** | A credencial de teste das contas novas só funciona nela |
