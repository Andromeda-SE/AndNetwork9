#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/And9.Gateway.Clan/And9.Gateway.Clan.csproj", "src/And9.Gateway.Clan/"]
COPY ["src/And9.Lib.Utility/And9.Lib.Utility.csproj", "src/And9.Lib.Utility/"]
COPY ["src/And9.Service.Auth.Senders/And9.Service.Auth.Senders.csproj", "src/And9.Service.Auth.Senders/"]
COPY ["src/And9.Lib.Broker/And9.Lib.Broker.csproj", "src/And9.Lib.Broker/"]
COPY ["src/And9.Lib.Models.Abstractions/And9.Lib.Models.Abstractions.csproj", "src/And9.Lib.Models.Abstractions/"]
COPY ["src/And9.Service.Auth.Abstractions/And9.Service.Auth.Abstractions.csproj", "src/And9.Service.Auth.Abstractions/"]
COPY ["src/And9.Service.Core.Abstractions/And9.Service.Core.Abstractions.csproj", "src/And9.Service.Core.Abstractions/"]
COPY ["src/And9.Gateway.Clan.Client/And9.Gateway.Clan.Client.csproj", "src/And9.Gateway.Clan.Client/"]
COPY ["src/And9.Integration.Discord.API/And9.Integration.Discord.API.csproj", "src/And9.Integration.Discord.API/"]
COPY ["src/And9.Service.Auth.API/And9.Service.Auth.API.csproj", "src/And9.Service.Auth.API/"]
COPY ["src/And9.Lib.API/And9.Lib.API.csproj", "src/And9.Lib.API/"]
COPY ["src/And9.Integration.Discord.Senders/And9.Integration.Discord.Senders.csproj", "src/And9.Integration.Discord.Senders/"]
COPY ["src/And9.Integration.Discord.Abstractions/And9.Integration.Discord.Abstractions.csproj", "src/And9.Integration.Discord.Abstractions/"]
RUN dotnet restore "src/And9.Gateway.Clan/And9.Gateway.Clan.csproj"
COPY . .
WORKDIR "/src/src/And9.Gateway.Clan"
RUN dotnet build "And9.Gateway.Clan.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "And9.Gateway.Clan.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "And9.Gateway.Clan.dll"]