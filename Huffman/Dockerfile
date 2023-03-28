FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Huffman/Huffman.csproj", "Huffman/"]
RUN dotnet restore "Huffman/Huffman.csproj"
COPY . .
WORKDIR "/src/Huffman"
RUN dotnet build "Huffman.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Huffman.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Huffman.dll"]
