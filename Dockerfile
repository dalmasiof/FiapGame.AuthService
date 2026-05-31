# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia csproj para cache de restore
COPY ["src/1-Auth.Api/1-Auth.Api.csproj", "src/1-Auth.Api/"]
COPY ["src/2-Auth.Application/2-Auth.Application.csproj", "src/2-Auth.Application/"]
COPY ["src/3-Auth.Infrastructure/3-Auth.Infrastructure.csproj", "src/3-Auth.Infrastructure/"]
COPY ["src/4-Auth.Domain/4-Auth.Domain.csproj", "src/4-Auth.Domain/"]

RUN dotnet restore "src/1-Auth.Api/1-Auth.Api.csproj"

# Copia o restante
COPY . .

WORKDIR "/src/src/1-Auth.Api"

RUN dotnet publish "1-Auth.Api.csproj" \
    -c Release \
    -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8081
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "1-Auth.Api.dll"]