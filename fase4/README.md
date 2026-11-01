# fase4 — área de preparação

Esta pasta guarda o que a Fase 4 precisa **antes** de os repositórios dos
microsserviços existirem: os contratos de evento, o outbox transacional, o
compose local e os ADRs.

Nada aqui entra na compilação. A pasta é irmã dos projetos, não filha de nenhum
deles, então o `PosTechChallenge.sln` e os `.csproj` ignoram estes arquivos. É de
propósito: enquanto os repositórios novos não existem, este código precisa ter
um lugar sem poder quebrar o build da Fase 3.

## Como isto vira código de verdade

Quando os quatro repositórios forem criados, o conteúdo de `contratos/` e
`outbox/` é copiado para dentro de cada serviço. Os contratos são **duplicados
de propósito**, não compartilhados por pacote: um serviço que muda o próprio
contrato não pode quebrar a compilação dos outros no mesmo commit. A versão do
evento (`Versao` no envelope) é o que mantém os lados em acordo, não o compilador.

| Pasta | Destino |
| --- | --- |
| `contratos/` | copiado para os quatro serviços |
| `outbox/` | copiado para os serviços que publicam evento |
| `docker-compose.fase4.yml` | base do compose local de cada serviço |
| `adr/` | vai para a documentação da entrega |

## Estado

Escrito sem compilador disponível no ambiente de automação. Rode um
`dotnet build` antes de confiar em qualquer arquivo daqui.
