FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build

COPY ["src/Hinario.Api/Hinario.API.csproj", "src/Hinario.Api/"]
COPY ["src/Hinario.Core/Hinario.Core.csproj", "src/Hinario.Core/"]

RUN dotnet restore "src/Hinario.Api/Hinario.API.csproj"
COPY . .
WORKDIR /build/src/Hinario.Api

FROM build AS publish
RUN dotnet publish "Hinario.API.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS app
WORKDIR /app

COPY --from=publish /publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Hinario.API.dll"]