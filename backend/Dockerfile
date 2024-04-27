#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AnyRivals.Domain/AnyRivals.Domain.csproj", "AnyRivals.Domain/"]
COPY ["AnyRivals.Application/AnyRivals.Application.csproj", "AnyRivals.Application/"]
COPY ["AnyRivals.Infrastructure/AnyRivals.Infrastructure.csproj", "AnyRivals.Infrastructure/"]
COPY ["AnyRivals.Web/AnyRivals.Web.csproj", "AnyRivals.Web/"]
RUN dotnet restore "./AnyRivals.Web/./AnyRivals.Web.csproj"
COPY . .
WORKDIR "/src/AnyRivals.Web"
RUN dotnet build "./AnyRivals.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AnyRivals.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnyRivals.Web.dll"]