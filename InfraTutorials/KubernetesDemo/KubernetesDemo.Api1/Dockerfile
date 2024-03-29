#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["KubernetesDemo.Api1/KubernetesDemo.Api1.csproj", "KubernetesDemo.Api1/"]
RUN dotnet restore "./KubernetesDemo.Api1/./KubernetesDemo.Api1.csproj"
COPY . .
WORKDIR "/src/KubernetesDemo.Api1"
RUN dotnet build "./KubernetesDemo.Api1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KubernetesDemo.Api1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KubernetesDemo.Api1.dll"]