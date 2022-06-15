FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CatalogItems.API.csproj", "./"]
RUN dotnet restore "CatalogItems.API.csproj"
COPY . .
RUN dotnet publish "CatalogItems.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogItems.API.dll"]