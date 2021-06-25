FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app/

COPY ./src/Memories.Image.Ingestor.Lambda/*.csproj ./src/Memories.Image.Ingestor.Lambda/
COPY ./memories-image-ingestor-lambda.sln ./
RUN dotnet restore

COPY ./src/ ./src/
RUN dotnet publish ./src/Memories.Image.Ingestor.Lambda -c Release -o ./out/memories-image-ingestor-lambda
