FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AltiKarePDFStampler/AltiKarePDFStampler.csproj", "AltiKarePDFStampler/"]
RUN dotnet restore "AltiKarePDFStampler/AltiKarePDFStampler.csproj"
COPY . .
WORKDIR "/src/AltiKarePDFStampler"
RUN dotnet build "AltiKarePDFStampler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AltiKarePDFStampler.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AltiKarePDFStampler.dll"]
