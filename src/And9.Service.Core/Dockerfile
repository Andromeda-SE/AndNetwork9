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
COPY ["src/And9.Service.Core/And9.Service.Core.csproj", "src/And9.Service.Core/"]
COPY ["src/And9.Service.Core.Senders/And9.Service.Core.Senders.csproj", "src/And9.Service.Core.Senders/"]
COPY ["src/And9.Service.Core.Abstractions/And9.Service.Core.Abstractions.csproj", "src/And9.Service.Core.Abstractions/"]
COPY ["src/And9.Lib.Models.Abstractions/And9.Lib.Models.Abstractions.csproj", "src/And9.Lib.Models.Abstractions/"]
COPY ["src/And9.Lib.Broker/And9.Lib.Broker.csproj", "src/And9.Lib.Broker/"]
COPY ["src/And9.Lib.Utility/And9.Lib.Utility.csproj", "src/And9.Lib.Utility/"]
COPY ["src/And9.Service.Core.Database/And9.Service.Core.Database.csproj", "src/And9.Service.Core.Database/"]
COPY ["src/And9.Lib.Database/And9.Lib.Database.csproj", "src/And9.Lib.Database/"]
RUN dotnet restore "src/And9.Service.Core/And9.Service.Core.csproj"
COPY . .
WORKDIR "/src/src/And9.Service.Core"
RUN dotnet build "And9.Service.Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "And9.Service.Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "And9.Service.Core.dll"]