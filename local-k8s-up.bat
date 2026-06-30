@echo off
setlocal EnableDelayedExpansion
chcp 65001 >nul

echo.
echo ============================================================
echo   PosTechChallenge - Subir ambiente K8s local
echo   Kubernetes: Docker Desktop
echo ============================================================
echo.

:: ── Verifica pré-requisitos ──────────────────────────────────
where kubectl >nul 2>&1 || (echo [ERRO] kubectl nao encontrado. Instale o Docker Desktop com Kubernetes habilitado. & pause & exit /b 1)
where docker  >nul 2>&1 || (echo [ERRO] docker nao encontrado. & pause & exit /b 1)

:: Verifica se o cluster docker-desktop esta acessivel
kubectl cluster-info --context docker-desktop >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Kubernetes do Docker Desktop nao esta acessivel.
    echo        Ative em: Docker Desktop ^> Settings ^> Kubernetes ^> Enable Kubernetes
    pause & exit /b 1
)
echo [OK] Cluster docker-desktop acessivel.

:: ── Le o .env ────────────────────────────────────────────────
if not exist ".env" (
    echo [ERRO] Arquivo .env nao encontrado na raiz do projeto.
    pause & exit /b 1
)

set MSSQL_SA_PASSWORD=
set APP_DB_PASSWORD=
set JWT_SECRET_KEY=

for /f "usebackq tokens=1,* delims==" %%A in (".env") do (
    set "LINE=%%A"
    if "!LINE:~0,1!" neq "#" (
        if "%%A"=="MSSQL_SA_PASSWORD" set "MSSQL_SA_PASSWORD=%%B"
        if "%%A"=="APP_DB_PASSWORD"   set "APP_DB_PASSWORD=%%B"
        if "%%A"=="JWT_SECRET_KEY"    set "JWT_SECRET_KEY=%%B"
    )
)

if "!MSSQL_SA_PASSWORD!"=="" (echo [ERRO] MSSQL_SA_PASSWORD nao definido no .env & pause & exit /b 1)
if "!APP_DB_PASSWORD!"==""   (echo [ERRO] APP_DB_PASSWORD nao definido no .env   & pause & exit /b 1)
if "!JWT_SECRET_KEY!"==""    (echo [ERRO] JWT_SECRET_KEY nao definido no .env    & pause & exit /b 1)

echo [OK] Variaveis carregadas do .env.

:: ── Build da imagem ──────────────────────────────────────────
echo.
echo [1/6] Build da imagem Docker...
docker build -t postechallenge-api:local . --quiet
if %errorlevel% neq 0 (echo [ERRO] docker build falhou. & pause & exit /b 1)
echo [OK] Imagem postechallenge-api:local criada.

:: ── Namespace ────────────────────────────────────────────────
echo.
echo [2/6] Criando namespace...
kubectl create namespace postechallenge --context docker-desktop --dry-run=client -o yaml | kubectl apply -f - --context docker-desktop
echo [OK] Namespace postechallenge pronto.

:: ── Secrets ──────────────────────────────────────────────────
echo.
echo [3/6] Criando Secrets...
set "CONN=Server=sqlserver,1433;Database=PosTechChallenge;User Id=appchat;Password=!APP_DB_PASSWORD!;TrustServerCertificate=True;"

kubectl create secret generic postechallenge-secrets ^
  --namespace=postechallenge ^
  --context=docker-desktop ^
  --from-literal=MSSQL_SA_PASSWORD=!MSSQL_SA_PASSWORD! ^
  --from-literal=APP_DB_PASSWORD=!APP_DB_PASSWORD! ^
  --from-literal=JWT_SECRET_KEY=!JWT_SECRET_KEY! ^
  "--from-literal=ConnectionStrings__DefaultConnection=!CONN!" ^
  --dry-run=client -o yaml | kubectl apply -f - --context docker-desktop

if %errorlevel% neq 0 (echo [ERRO] Falha ao criar secrets. & pause & exit /b 1)
echo [OK] Secrets criados.

:: ── ConfigMap SQL ─────────────────────────────────────────────
echo.
echo [4/6] Criando ConfigMap do script SQL...
kubectl create configmap sql-init-script ^
  --namespace=postechallenge ^
  --context=docker-desktop ^
  --from-file=init.sql=infra/sql/init.sql ^
  --dry-run=client -o yaml | kubectl apply -f - --context docker-desktop

if %errorlevel% neq 0 (echo [ERRO] Falha ao criar ConfigMap SQL. & pause & exit /b 1)
echo [OK] ConfigMap sql-init-script criado.

:: ── Kustomize overlay local ───────────────────────────────────
echo.
echo [5/6] Aplicando manifests K8s (kustomize overlay local)...
kubectl apply -k k8s/overlays/local --context docker-desktop
if %errorlevel% neq 0 (echo [ERRO] kubectl apply -k falhou. & pause & exit /b 1)
echo [OK] Manifests aplicados.

:: ── Job de init do banco ──────────────────────────────────────
echo.
echo [6/6] Aguardando SQL Server ficar pronto (pode levar 1-2 min)...
kubectl rollout status statefulset/sqlserver -n postechallenge --context docker-desktop --timeout=180s
if %errorlevel% neq 0 (
    echo [AVISO] SQL Server ainda inicializando, aguarde mais um pouco...
)

echo Rodando job de inicializacao do banco...
kubectl delete job sqlserver-init --namespace=postechallenge --context=docker-desktop --ignore-not-found=true >nul 2>&1
kubectl apply -f k8s/base/sqlserver/init-job.yaml --context docker-desktop
kubectl wait job/sqlserver-init --namespace=postechallenge --context=docker-desktop --for=condition=complete --timeout=120s
if %errorlevel% neq 0 (
    echo [AVISO] Job de init pode ainda estar rodando. Verifique com: kubectl logs job/sqlserver-init -n postechallenge
)

:: ── Resumo ────────────────────────────────────────────────────
echo.
echo ============================================================
echo   Ambiente pronto!
echo.
echo   Pods:
kubectl get pods -n postechallenge --context docker-desktop
echo.
echo   Para acessar a API:
echo   kubectl port-forward svc/postechallenge-api 8080:80 -n postechallenge --context docker-desktop
echo.
echo   Swagger: http://localhost:8080/swagger
echo   (rode o port-forward acima em outro terminal)
echo ============================================================
echo.
pause
