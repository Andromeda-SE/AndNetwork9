#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/And9.Service.Award/And9.Service.Award.csproj", "src/And9.Service.Award/"]
COPY ["src/And9.Service.Award.Database/And9.Service.Award.Database.csproj", "src/And9.Service.Award.Database/"]
COPY ["src/And9.Service.Award.Abstractions/And9.Service.Award.Abstractions.csproj", "src/And9.Service.Award.Abstractions/"]
COPY ["src/And9.Lib.Models.Abstractions/And9.Lib.Models.Abstractions.csproj", "src/And9.Lib.Models.Abstractions/"]
COPY ["src/And9.Lib.Database/And9.Lib.Database.csproj", "src/And9.Lib.Database/"]
COPY ["src/And9.Service.Core.Senders/And9.Service.Core.Senders.csproj", "src/And9.Service.Core.Senders/"]
COPY ["src/And9.Service.Core.Abstractions/And9.Service.Core.Abstractions.csproj", "src/And9.Service.Core.Abstractions/"]
COPY ["src/And9.Lib.Formatters/And9.Lib.Formatters.csproj", "src/And9.Lib.Formatters/"]
COPY ["src/And9.Lib.Broker/And9.Lib.Broker.csproj", "src/And9.Lib.Broker/"]
COPY ["src/And9.Lib.Utility/And9.Lib.Utility.csproj", "src/And9.Lib.Utility/"]
COPY ["src/And9.Service.Award.Senders/And9.Service.Award.Senders.csproj", "src/And9.Service.Award.Senders/"]
COPY ["src/And9.Integration.Steam.Senders/And9.Integration.Steam.Senders.csproj", "src/And9.Integration.Steam.Senders/"]
RUN dotnet restore "src/And9.Service.Award/And9.Service.Award.csproj"
COPY . .
WORKDIR "/src/src/And9.Service.Award"
RUN dotnet build "And9.Service.Award.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "And9.Service.Award.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "And9.Service.Award.dll"]