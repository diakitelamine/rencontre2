# Stage 1: Build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0.202 AS build-env
WORKDIR /app
EXPOSE 8080

# Copy everything and restore as distinct layers
COPY . .

# Run dotnet restore and publish
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy the published output from build environment
COPY --from=build-env /app/out .

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "API.dll"]
