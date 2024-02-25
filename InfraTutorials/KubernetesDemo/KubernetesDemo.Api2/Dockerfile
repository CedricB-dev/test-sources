#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["KubernetesDemo.Api2/KubernetesDemo.Api2.csproj", "KubernetesDemo.Api2/"]
RUN dotnet restore "./KubernetesDemo.Api2/./KubernetesDemo.Api2.csproj"
COPY . .
WORKDIR "/src/KubernetesDemo.Api2"
RUN dotnet build "./KubernetesDemo.Api2.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KubernetesDemo.Api2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KubernetesDemo.Api2.dll"]