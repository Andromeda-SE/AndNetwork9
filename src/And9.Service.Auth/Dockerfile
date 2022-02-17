#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/And9.Service.Auth/And9.Service.Auth.csproj", "src/And9.Service.Auth/"]
COPY ["src/And9.Service.Auth.Senders/And9.Service.Auth.Senders.csproj", "src/And9.Service.Auth.Senders/"]
COPY ["src/And9.Lib.Broker/And9.Lib.Broker.csproj", "src/And9.Lib.Broker/"]
COPY ["src/And9.Lib.Models.Abstractions/And9.Lib.Models.Abstractions.csproj", "src/And9.Lib.Models.Abstractions/"]
COPY ["src/And9.Service.Auth.Abstractions/And9.Service.Auth.Abstractions.csproj", "src/And9.Service.Auth.Abstractions/"]
COPY ["src/And9.Service.Core.Abstractions/And9.Service.Core.Abstractions.csproj", "src/And9.Service.Core.Abstractions/"]
COPY ["src/And9.Lib.API/And9.Lib.API.csproj", "src/And9.Lib.API/"]
COPY ["src/And9.Service.Auth.Database/And9.Service.Auth.Database.csproj", "src/And9.Service.Auth.Database/"]
COPY ["src/And9.Lib.Database/And9.Lib.Database.csproj", "src/And9.Lib.Database/"]
COPY ["src/And9.Service.Core.Database/And9.Service.Core.Database.csproj", "src/And9.Service.Core.Database/"]
RUN dotnet restore "src/And9.Service.Auth/And9.Service.Auth.csproj"
COPY . .
WORKDIR "/src/src/And9.Service.Auth"
RUN dotnet build "And9.Service.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "And9.Service.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "And9.Service.Auth.dll"]