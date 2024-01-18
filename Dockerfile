# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled as runtime
WORKDIR /app
COPY . ./app

# Serve on port 8080, we cannot serve on port 80 with a custom user that is not root.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Spydersoft.TechRadar.Data.Api.dll"]
