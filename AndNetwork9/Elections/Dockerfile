#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-preview.7-alpine3.13 AS base
RUN apk update
RUN apk add --no-cache icu-libs
RUN apk add --no-cache tzdata
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN cp /usr/share/zoneinfo/Etc/UTC /etc/localtime
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0.100-preview.7-alpine3.13 AS build
WORKDIR /src
COPY ["AndNetwork9/Elections/AndNetwork9.Elections.csproj", "AndNetwork9/Elections/"]
COPY ["AndNetwork9/Shared/AndNetwork9.Shared.csproj", "AndNetwork9/Shared/"]
COPY ["AndNetwork9/Shared.Backend/AndNetwork9.Shared.Backend.csproj", "AndNetwork9/Shared.Backend/"]
RUN dotnet restore "AndNetwork9/Elections/AndNetwork9.Elections.csproj"
COPY . .
WORKDIR "/src/AndNetwork9/Elections"
RUN dotnet build "AndNetwork9.Elections.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AndNetwork9.Elections.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AndNetwork9.Elections.dll"]