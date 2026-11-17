# Fase 4 — Rastreio de tarefas

Checklist da Fase 4 (microsserviços, Saga, mensageria). Atualizado conforme as
tarefas fecham, para o time ver onde estamos sem precisar perguntar.

**Início:** 01/11/2026 · **Última atualização:** 17/11/2026
**Estado:** vídeo gravado e ambiente destruído em 17/11. Falta a entrega (PDF e envio).

## Legenda

- `[x]` concluída
- `[ ]` pendente

---

## O que falta

### Para gravar o vídeo

- [ ] **Ensaiar uma vez** seguindo `postech-os-service/docs/cenarios-do-video.md`, com os dados de `docs/massa-de-testes.md`
- [ ] **Abrir o Datadog e conferir os traces** — os quatro serviços passaram a enviar (antes só chegavam logs). O que só dá para ver na tela: se um trace liga os serviços através da fila. Se não ligar, o vídeo usa os logs pelo Correlation ID
- [ ] **Gravar** — o [PR #4 do `postech-execucao-service`](https://github.com/Gustavollier/postech-execucao-service/pull/4) está pronto para ser mergeado ao vivo no cenário de deploy. **Não mergear antes**
- [ ] **Publicar o vídeo** no YouTube ou Vimeo (público ou não listado)

### Para a entrega

- [ ] **Nomes e RMs dos participantes** — entram no PDF
- [ ] **PDF de entrega** — participantes, links dos repositórios e do vídeo, diagrama, estratégia da Saga, justificativa da divisão e das tecnologias
- [ ] **Enviar o PDF** no portal

### Pipelines dos repositórios de infra

- [ ] **Estão sem funcionar, e não é pelo código.** Os secrets `AZURE_SUBSCRIPTION_ID` do `postech-infra-k8s` e do `postech-infra-db` ainda apontam para a assinatura antiga (o plano falha com `SubscriptionNotFound`), e o app `postech-13soat-github-oidc` não tem papel nenhum na assinatura nova. Para religar: atualizar o secret, dar *Contributor* no resource group e acesso aos segredos do Key Vault a esse app, e conferir se `DD_API_KEY` e `DD_APP_KEY` são as chaves em uso — se forem antigas, um `apply` pela pipeline troca as chaves do cluster e derruba o envio ao Datadog
- Enquanto isso, mudança de infra é `terraform apply` local com login de Owner, como foi feito em 16/11

### Higiene

- [ ] **Proteger a `main`** dos quatro repositórios de serviço e do `postech-front` — *Settings → Branches*: exigir PR e o check **Build, testes e cobertura**. Hoje nenhuma está protegida
- [ ] **Confirmar que os tokens do Mercado Pago foram renovados** depois de terem aparecido no chat (o que está no Key Vault é válido e é do vendedor de teste)
- [ ] **SonarCloud, Identidade:** marcar como *Accept* o S6781 (chave JWT lida da configuração; é falso positivo)

### Ambiente

- [x] **Destruído em 17/11/2026**, depois do vídeo gravado: `terraform destroy` do `postech-infra-k8s` (68 recursos) e do `postech-infra-db` (15). Não há mais APIM, AKS, SQL, Key Vault, registro de imagens nem Auth Function. Ficaram só o resource group `pos-tech-fiap` e o storage do state (`postechtfstate13soat`), que custam centavos
- O painel publicado continua abrindo, mas não funciona: a API não existe mais
- Para subir de novo: `terraform apply` no `postech-infra-db`, gravar os quatro segredos de fora no Key Vault, `apply` no `postech-infra-k8s` (uns 40 min pelo APIM), deploy dos serviços, e `docs/massa/semear.py` para a massa. O IP do APIM e o do ingress mudam; o webhook do Mercado Pago continua o mesmo se o nome do APIM for o mesmo

### Opcional, se sobrar tempo

- [ ] Billing chamar o estorno de verdade no Mercado Pago (`POST /v1/orders/{id}/refund`) — hoje o estorno é o evento de domínio
- [ ] Validar a assinatura (`x-signature`) do webhook do Mercado Pago — hoje o Billing se protege consultando o pedido no adquirente
- [ ] Code smells que o SonarCloud aponta (nenhum afeta o quality gate)

---

## Feito em 15 e 16/11

- [x] Ambiente aplicado na Azure: APIM Developer, AKS, SQL, Key Vault, ACR, Auth Function; os quatro serviços com deploy pela pipeline
- [x] Os quatro segredos de fora da Azure gravados no Key Vault
- [x] Alerta de orçamento de US$ 50 criado
- [x] Webhook do Mercado Pago configurado e chegando sozinho — o pagamento do Pix de teste confirma sem intervenção
- [x] APIM voltou a alcançar os serviços (a sonda do load balancer recebia 404 e derrubava os nós)
- [x] Billing manda itens e descrição do extrato no pedido (Qualidade da Integração do Mercado Pago)
- [x] **Autorização por perfil e posse nos quatro serviços** — antes, um token de cliente lia ordens, orçamentos e o cadastro da oficina inteira. Tabela por rota no README de cada serviço, em *Quem pode o quê*
- [x] **Cluster fechado para tráfego que não venha do APIM** — o IP do ingress respondia direto da internet, sem token
- [x] Painel (`postech-front`) revisado e publicado: lista de clientes, tela de Demonstração, ordem cancelada, peça do estoque no diagnóstico, trilha da saga, rótulos legíveis
- [x] Teste de ponta a ponta pela interface (`postech-front/e2e/fluxo.mjs`, 16 passos) e coleção do Postman verdes contra o ambiente real
- [x] `docs/cenarios-do-video.md`: o que mostrar e o que falar em cada cenário
- [x] **Terraform do `postech-infra-k8s` reproduz o ambiente** — reconstruído a partir do state (ingress controller, nós ARM64, Auth Function), com a sonda TCP e a restrição ao IP do APIM no código. `terraform plan` nos dois repositórios de infra: *No changes*. Branches `fase4/regiao-canadacentral` mergeadas na `main`
- [x] **Base limpa e massa de testes** — 5 funcionários, 7 clientes, 9 peças e 7 ordens em pontos diferentes da saga (`postech-os-service/docs/massa-de-testes.md`, criada por `docs/massa/semear.py`)
- [x] **APM no Datadog** — os pods não recebiam o tracer .NET por falta de um rótulo; os quatro serviços agora enviam traces
- [x] PR do cenário de deploy preparado no `postech-execucao-service`

Testes por serviço hoje: OS 122, Billing 117, Execução 141, Identidade 122.

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
