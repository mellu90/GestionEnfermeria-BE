# Fase base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Fase de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos el csproj de GestionEnfermeria a la raíz de /src
COPY ["GestionEnfermeria.csproj", "./"]

# Restauramos dependencias
RUN dotnet restore "./GestionEnfermeria.csproj"

# Copiamos todo el resto del código
COPY . .

# Construimos y Publicamos
RUN dotnet publish "./GestionEnfermeria.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Fase final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GestionEnfermeria.dll"]