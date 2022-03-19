#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
RUN apk update
RUN apk add --no-cache curl
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/And9.Integration.VK/And9.Integration.VK.csproj", "src/And9.Integration.VK/"]
COPY ["src/And9.Integration.VK.Senders/And9.Integration.VK.Senders.csproj", "src/And9.Integration.VK.Senders/"]
COPY ["src/And9.Lib.Broker/And9.Lib.Broker.csproj", "src/And9.Lib.Broker/"]
COPY ["src/And9.Lib.Models.Abstractions/And9.Lib.Models.Abstractions.csproj", "src/And9.Lib.Models.Abstractions/"]
COPY ["src/And9.Lib.Utility/And9.Lib.Utility.csproj", "src/And9.Lib.Utility/"]
RUN dotnet restore "src/And9.Integration.VK/And9.Integration.VK.csproj"
COPY . .
WORKDIR "/src/src/And9.Integration.VK"
RUN dotnet build "And9.Integration.VK.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "And9.Integration.VK.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "And9.Integration.VK.dll"]