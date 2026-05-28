# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./SupervisorMobility.Client/SupervisorMobility.Client.csproj
RUN dotnet publish ./SupervisorMobility.Client/SupervisorMobility.Client.csproj -c Release -o /app/publish

# Etapa de runtime con Nginx
FROM nginx:alpine AS runtime
COPY ./nginx/default.conf /etc/nginx/conf.d/default.conf
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish .
