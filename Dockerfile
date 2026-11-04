FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["OrganizaDinDin.csproj", "./"]
RUN dotnet restore "OrganizaDinDin.csproj"

COPY . .
RUN dotnet publish "OrganizaDinDin.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production

CMD ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet OrganizaDinDin.dll
