FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "Apptech.Assessment.slnx"

RUN dotnet publish "src/Apptech.Assessment.DbMigrator/Apptech.Assessment.DbMigrator.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Apptech.Assessment.DbMigrator.dll"]