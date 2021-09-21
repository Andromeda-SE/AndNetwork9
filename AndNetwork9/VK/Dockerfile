#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["AndNetwork9/VK/AndNetwork9.VK.csproj", "AndNetwork9/VK/"]
COPY ["AndNetwork9/Shared.Backend/AndNetwork9.Shared.Backend.csproj", "AndNetwork9/Shared.Backend/"]
COPY ["AndNetwork9/Shared/AndNetwork9.Shared.csproj", "AndNetwork9/Shared/"]
RUN dotnet restore "AndNetwork9/VK/AndNetwork9.VK.csproj"
COPY . .
WORKDIR "/src/AndNetwork9/VK"
RUN dotnet build "AndNetwork9.VK.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AndNetwork9.VK.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AndNetwork9.VK.dll"]