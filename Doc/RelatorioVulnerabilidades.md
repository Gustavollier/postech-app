# Relatorio de Vulnerabilidades - OWASP Top 10

Projeto: PosTechChallenge - Sistema de Gestao de Oficina Mecanica

Data da analise: 2026-05-03

Referencia: OWASP Top 10:2021 - https://owasp.org/Top10/

## Escopo

Foram analisados:

- API ASP.NET Core em `PosTechChallenge`
- Camada de aplicacao em `PosTechChallenge.Applicacao`
- Camada de infraestrutura em `PosTechChallenge.Infraestrutura`
- Configuracoes Docker, appsettings e bootstrap SQL
- Testes de integracao relacionados a autenticacao e autorizacao

## Scans executados

| Ferramenta | Comando | Resultado |
|---|---|---|
| NuGet vulnerable scan | `dotnet list <projeto> package --vulnerable --include-transitive` | Nenhum pacote vulneravel encontrado em API, Aplicacao, Infraestrutura e Testes |
| NuGet outdated scan | `dotnet list PosTechChallenge.sln package --outdated` | Existem pacotes desatualizados, mas sem vulnerabilidade reportada pelo scan atual |
| Secret scan local | `rg` por `Password=`, `Jwt__SecretKey`, `SecretKey`, `MSSQL_SA_PASSWORD` | Segredos hardcoded encontrados e removidos de appsettings, launchSettings e docker-compose |
| Semgrep | `semgrep scan --config auto` | Nao executado: ferramenta nao instalada no ambiente |
| Gitleaks | `gitleaks detect --source . --no-banner` | Nao executado: ferramenta nao instalada no ambiente |

## Resumo executivo

Foram corrigidos os riscos mais relevantes para o MVP:

- Autenticacao JWT passou a ser obrigatoria por padrao.
- `POST /api/v1/autenticacao/login` permanece publico.
- `GET /api/v1/ordens-servico/{id}/status` permanece publico por requisito de acompanhamento pelo cliente.
- O endpoint legado `POST /api/v1/autenticacao/criar-senha` foi removido; senha inicial e criada no cadastro de funcionario.
- Segredos foram removidos de arquivos versionados e movidos para variaveis de ambiente.
- A imagem final Docker passou a executar com usuario nao-root.
- Headers basicos de seguranca foram adicionados nas respostas HTTP.
- A alteracao de senha exige JWT, senha atual, nova senha e confirmacao.

## Mapeamento OWASP Top 10

| Categoria | Analise | Status |
|---|---|---|
| A01 Broken Access Control | Havia varios endpoints sem `[Authorize]`. Foi criada politica global que exige usuario autenticado e excecoes publicas explicitas. Por decisao de testes do MVP, o cadastro de funcionario ficou publico. | Parcial |
| A02 Cryptographic Failures | Senhas usam BCrypt, ponto positivo. A chave JWT estava em arquivos versionados/compose. Foi movida para `JWT_SECRET_KEY` e validada com minimo de 32 bytes. | Corrigido |
| A03 Injection | Repositorios Dapper usam parametros nomeados (`@Id`, `@CPF`, etc.), sem concatenacao direta de SQL observada. | Sem achado critico |
| A04 Insecure Design | Fluxo separado de criacao de senha foi removido. A senha inicial nasce no cadastro de funcionario, e alteracao posterior exige senha atual. | Corrigido |
| A05 Security Misconfiguration | `docker-compose` subia API como `Development`, expondo Swagger por padrao. Agora o default e `Production`, com override por `.env` quando necessario. Headers basicos foram adicionados. | Corrigido |
| A06 Vulnerable and Outdated Components | Scan NuGet nao encontrou pacotes vulneraveis. O scan de outdated encontrou atualizacoes disponiveis; recomenda-se atualizar em sprint propria com teste de regressao. | Monitorado |
| A07 Identification and Authentication Failures | Login usa JWT com issuer/audience/lifetime e BCrypt. O endpoint separado de criacao de senha foi removido; alteracao de senha exige JWT e senha atual. | Corrigido |
| A08 Software and Data Integrity Failures | Docker agora usa variaveis externas para segredos. Ainda nao ha assinatura/verificacao de imagens ou pipeline com SAST obrigatorio. | Parcial |
| A09 Security Logging and Monitoring Failures | Existe monitoramento de tempo medio. Mensagens de excecao em autenticacao deixaram de expor detalhes internos ao cliente. Ainda falta trilha de auditoria de login/redefinicao de senha. | Parcial |
| A10 Server-Side Request Forgery | Nao foram encontrados fluxos que recebam URL remota do usuario e facam requisicoes server-side. | Nao aplicavel no MVP |

## Evidencias de correcao

Arquivos alterados:

- `PosTechChallenge/Program.cs`
- `PosTechChallenge/Controllers/AutenticacaoController.cs`
- `PosTechChallenge/Controllers/OrdemServicoController.cs`
- `PosTechChallenge.Applicacao/UseCases/Autenticacao/LoginUseCase.cs`
- `PosTechChallenge.Applicacao/UseCases/Autenticacao/AlterarSenhaUseCase.cs`
- `PosTechChallenge.Infraestrutura/Repositorios/SegurancaRepositorio.cs`
- `infra/sql/init.sql`
- `docker-compose.yml`
- `PosTechChallenge/Dockerfile`
- `PosTechChallenge/appsettings.json`
- `PosTechChallenge/appsettings.Development.json`
- `PosTechChallenge/Properties/launchSettings.json`
- `.env.example`
- `.gitignore`
- testes de integracao em `PosTechChallenge.Testes/Integration`

## Validacao

Resultado dos testes:

```text
dotnet test PosTechChallenge.Testes/PosTechChallenge.Testes.csproj --no-restore
Aprovado: 191, Falha: 0, Ignorado: 0
```

Resultado do scan de dependencias:

```text
dotnet list <projeto> package --vulnerable --include-transitive
Nenhum pacote vulneravel encontrado em API, Aplicacao, Infraestrutura e Testes.
```

## Riscos residuais e proximos passos

- Instalar e executar `gitleaks` no pipeline para barrar novos segredos.
- Instalar e executar `semgrep` ou CodeQL no pipeline para SAST.
- Rodar OWASP ZAP baseline contra a API em ambiente local ou homologacao.
- Avaliar atualizacao dos pacotes desatualizados apontados pelo NuGet.
- Criar auditoria para login, redefinicao de senha e alteracoes administrativas.
- Considerar separacao entre fluxo de primeira senha e fluxo de redefinicao de senha com token de convite ou aprovacao administrativa.
