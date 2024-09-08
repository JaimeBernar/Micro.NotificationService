FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY Micro.NotificationService.Common Micro.NotificationService.Common
COPY Micro.NotificationService Micro.NotificationService

RUN dotnet restore Micro.NotificationService

RUN dotnet build Micro.NotificationService -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish Micro.NotificationService -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Micro.NotificationService.dll"]