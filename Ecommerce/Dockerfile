# syntax=docker/dockerfile:1
# Create a stage for building the application.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /Ecommerce

# Copy the csproj and restore any dependencies (via nuget)
COPY ["Ecommerce.sln", "."]
COPY ["Ecommerce.WebAPI/Ecommerce.WebAPI.csproj", "Ecommerce.WebAPI/"]
COPY ["Ecommerce.Controller/Ecommerce.Controller.csproj", "Ecommerce.Controller/"]
COPY ["Ecommerce.Core/Ecommerce.Core.csproj", "Ecommerce.Core/"]
COPY ["Ecommerce.Service/Ecommerce.Service.csproj", "Ecommerce.Service/"]
COPY ["Ecommerce.Tests/Ecommerce.Tests.csproj", "Ecommerce.Tests/"]
RUN dotnet restore

# Copy the rest of the application and build it
COPY . .
WORKDIR /Ecommerce/Ecommerce.WebAPI
RUN dotnet publish -c Release -o ./publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /Ecommerce/Ecommerce.WebAPI
COPY --from=build /publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "Ecommerce.WebAPI.dll"]