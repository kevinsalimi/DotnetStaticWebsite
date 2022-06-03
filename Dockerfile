#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80  
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["K1_Static_Website.csproj", ""]
RUN dotnet restore "./K1_Static_Website.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "K1_Static_Website.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "K1_Static_Website.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K1_Static_Website.dll"]