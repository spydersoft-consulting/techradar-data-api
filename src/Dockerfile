# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled as runtime
WORKDIR /app
COPY . ./
ENTRYPOINT ["dotnet", "Spydersoft.TechRadar.Api.dll"]
