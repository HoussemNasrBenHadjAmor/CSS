# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY CSSPortal/CSSPortal.csproj CSSPortal/
RUN dotnet restore "CSSPortal/CSSPortal.csproj"

# Copy everything else and build
COPY CSSPortal/. CSSPortal/
WORKDIR /src/CSSPortal
RUN dotnet build "CSSPortal.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CSSPortal.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSSPortal.dll"]
