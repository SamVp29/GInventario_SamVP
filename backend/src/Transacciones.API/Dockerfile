FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Transacciones.API/Transacciones.API.csproj src/Transacciones.API/
RUN dotnet restore src/Transacciones.API/Transacciones.API.csproj

COPY src/Transacciones.API/ src/Transacciones.API/
WORKDIR /src/src/Transacciones.API
RUN dotnet build Transacciones.API.csproj -c Release -o /app/build
RUN dotnet publish Transacciones.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Transacciones.API.dll"]
