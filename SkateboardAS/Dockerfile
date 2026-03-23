FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SkateboardAS/SkateboardAS.csproj", "SkateboardAS/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Orchestration/Orchestration.csproj", "Orchestration/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "SkateboardAS/SkateboardAS.csproj"
COPY . .
WORKDIR "/src/SkateboardAS"
RUN dotnet build "SkateboardAS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SkateboardAS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SkateboardAS.dll"]
