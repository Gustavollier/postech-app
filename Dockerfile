# Esses ARGs permitem a troca da base usada para criar a imagem final durante a depuração do VS
ARG LAUNCHING_FROM_VS
ARG FINAL_BASE_IMAGE=${LAUNCHING_FROM_VS:+aotdebug}

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PosTechChallenge/PosTechChallenge.csproj", "PosTechChallenge/"]
RUN dotnet restore "./PosTechChallenge/PosTechChallenge.csproj"
COPY . .
WORKDIR "/src/PosTechChallenge"
RUN dotnet build "./PosTechChallenge.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PosTechChallenge.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Esta fase é usada como base para a fase final ao iniciar no VS para dar suporte à depuração no modo normal
FROM base AS aotdebug
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    gdb
USER app

# CORREÇÃO: imagem final alterada de runtime-deps para aspnet:10.0
# runtime-deps só contém dependências nativas do SO (para apps AOT).
# Como PublishAot=false no .csproj, a app depende do runtime .NET,
# que está presente apenas na imagem aspnet:10.0.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "PosTechChallenge.dll"]
