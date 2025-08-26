# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Tüm dosyaları kopyala ve restore/publish yap
COPY . .
# AppHost kapalı: her platformda taşınabilir DLL üretir
RUN dotnet publish -c Release -o /out /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Konteyner içi port
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Sadece **uygulama DLL'ini** başlat
ENTRYPOINT ["dotnet","WhetherForecast.dll"]

