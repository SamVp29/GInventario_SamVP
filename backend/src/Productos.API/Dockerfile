FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Productos.API/Productos.API.csproj src/Productos.API/
RUN dotnet restore src/Productos.API/Productos.API.csproj

COPY src/Productos.API/ src/Productos.API/
WORKDIR /src/src/Productos.API
RUN dotnet build Productos.API.csproj -c Release -o /app/build
RUN dotnet publish Productos.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Productos.API.dll"]
