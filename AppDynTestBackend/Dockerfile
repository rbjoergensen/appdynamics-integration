#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["AppDynTestBackend.csproj", "AppDynTestBackend/AppDynTestBackend.csproj"]
RUN dotnet restore "AppDynTestBackend/AppDynTestBackend.csproj"
WORKDIR "/src/AppDynTestBackend"
COPY . .
RUN dotnet build "AppDynTestBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppDynTestBackend.csproj" -c Release -o /app/publish

FROM docker.io/appdynamics/dotnet-core-agent:20.11.0-linux AS appdynamics

FROM base AS final
RUN apt update -yqqq; apt install -yqqq iputils-ping curl
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=appdynamics /opt/appdynamics /opt/temp
COPY AppDynamicsConfig.json /opt/temp/AppDynamicsConfig.json
COPY Data/my-root.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "AppDynTestBackend.dll"]
