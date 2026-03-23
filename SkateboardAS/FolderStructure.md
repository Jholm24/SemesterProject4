# SkateboardAS — Project Context for Claude

## Project Overview

This is a 4th semester Software Technology Engineering project (ST4-PRO) at the University of Southern Denmark (SDU). The goal is to interface with a simulated Industry 4.0 production line.

The production process:
1. Request parts from an automated warehouse
2. AGV picks up and transports parts to the assembly station
3. Assembly station assembles the parts
4. AGV brings finished product back to warehouse

The system supports multiple production lines, role-based access (Manager / Operator), and a drag-and-drop GUI for composing production lines from discovered machine components.

---

## Technology Decisions

### Language: C# with ASP.NET Core (.NET 8)
- Course material references Java but no language constraint is imposed
- Team is more proficient in C# than Java
- C# + ASP.NET Core provides equivalent CBS support to Java
- Built-in DI container is a first-class citizen

### Frontend: Blazor Server
- Real-time monitoring via SignalR built in — critical for live machine status
- No JavaScript needed — full C#
- Push updates without polling — perfect for production line dashboard
- Aligns with Industry 4.0 narrative (browser accessible)

### Architecture: CBS + MVC + MEF + AssemblyLoadContext
- CBS (Component-Based System) at the **system** level — lives across ALL projects
- MVC at the **application** level — lives ONLY inside the Web project
- MEF (Managed Extensibility Framework) for composition, contract validation, and service registry
- AssemblyLoadContext for true runtime isolation (OSGi equivalent)
- ASP.NET Core DI for clean consumption by MVC controllers and Blazor pages

### Auth: ASP.NET Core Identity + JWT
- Two roles: Manager, Operator
- Manager: full CRUD on production lines, formulas, employees; monitor everything; drag-and-drop composition
- Operator: view and control only assigned production lines; start/stop lines

### Database: PostgreSQL with Entity Framework Core
- Stores users, production line configurations, formulas, task assignments

---

## External Assets and Protocols

| Asset | Protocol | Endpoint | Key Operations |
|---|---|---|---|
| Warehouse (Effimat) | SOAP | http://localhost:8081/Service.asmx | PickItem(trayId), InsertItem(trayId, name), GetInventory() |
| AGV (MiR + UR) | REST | http://localhost:8082/v1/status | GET status, PUT load program, PUT execute |
| Assembly Station (UR) | MQTT | mqtt://localhost:1883 | publish emulator/operation, subscribe emulator/status, emulator/checkhealth |

### AGV Programs
- MoveToChargerOperation
- MoveToAssemblyOperation
- MoveToStorageOperation
- PutAssemblyOperation
- PickAssemblyOperation
- PickWarehouseOperation
- PutWarehouseOperation

### State Information

**Warehouse states:** 0 = Idle, 1 = Executing, 2 = Error

**AGV states:** 1 = Idle, 2 = Executing, 3 = Charging

**Assembly states:** 0 = Idle, 1 = Executing, 2 = Error

---

## CBS — The 7 Pillars and C# Implementation

### What is a CBS?
A Component-Based System must satisfy 7 core pillars:

1. **Component Model** — interface + identity + boundary + dependency declaration
2. **Encapsulation** — internal details hidden, only interface visible
3. **Explicit Contracts** — dependencies declared explicitly and verifiably
4. **Late Binding** — components wired at runtime, not compile time
5. **Lifecycle Management** — installed → starting → active → stopping → uninstalled
6. **Service Registry** — central registry where components register and consumers look up
7. **Versioning** — version declarations and compatibility checking

### Why Java/OSGi is the CBS reference (5/5 on all pillars)
OSGi enforces ALL 7 pillars at the JVM/platform level:
- MANIFEST.MF = component model + encapsulation + contracts in one file
- OSGi Service Registry = platform-level service registry
- Bundle lifecycle state machine = enforced by runtime
- Per-bundle classloader = true isolation, multiple versions can coexist

### C# pillar ratings vs Java

| Pillar | C# Implementation | Rating | Java OSGi |
|---|---|---|---|
| Component Model | Interface + `[Component]` attribute + .csproj boundary | 4/5 | 5/5 |
| Encapsulation | `internal` + `InternalsVisibleTo` + .csproj refs + AssemblyLoadContext isolation | 4/5 | 5/5 |
| Explicit Contracts | `[Provides]` `[Requires]` + ContractValidator fail-fast + MEF `[Export]`/`[Import]` | 3/5 | 5/5 |
| Late Binding | AssemblyLoadContext runtime loading + MEF composition + DI resolution | 4/5 | 5/5 |
| Lifecycle Management | `IComponent` state machine + `ComponentLifecycleManager` via `IHostedService` | 3/5 | 5/5 |
| Service Registry | MEF `CompositionContainer` + metadata + `Lazy<T, TMetadata>` deferred instantiation | 4/5 | 5/5 |
| Versioning | `VersionedComponentRegistry` + `[ExportMetadata("Version", ...)]` + compatibility checks | 2/5 | 5/5 |
| **Overall** | | **3.4/5** | **5/5** |

### How We Maximize Each Pillar

**Component Model (4/5):** Every component has a `[Component]` attribute declaring its identity. Each lives in its own .csproj (assembly boundary). Dependencies are declared via `[Provides]` and `[Requires]` attributes — inspectable at composition time.

**Encapsulation (4/5):** Each component assembly uses `internal` on all implementation classes. Only the `[Export]` entry point and shared contract interfaces are `public`. AssemblyLoadContext gives each component its own isolated context — closest .NET equivalent to OSGi per-bundle classloaders.

**Explicit Contracts (3/5):** `[Provides]` and `[Requires]` attributes on each component class. `ContractValidator` scans all loaded components at startup and throws if any `[Requires]` is unmet by a corresponding `[Provides]`. MEF `[Export]`/`[Import]` provides the runtime wiring.

**Late Binding (4/5):** Components are loaded from DLLs at runtime via `AssemblyLoadContext` — no compile-time project references from the host. MEF discovers exports from loaded assemblies. ASP.NET Core DI receives resolved instances. The host never knows concrete types.

**Lifecycle Management (3/5):** `IComponent` interface defines the state machine: `Installed → Starting → Active → Stopping → Uninstalled`. `ComponentLifecycleManager` (an `IHostedService`) drives transitions at startup and shutdown. Components report their own `ComponentState`.

**Service Registry (4/5):** MEF `CompositionContainer` acts as the registry. Components register via `[Export]`, consumers look up via `[Import]` or `container.GetExportedValue<T>()`. `Lazy<T, TMetadata>` enables inspection without instantiation — metadata (Name, Version, Protocol, MachineType) is available before the component is created.

**Versioning (2/5):** `[ExportMetadata("Version", "1.0.0")]` on each component. `VersionedComponentRegistry` can enforce compatibility rules. Weakest pillar — .NET doesn't enforce version isolation the way OSGi does, but AssemblyLoadContext helps.

---

## Three-Layer Loading Architecture

```
Layer 1 — AssemblyLoadContext (ISOLATION)
  Each component .dll loaded into its own isolated context
  Mirrors OSGi per-bundle classloader
  Enables true runtime isolation between components
  Components loaded from plugins/ directory

Layer 2 — MEF CompositionContainer (COMPOSITION + REGISTRY)
  Discovers [Export] attributes from isolated assemblies
  Validates all [Import] contracts at composition time
  Throws CompositionException if any contract is unmet
  Provides metadata-based service registry
  Supports Lazy<T, TMetadata> for deferred instantiation

Layer 3 — ASP.NET Core DI (CONSUMPTION)
  Receives MEF-resolved instances
  Makes them available via constructor injection
  Integrates cleanly with MVC controllers and Blazor pages
```

---

## MVC + CBS Relationship

```
CBS is the SYSTEM level architecture
→ lives across ALL projects in the solution

MVC is the APPLICATION level architecture
→ lives ONLY inside the Web (SkateboardAS) project

They nest — MVC lives inside CBS, not alongside it
```

### Rules That Keep Them Clean
1. Controllers only call `IProductionOrchestrator` — never individual machine services directly
2. Blazor pages only call controllers via API/service layer — never inject CBS interfaces directly
3. CBS components never know about MVC — they have no reference to the Web project
4. DTOs live in the Web project only — CBS models stay in Core
5. `Program.cs` is the only place CBS and MVC explicitly connect (the three-layer wiring)

---

## Solution Structure

```
SkateboardAS/                                 ← solution root
│
├── SkateboardAS.sln                          ← open THIS in IDE
├── README.md
├── docker-compose.yml
├── plugins/                                  ← component DLLs output here — loaded at runtime
│
├── Core/                                     ← CBS contracts — NO implementation, NO dependencies
│   ├── Core.csproj
│   ├── Interfaces/
│   │   ├── IMachineComponent.cs              ← core MEF contract all machine components export
│   │   ├── IAgvService.cs                    ← AGV-specific operations (LoadProgram, Execute)
│   │   ├── IWarehouseService.cs              ← Warehouse-specific (PickItem, InsertItem, GetInventory)
│   │   ├── IAssemblyService.cs               ← Assembly-specific (StartOperation, CheckHealth)
│   │   ├── IProductionOrchestrator.cs        ← orchestrates the full production cycle
│   │   └── IComponentUIDescriptor.cs         ← contract for component-owned UI card definitions
│   ├── Lifecycle/
│   │   ├── IComponent.cs                     ← lifecycle: StartAsync, StopAsync, State
│   │   └── ComponentState.cs                 ← enum: Installed, Starting, Active, Stopping, Uninstalled
│   ├── Metadata/
│   │   └── IComponentMetadata.cs             ← MEF metadata: Name, Version, Protocol, MachineType, Description, Icon, Priority
│   ├── Attributes/
│   │   ├── ComponentAttribute.cs             ← [Component("AGV Service", "1.0.0")] — identity
│   │   ├── ProvidesAttribute.cs              ← [Provides(typeof(IAgvService))] — explicit contract
│   │   └── RequiresAttribute.cs              ← [Requires(typeof(IWarehouseService))] — explicit dependency
│   ├── Models/
│   │   ├── MachineStatusModel.cs             ← shared: Id, Name, State, Timestamp
│   │   ├── CommandResult.cs                  ← shared result for Start/Stop/Reset
│   │   ├── ComponentCardModel.cs             ← UI card: name, icon, status fields, actions
│   │   ├── AgvStatus.cs                      ← AGV-specific: position, battery, current program
│   │   ├── AssemblyStatus.cs                 ← Assembly-specific: operation, progress, health
│   │   ├── Inventory.cs                      ← Warehouse inventory model
│   │   └── ProductionStatus.cs               ← overall production line status
│   ├── Enums/
│   │   ├── MachineType.cs                    ← AGV, AssemblyStation, Warehouse
│   │   ├── MachineStatus.cs                  ← Idle, Running, Error, Offline
│   │   └── UserRole.cs                       ← Manager, Operator
│   └── Events/
│       ├── MachineStatusChangedEvent.cs      ← published when a machine changes state
│       └── ComponentLoadedEvent.cs           ← published when MEF discovers a new component
│
├── Infrastructure.Agv/                       ← CBS component 1 — AGV (REST protocol)
│   ├── Infrastructure.Agv.csproj             ← references Core only; outputs to ../../plugins/
│   ├── AgvService.cs                         ← [Export(typeof(IAgvService))] + [Export(typeof(IMachineComponent))] + IComponent
│   ├── AgvUIDescriptor.cs                    ← [Export(typeof(IComponentUIDescriptor))] — card layout
│   └── Internal/                             ← ALL classes here are internal — encapsulation
│       ├── AgvHttpClient.cs                  ← internal: REST calls to http://localhost:8082
│       ├── AgvStatusMapper.cs                ← internal: maps raw REST response to AgvStatus
│       ├── AgvState.cs                       ← internal: position, battery, current route
│       └── AgvConfig.cs                      ← internal: speed, max payload, home position
│
├── Infrastructure.Warehouse/                 ← CBS component 2 — Warehouse (SOAP protocol)
│   ├── Infrastructure.Warehouse.csproj       ← references Core only; outputs to ../../plugins/
│   ├── WarehouseService.cs                   ← [Export(typeof(IWarehouseService))] + [Export(typeof(IMachineComponent))] + IComponent
│   ├── WarehouseUIDescriptor.cs              ← [Export(typeof(IComponentUIDescriptor))]
│   └── Internal/
│       ├── WarehouseSoapClient.cs            ← internal: SOAP calls to http://localhost:8081
│       ├── InventoryMapper.cs                ← internal: maps SOAP response to Inventory
│       ├── WarehouseState.cs                 ← internal: inventory levels, slot occupancy
│       └── WarehouseConfig.cs                ← internal: capacity, slot layout
│
├── Infrastructure.Assembly/                  ← CBS component 3 — Assembly Station (MQTT protocol)
│   ├── Infrastructure.Assembly.csproj        ← references Core only; outputs to ../../plugins/
│   ├── AssemblyService.cs                    ← [Export(typeof(IAssemblyService))] + [Export(typeof(IMachineComponent))] + IComponent
│   ├── AssemblyUIDescriptor.cs               ← [Export(typeof(IComponentUIDescriptor))]
│   └── Internal/
│       ├── MqttClientWrapper.cs              ← internal: MQTT pub/sub to mqtt://localhost:1883
│       ├── AssemblyStatusMapper.cs           ← internal: maps MQTT messages to AssemblyStatus
│       ├── AssemblyState.cs                  ← internal: current operation, progress, queue
│       └── AssemblyConfig.cs                 ← internal: tools available, cycle time
│
├── Orchestration/                            ← CBS coordination layer — ties components together
│   ├── Orchestration.csproj                  ← references Core only
│   ├── ProductionOrchestrator.cs             ← implements IProductionOrchestrator — runs the 16-step sequence
│   ├── Loading/
│   │   ├── ComponentLoader.cs                ← AssemblyLoadContext: loads each DLL in isolated context (Pillar 2 + 4)
│   │   └── MefCompositionBuilder.cs          ← MEF: builds CompositionContainer from loaded assemblies (Pillar 3 + 6)
│   ├── Lifecycle/
│   │   └── ComponentLifecycleManager.cs      ← IHostedService: drives Installed→Starting→Active→Stopping→Uninstalled (Pillar 5)
│   ├── Registry/
│   │   ├── ComponentServiceRegistry.cs       ← MEF-backed registry with Lazy<T, TMetadata> (Pillar 6)
│   │   └── VersionedComponentRegistry.cs     ← version tracking and compatibility checking (Pillar 7)
│   ├── Validation/
│   │   └── ContractValidator.cs              ← scans [Provides]/[Requires], fails fast if unmet (Pillar 3)
│   └── Recomposition/
│       └── PluginDirectoryWatcher.cs         ← FileSystemWatcher on plugins/ — triggers re-scan
│
├── Shared/                                   ← cross-cutting concerns NOT specific to any machine
│   ├── Shared.csproj                         ← references Core
│   ├── Data/
│   │   ├── AppDbContext.cs                   ← EF Core DbContext (PostgreSQL)
│   │   └── Migrations/
│   ├── Identity/
│   │   ├── AppUser.cs                        ← IdentityUser extension: role, assigned production lines
│   │   └── IdentityConfig.cs                 ← JWT + Identity setup
│   ├── Entities/
│   │   ├── ProductionLine.cs                 ← stores assigned component IDs
│   │   ├── Formula.cs                        ← product recipe: combination of component types
│   │   └── TaskAssignment.cs                 ← manager assigns operator to production line
│   ├── Services/
│   │   ├── ProductionLineService.cs          ← compose/start/stop production lines
│   │   ├── UserService.cs                    ← register, assign, remove operators
│   │   └── FormulaService.cs                 ← CRUD product formulas
│   └── Repositories/
│       ├── ProductionLineRepository.cs
│       ├── UserRepository.cs
│       └── FormulaRepository.cs
│
└── SkateboardAS/                             ← MVC + Blazor Server Web layer — the host shell
    ├── SkateboardAS.csproj                   ← references Core + Orchestration + Shared (NOT Infrastructure.*)
    ├── Program.cs                            ← wires all 3 layers: AssemblyLoadContext → MEF → DI
    ├── appsettings.json                      ← DB connection, JWT config, plugin directory path
    ├── Dockerfile
    ├── Controllers/                          ← MVC — thin routing, delegates to orchestrator/services
    │   ├── ProductionController.cs           ← start/stop production cycles via IProductionOrchestrator
    │   ├── ComponentDiscoveryController.cs   ← GET /api/components — returns discovered metadata + card models
    │   ├── AuthController.cs                 ← login, register, JWT issuance
    │   ├── ProductionLineController.cs       ← CRUD + start/stop production lines
    │   ├── UserController.cs                 ← manager: employee management
    │   └── FormulaController.cs              ← CRUD for product formulas
    ├── Hubs/
    │   └── ProductionHub.cs                  ← SignalR real-time machine status push to Blazor
    ├── DTOs/                                 ← MVC models — NEVER CBS models from Core
    │   ├── AgvStatusDto.cs
    │   ├── WarehouseStatusDto.cs
    │   ├── AssemblyStatusDto.cs
    │   ├── ProductionStatusDto.cs
    │   └── UserDto.cs
    ├── Components/                           ← Blazor Server views
    │   ├── Layout/
    │   │   ├── MainLayout.razor
    │   │   └── NavMenu.razor
    │   └── Pages/
    │       ├── Login.razor
    │       ├── Manager/
    │       │   ├── Dashboard.razor           ← renders ALL discovered components as cards via metadata
    │       │   ├── ProductionLineEditor.razor ← drag-and-drop: assign components to lines using metadata
    │       │   ├── EmployeeManagement.razor   ← register, assign tasks, remove operators
    │       │   └── FormulaEditor.razor        ← create/edit component combinations
    │       └── Operator/
    │           ├── MyLines.razor              ← view assigned production lines
    │           └── LineControl.razor          ← start/stop/monitor a single line
    ├── UIComponents/                         ← reusable Blazor UI components (NOT CBS components)
    │   ├── MachineCard.razor                 ← GENERIC card — renders ANY machine from ComponentCardModel
    │   ├── ProductionLineCard.razor          ← card showing a production line with its composed components
    │   ├── DragDropZone.razor                ← drag-and-drop container for composition
    │   └── StatusIndicator.razor             ← visual badge: Idle / Running / Error / Offline
    └── wwwroot/
```

---

## Project References — STRICTLY ENFORCED

```
Core                     → (no references — foundation layer)
Infrastructure.Agv       → Core
Infrastructure.Warehouse → Core
Infrastructure.Assembly  → Core
Orchestration            → Core
Shared                   → Core
SkateboardAS             → Core + Orchestration + Shared

CRITICAL: Infrastructure projects do NOT reference each other
CRITICAL: SkateboardAS does NOT reference Infrastructure.* projects directly
          It only sees them via interfaces resolved through MEF + DI at runtime
CRITICAL: No component project references another component project
```

### Component .csproj Template (every Infrastructure.* project)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputPath>..\..\plugins\$(AssemblyName)</OutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Core\Core.csproj" />
  </ItemGroup>

  <!-- Protocol-specific packages added per component -->
</Project>
```

---

## NuGet Packages

| Project | Package | Purpose |
|---|---|---|
| Core | System.Composition.AttributedModel | MEF attributes: Export, Import, ExportMetadata |
| Orchestration | System.Composition.Hosting | MEF CompositionContainer |
| Orchestration | System.Composition.TypedParts | MEF typed part support |
| Shared | Npgsql.EntityFrameworkCore.PostgreSQL | PostgreSQL via EF Core |
| Shared | Microsoft.AspNetCore.Identity.EntityFrameworkCore | Identity + roles |
| SkateboardAS | System.Composition | MEF |
| SkateboardAS | Microsoft.AspNetCore.Authentication.JwtBearer | JWT auth |
| SkateboardAS | Swashbuckle.AspNetCore | Swagger API docs |
| Infrastructure.Agv | Microsoft.Extensions.Http | HttpClient for REST |
| Infrastructure.Agv | System.Composition.AttributedModel | MEF Export |
| Infrastructure.Warehouse | System.ServiceModel.Http | SOAP client |
| Infrastructure.Warehouse | System.Composition.AttributedModel | MEF Export |
| Infrastructure.Assembly | MQTTnet | MQTT client |
| Infrastructure.Assembly | MQTTnet.Extensions.ManagedClient | Managed MQTT |
| Infrastructure.Assembly | System.Composition.AttributedModel | MEF Export |

---

## Key Contract Definitions

### IComponent.cs (lifecycle — Pillar 5)
```csharp
namespace Core.Lifecycle;

public interface IComponent
{
    ComponentState State { get; }
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync(CancellationToken ct = default);
}
```

### ComponentState.cs (lifecycle states — Pillar 5)
```csharp
namespace Core.Lifecycle;

public enum ComponentState
{
    Installed,
    Starting,
    Active,
    Stopping,
    Uninstalled
}
```

### IMachineComponent.cs (core MEF contract — Pillar 1 + 6)
```csharp
namespace Core.Interfaces;

public interface IMachineComponent
{
    string Id { get; }
    string Name { get; }
    MachineType Type { get; }
    MachineStatus Status { get; }
    Task<CommandResult> StartAsync(CancellationToken ct = default);
    Task<CommandResult> StopAsync(CancellationToken ct = default);
    Task<CommandResult> ResetAsync(CancellationToken ct = default);
    Task<MachineStatusModel> GetStatusAsync(CancellationToken ct = default);
}
```

### IComponentMetadata.cs (metadata — Pillar 6)
```csharp
namespace Core.Metadata;

public interface IComponentMetadata
{
    string Name { get; }
    string Version { get; }
    string Protocol { get; }
    string MachineType { get; }
    string Description { get; }
    string Icon { get; }
    int Priority { get; }
}
```

### IComponentUIDescriptor.cs (UI card contract)
```csharp
namespace Core.Interfaces;

public interface IComponentUIDescriptor
{
    string ComponentType { get; }
    ComponentCardModel GetCardModel();
    IEnumerable<string> GetAvailableActions();
    IEnumerable<string> GetDisplayedStatusFields();
}
```

### Custom CBS Attributes (Pillar 1 + 3)
```csharp
namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ComponentAttribute : Attribute
{
    public string Name { get; }
    public string Version { get; }
    public ComponentAttribute(string name, string version)
    {
        Name = name;
        Version = version;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ProvidesAttribute : Attribute
{
    public Type Contract { get; }
    public ProvidesAttribute(Type contract) { Contract = contract; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiresAttribute : Attribute
{
    public Type Contract { get; }
    public RequiresAttribute(Type contract) { Contract = contract; }
}
```

---

## Key Code Patterns

### Component Declaration (every Infrastructure.* project follows this)
```csharp
using System.Composition;
using Core.Interfaces;
using Core.Lifecycle;
using Core.Attributes;
using Core.Enums;

namespace Infrastructure.Agv;

[Component("AGV Service", "1.0.0")]
[Provides(typeof(IAgvService))]
[Provides(typeof(IMachineComponent))]
[Export(typeof(IAgvService))]
[Export(typeof(IMachineComponent))]
[ExportMetadata("Name", "AGV Service")]
[ExportMetadata("Version", "1.0.0")]
[ExportMetadata("Protocol", "REST")]
[ExportMetadata("MachineType", "AGV")]
[ExportMetadata("Description", "Automated Guided Vehicle for part transport")]
[ExportMetadata("Icon", "truck")]
[ExportMetadata("Priority", 0)]
public class AgvService : IAgvService, IMachineComponent, IComponent
{
    private readonly AgvHttpClient _client = new();  // internal class — encapsulated

    public ComponentState State { get; private set; } = ComponentState.Installed;
    public string Id => "agv-01";
    public string Name => "AGV";
    public MachineType Type => MachineType.AGV;
    public MachineStatus Status { get; private set; } = MachineStatus.Offline;

    // IComponent lifecycle
    public async Task StartAsync(CancellationToken ct = default)
    {
        State = ComponentState.Starting;
        await _client.VerifyConnectionAsync(ct);
        Status = MachineStatus.Idle;
        State = ComponentState.Active;
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        State = ComponentState.Stopping;
        Status = MachineStatus.Offline;
        State = ComponentState.Uninstalled;
    }

    // IMachineComponent + IAgvService methods...
}
```

### Program.cs Wiring (three layers)
```csharp
// Layer 1 — AssemblyLoadContext isolation
var loader = new ComponentLoader();
var agvAssembly = loader.LoadComponent("plugins/Infrastructure.Agv/Infrastructure.Agv.dll");
var warehouseAssembly = loader.LoadComponent("plugins/Infrastructure.Warehouse/Infrastructure.Warehouse.dll");
var assemblyAssembly = loader.LoadComponent("plugins/Infrastructure.Assembly/Infrastructure.Assembly.dll");

// Layer 2 — MEF composition + validation
var mefContainer = new MefCompositionBuilder()
    .AddAssembly(agvAssembly)
    .AddAssembly(warehouseAssembly)
    .AddAssembly(assemblyAssembly)
    .Build(); // throws CompositionException if any contract violated

// Contract validation (Pillar 3)
ContractValidator.Validate(mefContainer); // fail-fast if [Requires] unmet

// Layer 3 — ASP.NET Core DI consumption
builder.Services.AddSingleton(mefContainer.GetExportedValue<IAgvService>());
builder.Services.AddSingleton(mefContainer.GetExportedValue<IWarehouseService>());
builder.Services.AddSingleton(mefContainer.GetExportedValue<IAssemblyService>());
builder.Services.AddScoped<IProductionOrchestrator, ProductionOrchestrator>();
builder.Services.AddHostedService<ComponentLifecycleManager>();

// Auth + DB
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(/* config */);
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// SignalR for real-time updates
builder.Services.AddSignalR();
```

### MVC Controller Pattern
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly IProductionOrchestrator _orchestrator;

    public ProductionController(IProductionOrchestrator orchestrator)
        => _orchestrator = orchestrator;

    [HttpPost("start")]
    [Authorize(Roles = "Operator,Manager")]
    public async Task<IActionResult> Start()
    {
        await _orchestrator.RunProductionCycleAsync();
        return Ok();
    }
}
```

### ComponentDiscoveryController (metadata-driven UI — Pillar 6)
```csharp
[ApiController]
[Route("api/components")]
public class ComponentDiscoveryController : ControllerBase
{
    private readonly IEnumerable<Lazy<IMachineComponent, IComponentMetadata>> _components;

    public ComponentDiscoveryController(
        IEnumerable<Lazy<IMachineComponent, IComponentMetadata>> components)
        => _components = components;

    [HttpGet]
    public IActionResult GetAll()
    {
        // Returns metadata WITHOUT instantiating components
        var cards = _components.Select(c => new {
            c.Metadata.Name,
            c.Metadata.MachineType,
            c.Metadata.Version,
            c.Metadata.Protocol,
            c.Metadata.Description,
            c.Metadata.Icon
        });
        return Ok(cards);
    }
}
```

---

## Production Sequence (from documentation)

```
1.  Warehouse.PickItem(trayId)
2.  AGV.LoadProgram("MoveToStorageOperation")
3.  AGV.ExecuteProgram()
4.  AGV.LoadProgram("PickWarehouseOperation")
5.  AGV.ExecuteProgram()
6.  AGV.LoadProgram("MoveToAssemblyOperation")
7.  AGV.ExecuteProgram()
8.  AGV.LoadProgram("PutAssemblyOperation")
9.  AGV.ExecuteProgram()
10. Assembly.StartOperation(processId)
11. Assembly.CheckHealth() → bool isHealthy
12. AGV.LoadProgram("PickAssemblyOperation")
13. AGV.ExecuteProgram()
14. AGV.LoadProgram("MoveToStorageOperation")
15. AGV.ExecuteProgram()
16. if isHealthy → Warehouse.InsertItem(acceptedTrayId, "Accepted Product")
    else         → Warehouse.InsertItem(defectTrayId, "Defect Product")
```

---

## Docker Setup

```yaml
# docker-compose.yml
services:
  mqtt:
    image: thmork/st4-mqtt:latest
    ports: [1883:1883, 9001:9001]

  st4-agv:
    image: thmork/st4-agv:latest
    ports: [8082:80]

  st4-warehouse:
    image: thmork/st4-warehouse:latest
    ports: [8081:80]

  st4-assemblystation:
    image: thmork/st4-assemblystation:latest
    environment:
      MQTT_TCP_CONNECTION_HOST: "mqtt"
      MQTT_TCP_CONNECTION_PORT: 1883

  db:
    image: postgres:16
    environment:
      POSTGRES_USER: skateboardas
      POSTGRES_PASSWORD: skateboardas
      POSTGRES_DB: skateboardas
    ports: [5432:5432]
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

Run with: `docker-compose up`

---

## Auth Roles

- **Manager:** Full CRUD on production lines, formulas, employees. Sees all lines and components. Drag-and-drop composition. Registers new employees. Assigns/removes operators from lines.
- **Operator:** View and control only assigned production lines. Start/stop lines. Can self-assign to available tasks. Cannot modify line composition or manage employees.

---

## Placeholder File Rules

Every `.cs` stub must:
- Use file-scoped namespace: `namespace Infrastructure.Agv.Internal;`
- Contain a compilable class/interface with `// TODO: Implement`
- Internal implementation classes: `internal class AgvHttpClient`
- Exported/public classes: `public class AgvService`
- Enums must have their values defined (not empty)

---

## Git Workflow

```
main
└── feature/cbs-mvc-structure

Terminal: always use Git Bash (not PowerShell)
Commit style: make all changes first, then single commit at the end
```

Always open `SkateboardAS.sln` in IDE — NOT a subfolder directly.

---

## What NOT to Do

- **Never** add `<ProjectReference>` from SkateboardAS to any `Infrastructure.*` project. Runtime discovery only via AssemblyLoadContext + MEF.
- **Never** let Infrastructure projects reference each other. Components are fully independent.
- **Never** hard-code machine types in SkateboardAS or Shared. Everything via contracts + metadata.
- **Never** put component-specific logic in SkateboardAS. It's a thin host shell: discover, compose, route.
- **Never** use MEF1 (`System.ComponentModel.Composition`). Use MEF2 (`System.Composition`).
- **Never** skip `[Component]`, `[Provides]`, `[Requires]` attributes. They make Pillar 1 and 3 inspectable.
- **Never** skip `[ExportMetadata]`. Metadata powers the UI, registry, and filtering.
- **Never** use CBS Core models as DTOs in controllers. Map to DTOs in the Web project.
- **Never** let Blazor pages inject CBS interfaces directly. Go through controllers/services.
- **Never** let controllers call machine services directly. Always go through `IProductionOrchestrator`.
