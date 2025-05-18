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
