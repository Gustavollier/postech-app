# Mapa do schema por dono

De qual serviço é cada tabela de hoje, e o que fazer com os `JOIN` que deixam de
ser possíveis quando os bancos se separam.

## Quem fica com o quê

| Tabela de hoje | Dono na Fase 4 | Banco |
| --- | --- | --- |
| `Cliente` | Identidade | `identidade-db` (SQL) |
| `Veiculo` | Identidade | `identidade-db` (SQL) |
| `Funcionario` | Identidade | `identidade-db` (SQL) |
| `Seguranca` | Identidade | `identidade-db` (SQL) |
| `OrdemServico` | Ordem de Serviço | `os-db` (SQL) |
| `Status` (histórico) | Ordem de Serviço | `os-db` (SQL) |
| `Orcamento` | Billing | `billing-db` (Mongo) |
| `Itens` | Execução | `execucao-db` (Mongo) |
| `Pecas` | Execução | `execucao-db` (Mongo) |
| `EmailOutbox` | substituída | vira `EventoOutbox` em cada serviço |

`Funcionario` fica com Identidade, e não com Execução, porque `Seguranca` guarda
o login e as duas tabelas não se separam. Execução passa a referenciar o
funcionário só pelo id, e recebe o nome dentro do evento quando precisa exibir.

## Os `JOIN` que quebram

Só dois lugares no código atual cruzam o que vai virar fronteira. Os dois fazem a
mesma coisa: montam a lista de itens de uma ordem com o nome da peça e o nome do
funcionário.

| Onde | O `JOIN` | O que acontece na Fase 4 |
| --- | --- | --- |
| `OrcamentoQuerys.cs:16-19` | `OrdemServico` → `Itens` → `Pecas`, `Funcionario` | deixa de existir: os itens chegam ao Billing dentro do evento |
| `OrdemServicoQuerys.cs:53-56` | o mesmo | deixa de existir: a OS não precisa dos itens para gerenciar status |

Nenhum dos dois vira chamada REST. Os dois somem, porque o dado passa a viajar no
evento no momento em que ele é produzido. Isso vale mais que a economia de uma
chamada: o Billing consegue gerar um orçamento mesmo com o Execução fora do ar.

## O que o mapeamento revelou sobre a saga

`Itens` pertence ao Execução, mas é de `Itens` que sai o valor do orçamento. Com
a saga escrita como "abrir a OS, gerar o orçamento", o Billing precisaria ler uma
tabela do Execução para saber quanto cobrar — exatamente o acesso que o enunciado
proíbe.

O erro não está na divisão, está na ordem dos passos. O fluxo real da oficina tem
o diagnóstico entre a abertura e o orçamento, e é no diagnóstico que os itens
nascem. A máquina de estados da Fase 2 já diz isso: `Recebida` → `Em Diagnóstico`
→ `Aguardando Aprovação`. A saga tinha pulado um passo que o domínio sempre teve.

Com o diagnóstico no lugar certo, o problema desaparece sozinho: o Execução
publica `DiagnosticoConcluido` **com os itens e os valores dentro**, e o Billing
monta o orçamento sem perguntar nada a ninguém.

```
1. OS aberta                  OS Service      -> OrdemAberta
2. Diagnóstico, itens lançados Execução       -> DiagnosticoConcluido (leva os itens)
3. Orçamento montado          Billing         -> OrcamentoGerado
4. Cliente aprova e paga      Billing         -> PagamentoConfirmado
5. Execução dos reparos       Execução        -> ExecucaoFinalizada
6. OS fechada                 OS Service
```

Contratos atualizados em `contratos/Eventos.cs`. O board no Miro ainda mostra a
versão de cinco passos e precisa ser corrigido.
