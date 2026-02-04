FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PicpaySimplificado.csproj", "."]
RUN dotnet restore "PicpaySimplificado.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "PicpaySimplificado.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PicpaySimplificado.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PicpaySimplificado.dll"]
