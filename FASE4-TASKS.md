# Fase 4 — Rastreio de tarefas

Checklist de execução da Fase 4 (microsserviços, Saga, mensageria).
Atualizado conforme as tarefas são concluídas, para o time ver onde estamos sem
precisar perguntar.

**Início:** 01/11/2026
**Plano completo:** board no Miro (arquitetura, saga e custos) e o Mapa da Fase 4.

## Legenda

- `[x]` concluída
- `[ ]` pendente
- **⛔ impeditivo** — a tarefa não avança até alguém destravar; o motivo está escrito na linha

## Ferramentas do ambiente de desenvolvimento

Todas instaladas e verificadas. Tudo que for marcado como concluído aqui foi
compilado e testado antes de receber o X.

| Ferramenta | Versão | Como foi obtida |
| --- | --- | --- |
| .NET SDK | 10.0.114 e 8.0.133 | `apt-get install dotnet-sdk-10.0 dotnet-sdk-8.0` |
| Docker | 29.9.2, daemon rodando | `dockerd` local, storage driver overlayfs |
| Azure CLI | 2.91.0 | `pip install azure-cli` num venv em `/opt/azcli` |
| kubectl | 1.37.2 | `az aks install-cli` |
| kubelogin | 0.2.22 | idem |
| Terraform | 1.9.8 | binário de releases.hashicorp.com |

O instalador oficial do .NET (`dot.net`) está bloqueado na saída de rede desta
máquina; o pacote do Ubuntu 24.04 resolve e entrega as duas versões que o
projeto usa.

### O que ainda depende de você

- **Login no Azure.** O `az` está instalado mas sem credencial. Um
  `az login --use-device-code` resolve: eu gero o código, você autentica.
- **Contas de terceiros.** Atlas, CloudAMQP, SonarCloud e Mercado Pago precisam
  de cadastro com os seus dados.
- **Criação dos repositórios.** Nome, visibilidade e dono são decisão sua.

---

## Etapa 0 — Decidir e abrir contas

Meta: sair do zero com as decisões fechadas, as contas gratuitas criadas e a
queima da Azure em repouso.

- [x] Levantar os requisitos da Fase 4 e pontuar criticidade
- [x] Desenhar a arquitetura alvo com os recursos e onde cada um mora
- [x] Decidir a divisão em serviços: Identidade, Ordem de Serviço, Billing, Execução
- [x] Decidir a estratégia da Saga: orquestrada, com o orquestrador no OS Service
- [x] Decidir onde fica o NoSQL: Billing e Execução em MongoDB
- [x] Decidir o broker: RabbitMQ gerenciado, fora do cluster
- [x] Registrar a decisão da Saga em ADR — `fase4/adr/ADR-0006-saga-orquestrada.md`
- [x] Registrar a decisão de bancos por serviço em ADR — `fase4/adr/ADR-0007-banco-por-servico.md`
- [ ] **⛔ Criar alerta de orçamento na Azure em US$ 50 e US$ 80** — precisa do portal
- [ ] **⛔ Pedir aumento de quota de vCPU** — precisa do portal, e a aprovação leva dias
- [ ] **⛔ Confirmar o cluster AKS parado** — precisa de `az`
- [ ] **⛔ Deletar o APIM para parar a queima de US$ 1,70/dia** — precisa de `az`
- [ ] **⛔ Abrir conta MongoDB Atlas e criar o cluster M0** — precisa de cadastro
- [ ] **⛔ Abrir conta CloudAMQP no plano Little Lemur** — precisa de cadastro
- [ ] **⛔ Abrir conta SonarCloud e ligar nos repositórios** — precisa de cadastro
- [ ] **⛔ Abrir conta Mercado Pago e pegar credenciais de sandbox** — precisa de cadastro
- [ ] **⛔ Criar o repositório `postech-identidade-service`** — criar repositório é decisão de vocês
- [ ] **⛔ Criar o repositório `postech-os-service`** — idem
- [ ] **⛔ Criar o repositório `postech-billing-service`** — idem
- [ ] **⛔ Criar o repositório `postech-execucao-service`** — idem
- [ ] **⛔ Proteger a `main` dos quatro repositórios novos** — depende dos repositórios existirem

---

## Etapa 1 — Um evento atravessando dois serviços

Meta: provar a decisão mais cara da fase enquanto ela ainda é barata de mudar.
Tudo local, custo zero.

- [x] Definir o envelope de mensagem com `X-Correlation-ID` propagado — `fase4/contratos/Envelope.cs`
- [x] Definir os contratos dos eventos de domínio — `fase4/contratos/Eventos.cs`
- [x] Definir a topologia do RabbitMQ: exchange, filas e assinaturas — `fase4/contratos/Eventos.cs`
- [x] Generalizar o `EmailOutbox` para um outbox transacional de eventos — `fase4/outbox/`
- [x] Tabela de idempotência do consumidor (`EventoProcessado`) — `fase4/outbox/schema-mensageria.sql`
- [x] `docker-compose` local com RabbitMQ, MongoDB e SQL Server — `fase4/docker-compose.fase4.yml`, validado com `docker compose config`
- [x] Solução `fase4/PosTech.Fase4.slnx` compilando, 0 erros e 0 warnings
- [x] Publicador que lê o outbox e publica no RabbitMQ — `PublicadorRabbitMq`, com confirmação do broker ligada
- [x] Declaração idempotente da topologia: exchange, filas, dead letters e bindings
- [x] Consumidor base com idempotência, retry em memória e dead letter — `ConsumidorDeEventos`
- [x] Processador do outbox com backoff exponencial e limite de tentativas
- [x] 26 testes unitários de contrato e de outbox
- [x] **Teste de integração provando o evento ponta a ponta** — 3 testes contra um RabbitMQ real via Testcontainers
- [ ] **⛔ Esqueleto .NET do OS Service publicando `OrdemAberta`** — precisa do repositório criado
- [ ] **⛔ Esqueleto .NET do Billing consumindo `DiagnosticoConcluido`** — idem

**Marco da Etapa 1 atingido.** `dotnet test` em `fase4/`: 29 passando, 0 falhas.
O `OrdemAberta` sai do outbox, atravessa um RabbitMQ de verdade e chega no
consumidor com o `correlationId` intacto; a entrega repetida é descartada; e a
mensagem que falha sempre termina na dead letter.

---

## Etapa 2 — Decompor o domínio

Meta: quatro serviços, quatro bancos, nenhum join atravessando fronteira.

- [x] Mapear o schema atual por dono (qual tabela vai para qual serviço) — `fase4/mapa-do-schema.md`
- [x] Listar todo join que hoje cruza contextos e definir o substituto de cada um — só dois, e os dois somem
- [ ] **⛔ OS Service: migrar ordem, status e histórico** — precisa do repositório e de `dotnet`
- [ ] **⛔ OS Service: migrar os testes de domínio que acompanham a OS** — idem
- [x] **Identidade: value objects de CPF e CNPJ, cópia única do contexto** — `PosTech.Identidade`
- [x] Schema do `identidade-db` com Cliente, Veiculo, Funcionario e Seguranca
- [x] **UNIQUE em `Cliente.CPF`, `Cliente.CNPJ` e `Funcionario.CPF`** — fecha a lacuna apontada na Fase 3
- [x] CHECK garantindo exatamente um documento por cliente
- [x] 22 testes de documento, incluindo a trava de divergência com a Auth Function
- [ ] **⛔ Apontar a Auth Function para o `identidade-db`** — precisa do repositório criado
- [x] **Billing: orçamento como documento no MongoDB** — `PosTech.Billing`, com outbox embutido no agregado
- [x] Regra de montagem do orçamento, pura e testada: o valor sai da soma dos subtotais, com o preço do dia
- [x] Índice único por ordem, para uma OS não ter dois orçamentos
- [x] 15 testes do Billing, 7 deles contra um MongoDB real
- [ ] **⛔ Billing: pagamento e integração com o Mercado Pago** — precisa das credenciais de sandbox
- [x] **Execução: fila, itens e peças** — `PosTech.Execucao`, documentos com outbox embutido
- [x] Transições da oficina puras e testadas: receber, diagnosticar, liberar, finalizar, cancelar
- [x] Reserva de peças no diagnóstico, para dois orçamentos não prometerem a mesma peça
- [x] Formato do fio fixado por teste: acentuação legível e enum por nome
- [x] 24 testes do Execução e do estoque
- [ ] **⛔ Fluxo feliz rodando ponta a ponta no compose local** — precisa do daemon do Docker

---

## Etapa 3 — Saga e Mercado Pago

Meta: a saga completa, com as compensações testadas.

- [x] Desenhar a máquina da saga a partir do `OrdemServicoDomainService` existente — `fase4/adr/ADR-0006-saga-orquestrada.md`
- [x] Corrigir a ordem dos passos: o diagnóstico entra entre abertura e orçamento
- [x] Decidir o uso de CQRS — `fase4/adr/ADR-0008-cqrs-sem-infraestrutura-nova.md`
- [x] **Orquestrador da saga como máquina pura, testável sem banco e sem broker** — `PosTech.Saga`
- [x] Caminho feliz: abrir, diagnosticar, orçar, aprovar, pagar, executar, fechar
- [x] Compensação: orçamento rejeitado
- [x] Compensação: pagamento recusado (volta um passo, não cancela)
- [x] Compensação: execução não assume no prazo (estorna e cancela)
- [x] Compensação só desfaz o que de fato aconteceu (não inventa estorno)
- [x] 11 testes do orquestrador, incluindo evento fora de ordem e repetido
- [ ] Atualizar o frame "Saga" do board no Miro para os seis passos
- [x] **Persistência real da saga em Azure SQL** — `SagaRepositorio`, com o estado e os eventos de compensação na mesma transação
- [x] Outbox e idempotência em Dapper/Azure SQL — `OutboxRepositorio`, `IdempotenciaRepositorio`
- [x] Tabela `SagaOrdemServico` no schema idempotente, com índice para o varredor de timeout
- [x] Varredor de sagas paradas além do prazo — `ObterParadasAsync`
- [x] 8 testes de integração contra um SQL Server real, incluindo a atomicidade do outbox
- [x] Read model descartado por decisão do líder técnico (ADR-0008 revisado): tabela nova está fora
- [ ] **⛔ Integração com o Mercado Pago em sandbox** — precisa das credenciais
- [ ] **⛔ Persistir o estado da saga no OS Service** — precisa do repositório e de `dotnet`
- [ ] **⛔ Caminho feliz: abrir, orçar, aprovar, pagar, executar, fechar** — idem
- [ ] **⛔ Compensação: pagamento recusado** — idem
- [ ] **⛔ Compensação: execução não assume dentro do prazo** — idem
- [ ] **⛔ Compensação: falha ao publicar, com reenvio pelo outbox** — idem
- [ ] **⛔ Integração com o Mercado Pago em sandbox** — precisa das credenciais
- [ ] **⛔ Webhook de confirmação de pagamento** — precisa de URL pública (ngrok local)
- [ ] **⛔ Teste automatizado que força a recusa e verifica a compensação** — precisa de `dotnet`

---

## Etapa 4 — Qualidade e pipelines

Meta: quatro pipelines verdes com build, teste, qualidade e deploy.

- [ ] **⛔ BDD do fluxo da saga com Reqnroll** — precisa dos repositórios e de `dotnet`
- [ ] **⛔ Gate de cobertura de 80% reprovando no CI de cada serviço** — idem
- [ ] **⛔ SonarCloud no CI de cada serviço** — precisa da conta SonarCloud
- [ ] **⛔ Pipeline do Identidade: build, teste, qualidade, imagem, deploy** — precisa do repositório
- [ ] **⛔ Pipeline do OS Service** — idem
- [ ] **⛔ Pipeline do Billing** — idem
- [ ] **⛔ Pipeline do Execução** — idem

### Ensaio de infraestrutura (1 dia ligado, cerca de US$ 5)

- [ ] **⛔ Subir um deployment de teste e confirmar que 4 serviços cabem em 4 vCPU** — precisa de `kubectl`
- [ ] **⛔ Desligar o cluster no fim do ensaio** — precisa de `az`

---

## Etapa 5 — Azure de verdade

Meta: o ambiente publicado, com o trace atravessando os serviços.
Aqui o crédito começa a sair. Parar o cluster toda noite.

- [ ] **⛔ Recriar o APIM** — precisa de `az` ou do portal, e leva 40 min
- [ ] **⛔ Terraform dos bancos `os-db` e `identidade-db`** — precisa de `terraform`
- [ ] **⛔ Rotas novas no APIM para os quatro serviços** — idem
- [ ] **⛔ Guardar os segredos do Atlas e do CloudAMQP no Key Vault** — precisa de `az`
- [ ] **⛔ Deploy dos quatro serviços no AKS** — precisa de `kubectl`
- [ ] **⛔ Propagar o contexto de trace dentro da mensagem** — precisa de `dotnet`
- [ ] **⛔ Confirmar no Datadog um trace cruzando os quatro serviços** — precisa do ambiente no ar

---

## Etapa 6 — Vídeo e entrega

- [ ] Roteiro do vídeo de 15 minutos
- [ ] Documentação de arquitetura por serviço
- [ ] PDF de entrega com participantes, links, diagrama e justificativas
- [ ] **⛔ Ensaiar a falha da saga antes de gravar** — precisa do ambiente no ar
- [ ] **⛔ Gravar o vídeo** — idem
- [ ] **⛔ Teardown total depois da entrega** — precisa de `az`
