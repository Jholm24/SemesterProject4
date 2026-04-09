# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Industry 4.0 production line orchestration system (SDU 4th semester project). A .NET 9 / Blazor Server application using a **Component-Based System (CBS)** architecture with MEF (Managed Extensibility Framework) for runtime plugin composition.

External machines communicating via three protocols:
- **AGV** (Automated Guided Vehicle): REST at `http://localhost:8082`
- **Warehouse**: SOAP at `http://localhost:8081/Service.asmx`
- **Assembly Station**: MQTT at `mqtt://localhost:1883`

## Build & Run

```bash
# Build entire solution
dotnet build SkateboardAS.sln

# Build and run the web app only
dotnet run --project SkateboardAS/SkateboardAS.csproj

# Build Infrastructure plugins (required before runtime loading)
dotnet build Infrastructure.Agv/Infrastructure.Agv.csproj
dotnet build Infrastructure.Warehouse/Infrastructure.Warehouse.csproj
dotnet build Infrastructure.Assembly/Infrastructure.Assembly.csproj
# Plugin DLLs output to: plugins/

# Start external services (docker-compose in root)
docker compose up
```

No test projects exist in this solution.

## Solution Structure

Seven projects with strict dependency rules:

```
Core          (no deps — contracts, models, enums, attributes)
  ↑
  ├── Infrastructure.Agv       → builds to plugins/Infrastructure.Agv/
  ├── Infrastructure.Warehouse → builds to plugins/Infrastructure.Warehouse/
  ├── Infrastructure.Assembly  → builds to plugins/Infrastructure.Assembly/
  ├── Orchestration            → MEF loading, lifecycle, registry
  ├── Shared                   → EF Core DbContext, Identity, repositories
  └── SkateboardAS (web)       → references Core + Orchestration + Shared only
```

**Critical constraint:** `SkateboardAS.csproj` does NOT reference any `Infrastructure.*` project. Infrastructure components are discovered at runtime via AssemblyLoadContext from the `plugins/` directory.

## Three-Layer Loading Architecture

This is the core architectural pattern. `Program.cs` wires it up:

1. **Layer 1 — AssemblyLoadContext** (`Orchestration/Loading/ComponentLoader.cs`): Loads each plugin DLL in isolation (like OSGi bundles). Prevents version conflicts between plugins.

2. **Layer 2 — MEF CompositionContainer** (`Orchestration/Loading/MefCompositionBuilder.cs`): Discovers `[Export]` attributes from isolated assemblies. Validates `[Requires]`/`[Provides]` contracts via `ContractValidator`. Provides metadata-based service registry.

3. **Layer 3 — ASP.NET Core DI**: Receives MEF-resolved instances and makes them available via constructor injection to controllers and Blazor pages.

## CBS Component Declaration Pattern

Every Infrastructure service must follow this exact pattern (see `Infrastructure.Agv/AgvService.cs`):

```csharp
[Component("Service Name", "1.0.0")]          // from Core/Attributes/
[Provides(typeof(ITheService))]               // contract this exposes
[Provides(typeof(IMachineComponent))]         // always expose this too
[Export(typeof(ITheService))]                 // MEF export
[Export(typeof(IMachineComponent))]
[ExportMetadata("Name", "...")]               // IComponentMetadata fields
[ExportMetadata("Version", "1.0.0")]
[ExportMetadata("Protocol", "REST|SOAP|MQTT")]
[ExportMetadata("MachineType", "AGV|Warehouse|AssemblyStation")]
[ExportMetadata("Description", "...")]
[ExportMetadata("Icon", "...")]               // icon name for UI card
[ExportMetadata("Priority", 0)]
public class TheService : ITheService, IMachineComponent, IComponent { ... }
```

Internal implementation details (HTTP/SOAP/MQTT clients) go in the `Internal/` subfolder and are never exposed outside the project.

## MVC + CBS Layering Rules

The application nests MVC inside CBS. These boundaries must be respected:

1. Controllers call `IProductionOrchestrator` only — never direct machine service interfaces
2. Blazor pages call controllers via HTTP API — never CBS interfaces directly
3. CBS components never reference the `SkateboardAS` web project
4. DTOs live in `SkateboardAS/DTOs/` — CBS uses `Core/Models/` types
5. `Program.cs` is the only explicit connection point between layers

## Key Conventions

- **Razor components**: Pages in `Components/Pages/Manager/` or `Components/Pages/Operator/`; reusable UI in `Components/UIComponents/`
- **Role-based pages**: Manager pages under `/manager/*`, Operator pages under `/operator/*`
- **Repository → Service**: All `Shared/Repositories/` are wrapped by `Shared/Services/`; controllers inject Services, not Repositories directly
- **ComponentState lifecycle**: `Installed → Starting → Active → Stopping → Uninstalled` (defined in `Core/Lifecycle/ComponentState.cs`)
- **CommandResult**: Use `CommandResult.Ok(message)` / `CommandResult.Fail(message)` from `Core/Models/CommandResult.cs` for all machine operation returns

## Database

PostgreSQL via EF Core. Connection string goes in `appsettings.Development.json` (gitignored) or environment variables.

```bash
# Run migrations
dotnet ef database update --project Shared/Shared.csproj --startup-project SkateboardAS/SkateboardAS.csproj

# Add a migration
dotnet ef migrations add MigrationName --project Shared/Shared.csproj --startup-project SkateboardAS/SkateboardAS.csproj
```

`AppDbContext` extends `IdentityDbContext<AppUser>` and lives in `Shared/Data/`.

## Authentication

JWT Bearer + ASP.NET Core Identity. Two roles: `Manager` and `Operator`. Auth config (secret, issuer, audience) lives in `Shared/Identity/IdentityConfig.cs` but actual values must come from config/secrets — the class is a placeholder.

## SignalR

`Hubs/ProductionHub.cs` broadcasts real-time machine status updates. Hub method: `SendStatusUpdate(machineId, status)`.

## docker-compose Services

| Service | Image | Port |
|---|---|---|
| mqtt | thmork/st4-mqtt | 1883, 9001 |
| st4-agv | thmork/st4-agv | 8082 |
| st4-warehouse | thmork/st4-warehouse | 8081 |
| st4-assemblystation | thmork/st4-assemblystation | (MQTT) |
| db | postgres:16 | 5432 |

DB credentials (docker): `skateboardas` / `skateboardas`
