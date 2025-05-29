# Etap build (kompilacja aplikacji)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Etap runtime (uruchamianie)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5230
ENV ASPNETCORE_URLS=http://*:5230
ENTRYPOINT ["dotnet", "kryptografia.dll"]
