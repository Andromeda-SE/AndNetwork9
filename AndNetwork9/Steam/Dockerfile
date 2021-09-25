#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["AndNetwork9/Steam/AndNetwork9.Steam.csproj", "AndNetwork9/Steam/"]
COPY ["AndNetwork9/Shared.Backend/AndNetwork9.Shared.Backend.csproj", "AndNetwork9/Shared.Backend/"]
COPY ["AndNetwork9/Shared/AndNetwork9.Shared.csproj", "AndNetwork9/Shared/"]
RUN dotnet restore "AndNetwork9/Steam/AndNetwork9.Steam.csproj"
COPY . .
WORKDIR "/src/AndNetwork9/Steam"
RUN dotnet build "AndNetwork9.Steam.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AndNetwork9.Steam.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AndNetwork9.Steam.dll"]