# Cirreum.Runtime.ServiceProvider

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.ServiceProvider.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.ServiceProvider/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.ServiceProvider.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.ServiceProvider/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.ServiceProvider?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.ServiceProvider/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Runtime.ServiceProvider?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Runtime.ServiceProvider/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Simplified service provider registration for ASP.NET Core applications**

## Overview

**Cirreum.Runtime.ServiceProvider** provides a streamlined approach to registering and configuring service providers in ASP.NET Core applications. It extends the `HostApplicationBuilder` with a generic, type-safe method for registering service providers with built-in configuration binding, health checks, and duplicate prevention.

## Key Features

- **Type-safe Registration** - Generic extension method with strong typing for settings and health options
- **Configuration Binding** - Automatic binding from `appsettings.json` using a consistent path pattern
- **Health Check Integration** - Built-in support for ASP.NET Core health checks
- **Duplicate Prevention** - Automatic detection and prevention of duplicate provider registrations
- **Deferred Logging** - Initialization tracking with deferred logging support

## Installation

```bash
dotnet add package Cirreum.Runtime.ServiceProvider
```

## Usage

Register a service provider in your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register a service provider with configuration
builder.RegisterServiceProvider<MyRegistrar, MySettings, MyInstanceSettings, MyHealthOptions>(
    "MyProvider",
    "MyProviderType"
);
```

Configure the provider in `appsettings.json`:

```json
{
  "Cirreum": {
    "MyProviderType": {
      "Providers": {
        "MyProvider": {
          "Setting1": "value1",
          "Setting2": 123
        }
      }
    }
  }
}
```

## Configuration Pattern

Service providers follow a consistent configuration path:
- **Path**: `Cirreum:{ProviderType}:Providers:{ProviderName}`
- **Provider Type**: Logical grouping of similar providers
- **Provider Name**: Unique identifier for each provider instance

## Requirements

- .NET 10.0 or later
- ASP.NET Core application

## Contribution Guidelines

1. **Be conservative with new abstractions**  
   The API surface must remain stable and meaningful.

2. **Limit dependency expansion**  
   Only add foundational, version-stable dependencies.

3. **Favor additive, non-breaking changes**  
   Breaking changes ripple through the entire ecosystem.

4. **Include thorough unit tests**  
   All primitives and patterns should be independently testable.

5. **Document architectural decisions**  
   Context and reasoning should be clear for future maintainers.

6. **Follow .NET conventions**  
   Use established patterns from Microsoft.Extensions.* libraries.

## Versioning

Cirreum.Runtime.ServiceProvider follows [Semantic Versioning](https://semver.org/):

- **Major** - Breaking API changes
- **Minor** - New features, backward compatible
- **Patch** - Bug fixes, backward compatible

Given its foundational role, major version bumps are rare and carefully considered.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*