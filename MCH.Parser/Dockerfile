#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MCH.Parser/MCH.Parser.csproj", "MCH.Parser/"]
COPY ["MCH.Data/MCH.Data.csproj", "MCH.Data/"]
COPY ["MCH.Utils/MCH.Utils.csproj", "MCH.Utils/"]
RUN dotnet restore "MCH.Parser/MCH.Parser.csproj"
COPY . .
WORKDIR "/src/MCH.Parser"
RUN dotnet build "MCH.Parser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MCH.Parser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MCH.Parser.dll"]