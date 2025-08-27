# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Csproj'u önce kopyala ve restore et (cache için iyi)
COPY WhetherForecast/WhetherForecast.csproj WhetherForecast/
RUN dotnet restore WhetherForecast/WhetherForecast.csproj

# Sonra tüm repo içeriğini kopyala ve publish et
COPY . .
RUN dotnet publish WhetherForecast/WhetherForecast.csproj -c Release -o /out /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet","WhetherForecast.dll"]
