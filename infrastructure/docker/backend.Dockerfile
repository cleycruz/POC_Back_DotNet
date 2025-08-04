FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CarritoComprasAPI.csproj", "."]
RUN dotnet restore "./CarritoComprasAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CarritoComprasAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarritoComprasAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarritoComprasAPI.dll"]
