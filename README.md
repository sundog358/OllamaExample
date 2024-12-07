# �� Ollama Chat Client by Sun & Rain Works

A modern console-based chat client for Ollama, built with .NET 9 and Semantic Kernel. Experience natural conversations with advanced AI models through a sleek command-line interface.

## 🌟 Key Features

- 🔄 Real-time streaming responses with token-by-token output
- 🎨 Rich colored console interface for enhanced readability
- 📝 Intelligent markdown code block handling
- 🔌 Seamless integration with Ollama API
- 🛡️ Robust error handling and graceful degradation
- 🧠 Powered by Microsoft's Semantic Kernel
- 💬 Full chat history support
- 🎯 Support for multiple LLM models

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Ollama](https://ollama.ai/download)
- Llama model installed: `bash
ollama pull llama3:latest  `

### 🛠️ Quick Start

1. Clone and navigate: `bash
git clone https://github.com/sunandrain/ollama-chat-client.git
cd ollama-chat-client   `

2. Install dependencies: `bash
dotnet restore   `

3. Start Ollama server: `bash
ollama serve   `

4. Run the application: `bash
dotnet run   `

## 💡 Usage Guide

### Basic Commands

- Type your message and press Enter to send
- Press Enter with empty input or type 'exit' to quit
- Messages are color-coded for better visualization:
  - 🟢 User input (Green)
  - 🔵 Assistant responses (Cyan headers)
  - 🔴 Error messages (Red)
  - 🟡 System messages (Yellow)

### Example Interaction
