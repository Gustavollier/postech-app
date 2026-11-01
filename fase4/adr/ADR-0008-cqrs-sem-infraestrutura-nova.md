# ADR-0008 — CQRS onde ele se paga, e só ali

**Status:** aceito
**Data:** 01/11/2026

## Contexto

A pergunta surgiu com a restrição junto: usar CQRS se fizer sentido, sem gastar
crédito. São duas perguntas diferentes dentro de uma, e elas têm respostas
opostas.

CQRS "completo" — modelo de escrita e modelo de leitura em **armazenamentos
separados**, alimentados por projeções — custa um banco a mais por serviço, um
processo de projeção a mais para manter no ar, e um modo de falha novo (a
projeção atrasada, que faz o usuário salvar algo e não ver na tela seguinte).
Com US$ 100 para a fase e quota de 4 vCPU, isso é dinheiro e risco trocados por
elegância.

Mas a decomposição criou um problema concreto que pede exatamente a parte boa do
CQRS.

## O problema que a divisão criou

A tela de detalhe de uma ordem mostra, numa coisa só: o status e o histórico
(do OS Service), o nome do cliente e o veículo (do Identidade), os itens
lançados (do Execução) e o valor do orçamento (do Billing).

No monólito isso era um `JOIN`. Depois da divisão, viram **três chamadas entre
serviços por abertura de tela** — e, pior, a tela passa a depender de os três
estarem no ar. Uma listagem de ordens multiplica isso por linha.

## Decisão

**Sim para CQRS como padrão de código, não para CQRS como infraestrutura.**

Em concreto, três coisas:

**1. Um read model da ordem, dentro de `os-db`.** Uma tabela `OrdemResumo`
mantida pelos eventos que o orquestrador já consome. Quando chega
`DiagnosticoConcluido`, grava os itens e o total; quando chega `OrcamentoGerado`,
grava o valor; o nome do cliente e o veículo vieram no `OrdemAberta`. A tela lê
uma linha, de um banco só.

**O custo em infraestrutura é zero**: é uma tabela num banco que já existe e já
está pago. Não há banco de leitura, não há processo de projeção separado — quem
atualiza é o mesmo consumidor que já roda para mover a saga.

**2. Separação de comando e consulta dentro de cada serviço.** Escrita passa pelo
caso de uso e pelo modelo de domínio; leitura vai direto do repositório para o
DTO, sem carregar agregado. O monólito já fazia isso informalmente, com Dapper
devolvendo DTO nas consultas. Aqui vira regra, e continua custando nada.

**3. Nada de event sourcing.** O estado continua guardado como estado. Event
sourcing resolveria auditoria, que não é requisito desta fase, ao preço de
reescrever toda a persistência.

## O que isso não é

Não é CQRS canônico, e o documento de entrega não vai chamar de CQRS completo.
É **um read model desnormalizado alimentado por evento**, que é a técnica que
resolve o problema real. Chamar de CQRS completo seria vender mais do que foi
feito, e a banca costuma perguntar justamente pelo banco de leitura separado que
não existe.

## Consequências

- A consistência do read model é **eventual**, na ordem de milissegundos. Para
  uma tela de acompanhamento de oficina é aceitável, e a escrita que o usuário
  acabou de fazer passa pelo caminho de comando, que é imediato.
- O read model é **descartável**: se divergir, é reconstruído relendo os eventos.
  Nada de negócio depende dele, e nenhuma decisão é tomada a partir dele.
- O read model fica em `os-db`, não num banco novo. Se um dia o volume pedir
  separação, a mudança é de infraestrutura e não de código.
- Dado duplicado cresce: o nome do cliente passa a existir no Identidade e no
  resumo da ordem. É o mesmo trade que já aceitamos nos eventos (ADR-0007), e
  pelo mesmo motivo — a tela não pode depender de três serviços no ar.
