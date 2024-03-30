#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Telegram.WebAPI/Telegram.WebAPI.csproj", "Telegram.WebAPI/"]
COPY ["Telegram.WebAPI.Application/Telegram.Application.csproj", "Telegram.WebAPI.Application/"]
COPY ["Telegram.WebAPI.Domain/Telegram.Domain.csproj", "Telegram.WebAPI.Domain/"]
COPY ["Telegram.WebAPI.Shared/Telegram.Shared.csproj", "Telegram.WebAPI.Shared/"]
COPY ["Telegram.WebAPI.IoC/Telegram.IoC.csproj", "Telegram.WebAPI.IoC/"]
COPY ["Telegram.WebAPI.Data/Telegram.Data.csproj", "Telegram.WebAPI.Data/"]
RUN dotnet restore "./Telegram.WebAPI/Telegram.WebAPI.csproj"
COPY . .
WORKDIR "/src/Telegram.WebAPI"
RUN dotnet build "./Telegram.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Telegram.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Telegram.WebAPI.dll"]