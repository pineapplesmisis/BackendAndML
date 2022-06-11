#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MCH.API/MCH.API.csproj", "MCH.API/"]
COPY ["MCH.Core/MCH.Core.csproj", "MCH.Core/"]
COPY ["MCH.Data/MCH.Data.csproj", "MCH.Data/"]
COPY ["MCH.Utils/MCH.Utils.csproj", "MCH.Utils/"]
RUN dotnet restore "MCH.API/MCH.API.csproj"
COPY . .
WORKDIR "/src/MCH.API"
RUN dotnet build "MCH.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MCH.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MCH.API.dll"]