#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/BBK.SaaS.Migrator/BBK.SaaS.Migrator.csproj", "src/BBK.SaaS.Migrator/"]
COPY ["src/BBK.SaaS.EntityFrameworkCore/BBK.SaaS.EntityFrameworkCore.csproj", "src/BBK.SaaS.EntityFrameworkCore/"]
COPY ["src/BBK.SaaS.Core/BBK.SaaS.Core.csproj", "src/BBK.SaaS.Core/"]
COPY ["src/BBK.SaaS.Core.Shared/BBK.SaaS.Core.Shared.csproj", "src/BBK.SaaS.Core.Shared/"]
RUN dotnet restore "src/BBK.SaaS.Migrator/BBK.SaaS.Migrator.csproj"
COPY . .
WORKDIR "/src/src/BBK.SaaS.Migrator"
RUN dotnet build "BBK.SaaS.Migrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BBK.SaaS.Migrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BBK.SaaS.Migrator.dll", "-s"]
