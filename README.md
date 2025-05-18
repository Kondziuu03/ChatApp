# ChatApp API - AI powered

A real-time chat application initially developed to learn about new technologies as SignalR, Kafka, Serilog. Currently it's being developed with new AI functionalities, to check how the SemanticKernel, Ollama connector and LLMs integration works.

For testings purposes was prepared a simple frontend in React with help of AI tools - https://github.com/Kondziuu03/ChatApp-frontend.

## Features
- Real-time chat functionality
- User authorization and authentication by JWT tokens
- One global chat for all users
- Kafka integration to listening for messages and saving to DB
- SemanticKernel with Ollama platform integration to provide new functionalities (for now user can use LLM to paraphrase or check grammar of typed message)

## To do
- Provide RAG functionality using Qdrant vector store and user roles
- Support multiple chats
- Dockerize app and keep integration with other components

## Technologies
- SignalR
- Serilog, Seq
- Kafka
- Entity Framework, MS SQL server
- SemanticKernel, Ollama connector

# Setup
Unfortunately, there is currently no easy way to run the application, as it's still in development. To make it work few extra steps need to be done.

## Fill appsettings with needed configuration, apply migrations to db
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      },
      {
        "Name": "Console"
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "ChatApp"
    }
  },
  "JwtSettingsOption": {
    "SecretKey": "",
    "ExpiryInMinutes": 60
  },
  "Origin": "http://localhost:3000/",
  "KafkaOption": {
    "Url": "localhost:9092"
  },
  "ConnectionString": "",
  "Ollama": {
    "ModelId": "llama3.1:8b",
    "Endpoint": "http://localhost:11434/v1"
  }
}

```

## Run Kafka and Zookeeper containers
```bash
docker compose -f kafka.yml up
```

## Run Seq container
```bash
docker run --name seq -d -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
```

## Run Ollama container

Pull Ollama image

```bash
docker pull ollama/ollama
```

Run the Ollama container

```bash
docker run -d --name my-ollama -p 11434:11434 ollama/ollama
```
Access the ollama container

```bash
docker compose exec ollama bash
```
Pull needed LLM models

```bash
ollama pull llama3.1:8b
```

## Run frontend (optional) - https://github.com/Kondziuu03/ChatApp-frontend

```bash
npm start
```
