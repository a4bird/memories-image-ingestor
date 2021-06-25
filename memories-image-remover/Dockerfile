FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app/

COPY ./src/Memories.Image.Remover.Lambda/*.csproj ./src/Memories.Image.Remover.Lambda/
COPY ./memories-image-remover-lambda.sln ./
RUN dotnet restore

COPY ./src/ ./src/
RUN dotnet publish ./src/Memories.Image.Remover.Lambda -c Release -o ./out/memories-image-remover-lambda
