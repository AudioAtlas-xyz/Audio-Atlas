# Backend Guide

## Installation of Prerequisites

* .NET 10 SDK (https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
* Ubuntu, Windows, or macOS

### Ubuntu

```bash
sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-10.0
```

### Windows

1. Download the .NET 10 SDK (x64) installer from the official Microsoft website
2. Run the installer and follow the setup instructions
3. Restart your terminal after installation

Verify installation:

```bash
dotnet --version
```

### macOS

#### Using Homebrew

```bash
brew update
brew install dotnet-sdk
```

#### Or via installer

1. Download the .NET 10 SDK installer for macOS from the official Microsoft website
2. Open the `.pkg` file and follow the installation steps

Verify installation:

```bash
dotnet --version
```

## How to Run

```bash
cd src/AudioAtlasView
dotnet run
```
