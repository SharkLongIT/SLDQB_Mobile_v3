#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/BBK.SaaS.Web.Public/BBK.SaaS.Web.Public.csproj", "src/BBK.SaaS.Web.Public/"]
COPY ["src/BBK.SaaS.Web.Core/BBK.SaaS.Web.Core.csproj", "src/BBK.SaaS.Web.Core/"]
COPY ["src/BBK.SaaS.Application/BBK.SaaS.Application.csproj", "src/BBK.SaaS.Application/"]
COPY ["src/BBK.SaaS.Application.Shared/BBK.SaaS.Application.Shared.csproj", "src/BBK.SaaS.Application.Shared/"]
COPY ["src/BBK.SaaS.Core.Shared/BBK.SaaS.Core.Shared.csproj", "src/BBK.SaaS.Core.Shared/"]
COPY ["src/BBK.SaaS.Core/BBK.SaaS.Core.csproj", "src/BBK.SaaS.Core/"]
COPY ["src/BBK.SaaS.EntityFrameworkCore/BBK.SaaS.EntityFrameworkCore.csproj", "src/BBK.SaaS.EntityFrameworkCore/"]
COPY ["src/BBK.SaaS.GraphQL/BBK.SaaS.GraphQL.csproj", "src/BBK.SaaS.GraphQL/"]
RUN dotnet restore "src/BBK.SaaS.Web.Public/BBK.SaaS.Web.Public.csproj"
COPY . .
WORKDIR "/src/src/BBK.SaaS.Web.Public"
RUN dotnet build "BBK.SaaS.Web.Public.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BBK.SaaS.Web.Public.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BBK.SaaS.Web.Public.dll"]
