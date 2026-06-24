FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Bê toàn bộ code và file doanweb.sln vào
COPY . .

# Nấu code dựa trên file solution tổng
RUN dotnet restore "doanweb.sln"
RUN dotnet publish "doanweb.sln" -c Release -o /app/publish

# Đóng gói đem đi chạy
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "doanweb.dll"]
