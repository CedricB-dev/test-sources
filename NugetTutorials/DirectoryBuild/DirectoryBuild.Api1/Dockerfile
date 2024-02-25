FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["DirectoryBuild.Lib/DirectoryBuild.Lib.csproj", "DirectoryBuild.Lib/"]
COPY ["DirectoryBuild.Api1/DirectoryBuild.Api1.csproj", "DirectoryBuild.Api1/"]
RUN dotnet restore "DirectoryBuild.Api1/DirectoryBuild.Api1.csproj"
COPY . .
WORKDIR "/src/DirectoryBuild.Api1"
RUN dotnet build "DirectoryBuild.Api1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DirectoryBuild.Api1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DirectoryBuild.Api1.dll"]
