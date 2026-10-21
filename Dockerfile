# 1. Utiliza el SDK de .NET 10.0 para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia todo el código fuente e inicia la restauración y publicación
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# 2. Utiliza la imagen de ejecución (runtime) de .NET 10.0
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Configura el puerto en el que escuchará Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProyectoWebFutbol.dll"]
