FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Huffman.Bench/Huffman.Bench.csproj", "Huffman.Bench/"]
RUN dotnet restore "Huffman.Bench/Huffman.Bench.csproj"
COPY . .
WORKDIR "/src/Huffman.Bench"
RUN dotnet build "Huffman.Bench.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Huffman.Bench.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Huffman.Bench.dll"]
