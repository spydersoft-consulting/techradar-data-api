# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy as runtime
WORKDIR /app
COPY . /app

# Create a group and user so we are not running our container and application as root and thus user 0 which is a security issue.
RUN addgroup --system --gid 1000 netcoregroup \
    && adduser --system --uid 1000 --ingroup netcoregroup --shell /bin/sh netcoreuser

# Serve on port 8080, we cannot serve on port 80 with a custom user that is not root.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER 1000

ENTRYPOINT ["dotnet", "Spydersoft.TechRadar.Data.Api.dll"]
