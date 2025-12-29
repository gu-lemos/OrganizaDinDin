# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar o arquivo .csproj e restaurar dependências
COPY ["OrganizaDinDin.csproj", "./"]
RUN dotnet restore "OrganizaDinDin.csproj"

# Copiar todo o código fonte e fazer o build
COPY . .
RUN dotnet build "OrganizaDinDin.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "OrganizaDinDin.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copiar os arquivos publicados
COPY --from=publish /app/publish .

# Expor a porta que a aplicação vai usar
EXPOSE 8080

# Configurar variáveis de ambiente
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando para iniciar a aplicação
# O Render irá definir a variável PORT automaticamente
CMD ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet OrganizaDinDin.dll
