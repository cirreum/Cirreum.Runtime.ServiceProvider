# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is the **Cirreum.Runtime.ServiceProvider** library - an ASP.NET Core extension library that provides service provider registration capabilities for the Cirreum Runtime Server. It's a .NET 10.0 library distributed as a NuGet package.

## Commands

### Build Commands
```bash
# Restore dependencies
dotnet restore Cirreum.Runtime.ServiceProvider.slnx

# Build the solution
dotnet build Cirreum.Runtime.ServiceProvider.slnx --configuration Release

# Pack for NuGet distribution
dotnet pack Cirreum.Runtime.ServiceProvider.slnx --configuration Release --output ./artifacts
```

### Publishing
The project uses GitHub Actions for CI/CD. Publishing to NuGet.org happens automatically when a release is created on GitHub.

## Architecture

### Core Extension Method
The main functionality is provided through the `RegisterServiceProvider` extension method in `src/Cirreum.Runtime.ServiceProvider/Extensions/Hosting/HostApplicationBuilderExtensions.cs`. This method:

- Registers service providers with strong typing for settings and health options
- Binds configuration from `Cirreum:{ProviderType}:Providers:{ProviderName}` path
- Prevents duplicate registrations
- Integrates with ASP.NET Core health checks
- Uses deferred logging for initialization tracking

### Configuration Pattern
Service providers are configured using the following pattern in appsettings.json:
```json
{
  "Cirreum": {
    "{ProviderType}": {
      "Providers": {
        "{ProviderName}": {
          // Provider-specific settings
        }
      }
    }
  }
}
```

### Dependencies
- **Framework**: .NET 10.0, ASP.NET Core
- **Key NuGet Packages**:
  - `Cirreum.Logging.Deferred` (v1.0.102) - For deferred logging
  - `Cirreum.ServiceProvider` (v1.0.2) - Base service provider interfaces

## Development Guidelines

### Code Style
- Uses .editorconfig with comprehensive C# style rules
- Tab indentation (4 spaces)
- Block-scoped namespace declarations
- Nullable reference types enabled
- XML documentation required for public APIs

### Versioning
- Semantic versioning (Major.Minor.Patch)
- Local builds use version 1.0.0
- CI/CD builds derive version from Git tags