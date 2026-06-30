FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "Apptech.Assessment.slnx"

RUN dotnet publish "src/Apptech.Assessment.HttpApi.Host/Apptech.Assessment.HttpApi.Host.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Apptech.Assessment.HttpApi.Host.dll"]