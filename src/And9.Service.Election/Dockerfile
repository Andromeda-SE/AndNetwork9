#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
RUN apk update
RUN apk add --no-cache icu-libs
RUN apk add --no-cache tzdata
RUN apk add --no-cache curl
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/And9.Service.Election/And9.Service.Election.csproj", "src/And9.Service.Election/"]
COPY ["src/And9.Service.Core.Senders/And9.Service.Core.Senders.csproj", "src/And9.Service.Core.Senders/"]
COPY ["src/And9.Service.Core.Abstractions/And9.Service.Core.Abstractions.csproj", "src/And9.Service.Core.Abstractions/"]
COPY ["src/And9.Lib.Models.Abstractions/And9.Lib.Models.Abstractions.csproj", "src/And9.Lib.Models.Abstractions/"]
COPY ["src/And9.Lib.Formatters/And9.Lib.Formatters.csproj", "src/And9.Lib.Formatters/"]
COPY ["src/And9.Lib.Broker/And9.Lib.Broker.csproj", "src/And9.Lib.Broker/"]
COPY ["src/And9.Lib.Utility/And9.Lib.Utility.csproj", "src/And9.Lib.Utility/"]
COPY ["src/And9.Service.Election.Senders/And9.Service.Election.Senders.csproj", "src/And9.Service.Election.Senders/"]
COPY ["src/And9.Service.Election.Abstractions/And9.Service.Election.Abstractions.csproj", "src/And9.Service.Election.Abstractions/"]
COPY ["src/And9.Integration.Discord.Senders/And9.Integration.Discord.Senders.csproj", "src/And9.Integration.Discord.Senders/"]
COPY ["src/And9.Integration.Discord.Abstractions/And9.Integration.Discord.Abstractions.csproj", "src/And9.Integration.Discord.Abstractions/"]
COPY ["src/And9.Service.Auth.Abstractions/And9.Service.Auth.Abstractions.csproj", "src/And9.Service.Auth.Abstractions/"]
COPY ["src/And9.Service.Election.Database/And9.Service.Election.Database.csproj", "src/And9.Service.Election.Database/"]
COPY ["src/And9.Lib.Database/And9.Lib.Database.csproj", "src/And9.Lib.Database/"]
RUN dotnet restore "src/And9.Service.Election/And9.Service.Election.csproj"
COPY . .
WORKDIR "/src/src/And9.Service.Election"
RUN dotnet build "And9.Service.Election.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "And9.Service.Election.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "And9.Service.Election.dll"]