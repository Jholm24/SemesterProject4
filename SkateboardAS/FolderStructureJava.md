# SkateboardAS — JPMS Project Instructions for Claude

## Project Overview

This is a 4th semester Software Technology Engineering project (ST4-PRO) at the University of Southern Denmark (SDU). The goal is to build a component-based system that interfaces with a simulated Industry 4.0 production line.

The production process:
1. Request parts from an automated warehouse
2. AGV picks up and transports parts to the assembly station
3. Assembly station assembles the parts
4. AGV brings finished product back to warehouse

The system supports multiple production lines, role-based access (Manager / Operator), and a GUI for composing production lines from discovered machine components.

---

## Technology Stack

| Concern | Technology |
|---|---|
| Language | Java 21 |
| Module System | JPMS (`module-info.java`) |
| Build Tool | Maven (multi-module) |
| Web Framework | Spring Boot 3.2+ |
| Frontend | Thymeleaf + HTMX for partial updates |
| Real-time | Spring WebSocket (STOMP over SockJS) |
| Auth | Spring Security + JWT (roles: MANAGER, OPERATOR) |
| ORM | Spring Data JPA / Hibernate |
| Database | PostgreSQL 16 |
| Service Discovery | `java.util.ServiceLoader` |
| Containerization | Docker + docker-compose |

---

## Architecture: CBS + MVC + JPMS + ServiceLoader

- **CBS** at the **system** level — enforced by `module-info.java` across ALL modules
- **MVC** at the **application** level — lives ONLY inside the `web` module (Spring MVC)
- **JPMS** enforces encapsulation, contracts, and module boundaries at the JVM level
- **ServiceLoader** provides late binding and service registry — replaces MEF entirely
- **Spring DI** for consumption within the web and orchestration layers

```
CBS is the SYSTEM level architecture
→ enforced by module-info.java across ALL modules

MVC is the APPLICATION level architecture
→ lives ONLY inside the web module

They nest — MVC lives inside CBS, not alongside it
```

### Rules That Keep Them Clean
1. Controllers only call `IProductionOrchestrator` — never individual machine services directly
2. Thymeleaf templates only receive data from controllers — never access CBS interfaces
3. CBS modules never know about Spring MVC — they have no `requires spring.web`
4. DTOs live in the web module only — CBS models stay in core
5. `ServiceLoaderConfig.java` is the only place CBS and Spring explicitly connect

---

## External Assets and Protocols

| Asset | Protocol | Endpoint | Key Operations |
|---|---|---|---|
| Warehouse (Effimat) | SOAP | http://localhost:8081/Service.asmx | pickItem(trayId), insertItem(trayId, name), getInventory() |
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

## CBS — The 6 Pillars in JPMS

A Component-Based System satisfies these core pillars. JPMS enforces most of them at the platform level.

### Pillar Ratings

| Pillar | JPMS Implementation | Rating |
|---|---|---|
| Component Model | `module-info.java` = identity + boundary + dependency declaration | 5/5 |
| Encapsulation | `exports` directive — unexported packages invisible, enforced by JVM | 5/5 |
| Explicit Contracts | `requires` + `provides...with` — validated at compile time AND startup | 5/5 |
| Late Binding | `ServiceLoader.load()` discovers implementations at runtime | 5/5 |
| Lifecycle Management | `MachineLifecycle` interface + Spring `SmartLifecycle` | 4/5 |
| Service Registry | `ServiceLoader.stream()` with lazy metadata via annotations | 4/5 |
| **Overall** | | **4.7/5** |

### How Each Pillar Is Implemented

**Component Model (5/5):** Every module has a `module-info.java` that declares its identity, boundaries, and dependencies. Each machine component is its own Maven module with its own `module-info.java`. The JVM treats this as a first-class module descriptor.

**Encapsulation (5/5):** Only packages listed in `exports` are visible to other modules. The `internal` package in each infrastructure module is NOT exported — the JVM refuses access at runtime, even via reflection (unless `opens` is declared). This is stronger than any manual enforcement.

**Explicit Contracts (5/5):** `requires dk.sdu.core` declares what a module depends on. `provides dk.sdu.core.api.IAgvService with dk.sdu.infrastructure.agv.AgvService` declares what it offers. The compiler validates these. If a `requires` is missing at startup, the JVM refuses to start. No custom validator needed.

**Late Binding (5/5):** `ServiceLoader.load(IMachineComponent.class)` discovers all implementations declared via `provides...with` across all modules on the module path. No compile-time references from the host to implementations. The `uses` directive in the consuming module declares intent without binding to a concrete class.

**Lifecycle Management (4/5):** `MachineLifecycle` interface defines the state machine: `INSTALLED → STARTING → ACTIVE → STOPPING → UNINSTALLED`. `ComponentLifecycleManager` implements Spring `SmartLifecycle` to drive transitions at application startup and shutdown. Not 5/5 because Java doesn't enforce a lifecycle at the platform level the way OSGi does.

**Service Registry (4/5):** `ServiceLoader` is the registry. `ServiceLoader.stream()` provides lazy access — you can inspect the provider's class (and its `@ComponentMetadata` annotation) without instantiating it. Not 5/5 because there's no built-in query/filter API like OSGi's service registry.

---

## Task: Scaffold the Folder Structure

Generate this exact structure as a multi-module Maven project. Every `.java` file must contain a valid package declaration and stub class/interface. Use `dk.sdu.<module>.<package>` naming.

```
skateboard-as/                                        ← project root
│
├── pom.xml                                           ← parent POM — defines modules, shared dependencies, Java 21
├── README.md
├── docker-compose.yml
│
│
│   # ================================================================
│   # CORE — CBS contracts. NO implementation. NO dependencies.
│   # Every other module depends on this.
│   # ================================================================
│
├── core/
│   ├── pom.xml                                       ← no dependencies (pure Java)
│   └── src/main/java/
│       ├── module-info.java                          ← exports ALL packages, requires nothing
│       └── dk/sdu/core/
│           ├── api/
│           │   ├── IMachineComponent.java            ← core service interface all machines implement
│           │   ├── IAgvService.java                  ← AGV-specific: loadProgram(), execute(), getStatus()
│           │   ├── IWarehouseService.java            ← Warehouse-specific: pickItem(), insertItem(), getInventory()
│           │   ├── IAssemblyService.java             ← Assembly-specific: startOperation(), checkHealth()
│           │   ├── IProductionOrchestrator.java      ← orchestrates the full production cycle
│           │   └── IComponentUIDescriptor.java       ← contract for component-owned UI card definitions
│           ├── lifecycle/
│           │   ├── MachineLifecycle.java             ← interface: start(), stop(), getState()
│           │   └── ComponentState.java               ← enum: INSTALLED, STARTING, ACTIVE, STOPPING, UNINSTALLED
│           ├── metadata/
│           │   └── ComponentMetadata.java            ← @ComponentMetadata annotation: name, protocol, machineType, icon, priority
│           ├── model/
│           │   ├── MachineStatusModel.java           ← record: id, name, state, timestamp
│           │   ├── CommandResult.java                ← record: success, message
│           │   ├── ComponentCardModel.java           ← record: name, icon, statusFields, actions
│           │   ├── AgvStatus.java                    ← record: position, battery, currentProgram, state
│           │   ├── AssemblyStatus.java               ← record: operation, progress, health, state
│           │   ├── Inventory.java                    ← record: tray list, slot occupancy
│           │   └── ProductionStatus.java             ← record: lineId, overall state, component statuses
│           ├── enums/
│           │   ├── MachineType.java                  ← AGV, ASSEMBLY_STATION, WAREHOUSE
│           │   ├── MachineStatus.java                ← IDLE, RUNNING, ERROR, OFFLINE
│           │   └── UserRole.java                     ← MANAGER, OPERATOR
│           └── event/
│               ├── MachineStatusChangedEvent.java    ← id, newStatus, timestamp
│               └── ComponentLoadedEvent.java         ← moduleId, metadata
│
│
│   # ================================================================
│   # INFRASTRUCTURE MODULES — each is a self-contained CBS component
│   # Each has its own module-info.java with provides...with
│   # Each has an internal/ package that is NOT exported
│   # ================================================================
│
├── infrastructure-agv/                               ← CBS component 1 — AGV (REST)
│   ├── pom.xml                                       ← depends on core only
│   └── src/main/java/
│       ├── module-info.java                          ← requires core; provides IAgvService + IMachineComponent + IComponentUIDescriptor
│       └── dk/sdu/infrastructure/agv/
│           ├── AgvService.java                       ← implements IAgvService + IMachineComponent + MachineLifecycle
│           ├── AgvUIDescriptor.java                  ← implements IComponentUIDescriptor — card layout/actions
│           └── internal/                             ← NOT in exports — JVM enforces invisibility
│               ├── AgvHttpClient.java                ← REST calls to http://localhost:8082/v1/*
│               ├── AgvStatusMapper.java              ← maps JSON response → AgvStatus
│               ├── AgvState.java                     ← internal state: position, battery, route
│               └── AgvConfig.java                    ← config: speed, max payload, home position
│
├── infrastructure-warehouse/                         ← CBS component 2 — Warehouse (SOAP)
│   ├── pom.xml                                       ← depends on core only
│   └── src/main/java/
│       ├── module-info.java                          ← requires core; provides IWarehouseService + IMachineComponent + IComponentUIDescriptor
│       └── dk/sdu/infrastructure/warehouse/
│           ├── WarehouseService.java                 ← implements IWarehouseService + IMachineComponent + MachineLifecycle
│           ├── WarehouseUIDescriptor.java            ← implements IComponentUIDescriptor
│           └── internal/                             ← NOT exported
│               ├── WarehouseSoapClient.java          ← SOAP calls to http://localhost:8081/Service.asmx
│               ├── InventoryMapper.java              ← maps SOAP XML → Inventory
│               ├── WarehouseState.java               ← internal: inventory levels, slot occupancy
│               └── WarehouseConfig.java              ← config: capacity, slot layout
│
├── infrastructure-assembly/                          ← CBS component 3 — Assembly Station (MQTT)
│   ├── pom.xml                                       ← depends on core only
│   └── src/main/java/
│       ├── module-info.java                          ← requires core; provides IAssemblyService + IMachineComponent + IComponentUIDescriptor
│       └── dk/sdu/infrastructure/assembly/
│           ├── AssemblyService.java                  ← implements IAssemblyService + IMachineComponent + MachineLifecycle
│           ├── AssemblyUIDescriptor.java             ← implements IComponentUIDescriptor
│           └── internal/                             ← NOT exported
│               ├── MqttClientWrapper.java            ← MQTT pub/sub to mqtt://localhost:1883
│               ├── AssemblyStatusMapper.java         ← maps MQTT messages → AssemblyStatus
│               ├── AssemblyState.java                ← internal: operation, progress, queue
│               └── AssemblyConfig.java               ← config: tools, cycle time
│
│
│   # ================================================================
│   # ORCHESTRATION — CBS coordination. Lean: only 2 files.
│   # Uses ServiceLoader to discover components — no manual wiring.
│   # ================================================================
│
├── orchestration/
│   ├── pom.xml                                       ← depends on core only
│   └── src/main/java/
│       ├── module-info.java                          ← requires core; uses IMachineComponent, IAgvService, IWarehouseService, IAssemblyService
│       └── dk/sdu/orchestration/
│           ├── ProductionOrchestrator.java           ← implements IProductionOrchestrator — runs the 16-step sequence
│           └── lifecycle/
│               └── ComponentLifecycleManager.java    ← Spring SmartLifecycle: iterates all MachineLifecycle and drives start/stop
│
│
│   # ================================================================
│   # SHARED — cross-cutting: DB entities, auth, production line mgmt
│   # Not a CBS component — just shared services.
│   # ================================================================
│
├── shared/
│   ├── pom.xml                                       ← depends on core, spring-data-jpa, postgresql
│   └── src/main/java/
│       ├── module-info.java                          ← requires core; exports entity, repository, service; opens entity to hibernate
│       └── dk/sdu/shared/
│           ├── entity/
│           │   ├── AppUser.java                      ← JPA @Entity, stores username, passwordHash, role, assignedLineIds
│           │   ├── ProductionLine.java               ← JPA @Entity: lineId, name, assignedComponentIds, status
│           │   ├── Formula.java                      ← JPA @Entity: name, required component types
│           │   └── TaskAssignment.java               ← JPA @Entity: operatorId, productionLineId
│           ├── repository/
│           │   ├── UserRepository.java               ← extends JpaRepository<AppUser, Long>
│           │   ├── ProductionLineRepository.java     ← extends JpaRepository<ProductionLine, Long>
│           │   └── FormulaRepository.java            ← extends JpaRepository<Formula, Long>
│           └── service/
│               ├── ProductionLineService.java        ← compose/start/stop production lines
│               ├── UserService.java                  ← register, assign, remove operators
│               └── FormulaService.java               ← CRUD product formulas
│
│
│   # ================================================================
│   # WEB — Spring Boot MVC + Thymeleaf host shell
│   # The only module that knows about Spring MVC / HTTP / WebSocket
│   # ================================================================
│
└── web/
    ├── pom.xml                                       ← depends on core, orchestration, shared, spring-boot-starter-*
    └── src/main/
        ├── java/
        │   ├── module-info.java                      ← requires core, orchestration, shared, spring.*; uses IMachineComponent + IComponentUIDescriptor
        │   └── dk/sdu/web/
        │       ├── SkateboardAsApplication.java      ← @SpringBootApplication entry point
        │       ├── config/
        │       │   ├── SecurityConfig.java           ← Spring Security: JWT filter, role-based access, CORS
        │       │   ├── ServiceLoaderConfig.java      ← bridges ServiceLoader discoveries into Spring DI
        │       │   └── WebSocketConfig.java          ← STOMP over SockJS endpoint registration
        │       ├── controller/
        │       │   ├── ProductionController.java     ← POST /api/production/start, /stop — delegates to IProductionOrchestrator
        │       │   ├── ComponentDiscoveryController.java ← GET /api/components — returns metadata from ServiceLoader.stream()
        │       │   ├── AuthController.java           ← POST /api/auth/login, /register — JWT issuance
        │       │   ├── ProductionLineController.java ← CRUD + start/stop production lines
        │       │   ├── UserController.java           ← manager: employee management
        │       │   └── FormulaController.java        ← CRUD for product formulas
        │       ├── websocket/
        │       │   └── ProductionWebSocketHandler.java ← pushes machine status updates to connected clients
        │       └── dto/
        │           ├── AgvStatusDto.java              ← never expose core models — always map to DTOs
        │           ├── WarehouseStatusDto.java
        │           ├── AssemblyStatusDto.java
        │           ├── ProductionStatusDto.java
        │           └── UserDto.java
        └── resources/
            ├── application.yml                       ← datasource, jwt secret, websocket config
            ├── templates/                            ← Thymeleaf server-rendered pages
            │   ├── login.html
            │   ├── manager/
            │   │   ├── dashboard.html                ← renders ALL discovered components as cards via metadata
            │   │   ├── production-line-editor.html   ← drag-and-drop: assign components to lines
            │   │   ├── employee-management.html      ← register, assign, remove operators
            │   │   └── formula-editor.html           ← create/edit component combinations
            │   ├── operator/
            │   │   ├── my-lines.html                 ← view assigned production lines
            │   │   └── line-control.html             ← start/stop/monitor a single line
            │   └── fragments/                        ← Thymeleaf reusable fragments (equivalent to Blazor UIComponents)
            │       ├── machine-card.html             ← GENERIC card — renders ANY machine from ComponentCardModel
            │       ├── production-line-card.html
            │       ├── drag-drop-zone.html
            │       └── status-indicator.html
            └── static/
                ├── css/
                │   └── app.css
                └── js/
                    ├── htmx.min.js                   ← HTMX: partial page updates without full SPA
                    ├── drag-drop.js                  ← HTML5 drag-and-drop for production line editor
                    └── websocket-client.js            ← SockJS + STOMP client for real-time status
```

---

## Module Declarations

### core/module-info.java
```java
module dk.sdu.core {
    // Exports everything — this is the shared contract layer
    exports dk.sdu.core.api;
    exports dk.sdu.core.lifecycle;
    exports dk.sdu.core.metadata;
    exports dk.sdu.core.model;
    exports dk.sdu.core.enums;
    exports dk.sdu.core.event;
    // No requires — zero dependencies
}
```

### infrastructure-agv/module-info.java
```java
module dk.sdu.infrastructure.agv {
    requires dk.sdu.core;
    requires java.net.http;                          // built-in REST client

    // ENCAPSULATION (Pillar 2): only this package is visible
    // dk.sdu.infrastructure.agv.internal is NOT listed → JVM blocks all access
    exports dk.sdu.infrastructure.agv;

    // EXPLICIT CONTRACTS (Pillar 3): what this module provides
    provides dk.sdu.core.api.IAgvService
        with dk.sdu.infrastructure.agv.AgvService;
    provides dk.sdu.core.api.IMachineComponent
        with dk.sdu.infrastructure.agv.AgvService;
    provides dk.sdu.core.api.IComponentUIDescriptor
        with dk.sdu.infrastructure.agv.AgvUIDescriptor;
}
```

### infrastructure-warehouse/module-info.java
```java
module dk.sdu.infrastructure.warehouse {
    requires dk.sdu.core;
    requires java.xml;                               // XML parsing for SOAP
    requires jakarta.xml.ws;                         // JAX-WS SOAP client

    exports dk.sdu.infrastructure.warehouse;
    // internal/ NOT exported — invisible

    provides dk.sdu.core.api.IWarehouseService
        with dk.sdu.infrastructure.warehouse.WarehouseService;
    provides dk.sdu.core.api.IMachineComponent
        with dk.sdu.infrastructure.warehouse.WarehouseService;
    provides dk.sdu.core.api.IComponentUIDescriptor
        with dk.sdu.infrastructure.warehouse.WarehouseUIDescriptor;
}
```

### infrastructure-assembly/module-info.java
```java
module dk.sdu.infrastructure.assembly {
    requires dk.sdu.core;
    requires com.hivemq.client.mqtt;                 // HiveMQ MQTT client

    exports dk.sdu.infrastructure.assembly;
    // internal/ NOT exported — invisible

    provides dk.sdu.core.api.IAssemblyService
        with dk.sdu.infrastructure.assembly.AssemblyService;
    provides dk.sdu.core.api.IMachineComponent
        with dk.sdu.infrastructure.assembly.AssemblyService;
    provides dk.sdu.core.api.IComponentUIDescriptor
        with dk.sdu.infrastructure.assembly.AssemblyUIDescriptor;
}
```

### orchestration/module-info.java
```java
module dk.sdu.orchestration {
    requires dk.sdu.core;

    // LATE BINDING (Pillar 4): declares intent to discover via ServiceLoader
    uses dk.sdu.core.api.IMachineComponent;
    uses dk.sdu.core.api.IAgvService;
    uses dk.sdu.core.api.IWarehouseService;
    uses dk.sdu.core.api.IAssemblyService;

    exports dk.sdu.orchestration;
    exports dk.sdu.orchestration.lifecycle;
}
```

### shared/module-info.java
```java
module dk.sdu.shared {
    requires dk.sdu.core;
    requires spring.data.jpa;
    requires jakarta.persistence;

    exports dk.sdu.shared.entity;
    exports dk.sdu.shared.repository;
    exports dk.sdu.shared.service;

    // JPA/Hibernate needs reflection access to entities
    opens dk.sdu.shared.entity to org.hibernate.orm.core, spring.core;
}
```

### web/module-info.java
```java
module dk.sdu.web {
    requires dk.sdu.core;
    requires dk.sdu.orchestration;
    requires dk.sdu.shared;

    requires spring.boot;
    requires spring.boot.autoconfigure;
    requires spring.web;
    requires spring.context;
    requires spring.security.core;
    requires spring.security.config;
    requires spring.security.web;
    requires spring.websocket;
    requires spring.messaging;

    // LATE BINDING (Pillar 4): web also discovers for the component discovery API
    uses dk.sdu.core.api.IMachineComponent;
    uses dk.sdu.core.api.IComponentUIDescriptor;

    // Spring needs reflection for DI, controllers, config
    opens dk.sdu.web to spring.core, spring.beans, spring.context;
    opens dk.sdu.web.controller to spring.web, spring.beans;
    opens dk.sdu.web.config to spring.beans, spring.context;
    opens dk.sdu.web.dto to com.fasterxml.jackson.databind;      // Jackson JSON serialization
}
```

---

## Module Dependencies — ENFORCED BY JVM

```
dk.sdu.core                     → (nothing — foundation)
dk.sdu.infrastructure.agv       → dk.sdu.core
dk.sdu.infrastructure.warehouse → dk.sdu.core
dk.sdu.infrastructure.assembly  → dk.sdu.core
dk.sdu.orchestration            → dk.sdu.core
dk.sdu.shared                   → dk.sdu.core
dk.sdu.web                      → dk.sdu.core + dk.sdu.orchestration + dk.sdu.shared

ENFORCED BY JVM: infrastructure modules CANNOT access each other
ENFORCED BY JVM: web CANNOT access internal packages in any module
ENFORCED BY JVM: missing requires → app refuses to start
ENFORCED BY JVM: missing provides → ServiceLoader.load() returns empty
```

---

## Key Contract Definitions

### MachineLifecycle.java (Pillar 5)
```java
package dk.sdu.core.lifecycle;

public interface MachineLifecycle {
    ComponentState getState();
    void start() throws Exception;
    void stop() throws Exception;
}
```

### ComponentState.java (Pillar 5)
```java
package dk.sdu.core.lifecycle;

public enum ComponentState {
    INSTALLED, STARTING, ACTIVE, STOPPING, UNINSTALLED
}
```

### IMachineComponent.java (Pillar 1 + 4)
```java
package dk.sdu.core.api;

import dk.sdu.core.enums.*;
import dk.sdu.core.model.*;

public interface IMachineComponent {
    String getId();
    String getName();
    MachineType getType();
    MachineStatus getStatus();
    CommandResult start();
    CommandResult stop();
    CommandResult reset();
    MachineStatusModel getStatusDetails();
}
```

### ComponentMetadata.java (annotation — Pillar 6)
```java
package dk.sdu.core.metadata;

import dk.sdu.core.enums.MachineType;
import java.lang.annotation.*;

@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface ComponentMetadata {
    String name();
    String protocol();
    MachineType machineType();
    String description() default "";
    String icon() default "";
    int priority() default 0;
}
```

### IComponentUIDescriptor.java
```java
package dk.sdu.core.api;

import dk.sdu.core.model.ComponentCardModel;
import java.util.List;

public interface IComponentUIDescriptor {
    String getComponentType();
    ComponentCardModel getCardModel();
    List<String> getAvailableActions();
    List<String> getDisplayedStatusFields();
}
```

### Core Models (use Java records)
```java
package dk.sdu.core.model;

import dk.sdu.core.enums.MachineStatus;
import java.time.Instant;

public record MachineStatusModel(String id, String name, MachineStatus status, Instant timestamp) {}
public record CommandResult(boolean success, String message) {}
public record ComponentCardModel(String name, String icon, List<String> statusFields, List<String> actions) {}
```

---

## Key Code Patterns

### Component Declaration (every infrastructure module follows this)
```java
package dk.sdu.infrastructure.agv;

import dk.sdu.core.api.*;
import dk.sdu.core.lifecycle.*;
import dk.sdu.core.metadata.*;
import dk.sdu.core.enums.*;
import dk.sdu.core.model.*;
import dk.sdu.infrastructure.agv.internal.AgvHttpClient;

@ComponentMetadata(
    name = "AGV Service",
    protocol = "REST",
    machineType = MachineType.AGV,
    description = "Automated Guided Vehicle for part transport",
    icon = "truck",
    priority = 0
)
public class AgvService implements IAgvService, IMachineComponent, MachineLifecycle {

    private final AgvHttpClient client = new AgvHttpClient();
    private ComponentState state = ComponentState.INSTALLED;
    private MachineStatus status = MachineStatus.OFFLINE;

    // MachineLifecycle
    @Override
    public void start() throws Exception {
        state = ComponentState.STARTING;
        client.verifyConnection();
        status = MachineStatus.IDLE;
        state = ComponentState.ACTIVE;
    }

    @Override
    public void stop() {
        state = ComponentState.STOPPING;
        status = MachineStatus.OFFLINE;
        state = ComponentState.UNINSTALLED;
    }

    @Override public ComponentState getState() { return state; }

    // IMachineComponent
    @Override public String getId() { return "agv-01"; }
    @Override public String getName() { return "AGV"; }
    @Override public MachineType getType() { return MachineType.AGV; }
    @Override public MachineStatus getStatus() { return status; }

    @Override
    public CommandResult start() {
        // delegates to lifecycle start()
        return new CommandResult(true, "AGV started");
    }

    @Override
    public CommandResult stop() {
        return new CommandResult(true, "AGV stopped");
    }

    @Override
    public CommandResult reset() {
        status = MachineStatus.IDLE;
        return new CommandResult(true, "AGV reset");
    }

    @Override
    public MachineStatusModel getStatusDetails() {
        return new MachineStatusModel(getId(), getName(), status, java.time.Instant.now());
    }

    // IAgvService-specific methods...
}
```

### ServiceLoaderConfig.java (bridges CBS → Spring)
```java
package dk.sdu.web.config;

import dk.sdu.core.api.*;
import dk.sdu.core.metadata.ComponentMetadata;
import org.springframework.context.annotation.*;
import java.util.*;

@Configuration
public class ServiceLoaderConfig {

    @Bean
    public ServiceLoader<IMachineComponent> machineComponents() {
        return ServiceLoader.load(IMachineComponent.class);
    }

    @Bean
    public List<ComponentMetadata> componentMetadataList() {
        return ServiceLoader.load(IMachineComponent.class)
            .stream()
            .map(p -> p.type().getAnnotation(ComponentMetadata.class))
            .filter(Objects::nonNull)
            .toList();
    }

    @Bean
    public IAgvService agvService() {
        return ServiceLoader.load(IAgvService.class).findFirst()
            .orElseThrow(() -> new IllegalStateException("AGV module not on module path"));
    }

    @Bean
    public IWarehouseService warehouseService() {
        return ServiceLoader.load(IWarehouseService.class).findFirst()
            .orElseThrow(() -> new IllegalStateException("Warehouse module not on module path"));
    }

    @Bean
    public IAssemblyService assemblyService() {
        return ServiceLoader.load(IAssemblyService.class).findFirst()
            .orElseThrow(() -> new IllegalStateException("Assembly module not on module path"));
    }
}
```

### ComponentDiscoveryController.java (metadata-driven UI — Pillar 6)
```java
package dk.sdu.web.controller;

import dk.sdu.core.api.IMachineComponent;
import dk.sdu.core.metadata.ComponentMetadata;
import org.springframework.web.bind.annotation.*;
import java.util.*;

@RestController
@RequestMapping("/api/components")
public class ComponentDiscoveryController {

    private final ServiceLoader<IMachineComponent> components;

    public ComponentDiscoveryController(ServiceLoader<IMachineComponent> components) {
        this.components = components;
    }

    @GetMapping
    public List<Map<String, Object>> getAll() {
        return components.stream()
            .map(provider -> {
                var meta = provider.type().getAnnotation(ComponentMetadata.class);
                if (meta == null) return null;
                return Map.<String, Object>of(
                    "name", meta.name(),
                    "machineType", meta.machineType().name(),
                    "protocol", meta.protocol(),
                    "description", meta.description(),
                    "icon", meta.icon()
                );
            })
            .filter(Objects::nonNull)
            .toList();
    }
}
```

### ProductionController.java
```java
package dk.sdu.web.controller;

import dk.sdu.core.api.IProductionOrchestrator;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/production")
public class ProductionController {

    private final IProductionOrchestrator orchestrator;

    public ProductionController(IProductionOrchestrator orchestrator) {
        this.orchestrator = orchestrator;
    }

    @PostMapping("/start")
    @PreAuthorize("hasAnyRole('OPERATOR', 'MANAGER')")
    public ResponseEntity<Void> start() {
        orchestrator.runProductionCycle();
        return ResponseEntity.ok().build();
    }

    @PostMapping("/stop")
    @PreAuthorize("hasAnyRole('OPERATOR', 'MANAGER')")
    public ResponseEntity<Void> stop() {
        orchestrator.stopProductionCycle();
        return ResponseEntity.ok().build();
    }
}
```

---

## Production Sequence

```
1.  warehouse.pickItem(trayId)
2.  agv.loadProgram("MoveToStorageOperation")
3.  agv.executeProgram()
4.  agv.loadProgram("PickWarehouseOperation")
5.  agv.executeProgram()
6.  agv.loadProgram("MoveToAssemblyOperation")
7.  agv.executeProgram()
8.  agv.loadProgram("PutAssemblyOperation")
9.  agv.executeProgram()
10. assembly.startOperation(processId)
11. assembly.checkHealth() → boolean isHealthy
12. agv.loadProgram("PickAssemblyOperation")
13. agv.executeProgram()
14. agv.loadProgram("MoveToStorageOperation")
15. agv.executeProgram()
16. if (isHealthy) → warehouse.insertItem(acceptedTrayId, "Accepted Product")
    else           → warehouse.insertItem(defectTrayId, "Defect Product")
```

---

## Docker Setup

```yaml
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

---

## Maven Dependencies (per module pom.xml)

### Parent POM
```xml
<properties>
    <java.version>21</java.version>
    <spring-boot.version>3.2.5</spring-boot.version>
</properties>
<modules>
    <module>core</module>
    <module>infrastructure-agv</module>
    <module>infrastructure-warehouse</module>
    <module>infrastructure-assembly</module>
    <module>orchestration</module>
    <module>shared</module>
    <module>web</module>
</modules>
```

| Module | Key Dependencies |
|---|---|
| core | *none* — pure Java |
| infrastructure-agv | `java.net.http` (built-in) |
| infrastructure-warehouse | `jakarta.xml.ws:jakarta.xml.ws-api`, `com.sun.xml.ws:jaxws-rt` |
| infrastructure-assembly | `com.hivemq:hivemq-mqtt-client` |
| orchestration | core only |
| shared | `spring-boot-starter-data-jpa`, `org.postgresql:postgresql` |
| web | `spring-boot-starter-web`, `spring-boot-starter-security`, `spring-boot-starter-thymeleaf`, `spring-boot-starter-websocket`, `io.jsonwebtoken:jjwt-api` + `jjwt-impl` + `jjwt-jackson`, `org.webjars.npm:htmx.org` |

---

## Auth Roles

- **MANAGER:** Full CRUD on production lines, formulas, employees. Sees all lines and components. Drag-and-drop composition. Registers new employees.
- **OPERATOR:** View and control only assigned production lines. Start/stop lines. Cannot modify composition or manage employees.

---

## Placeholder File Rules

Every `.java` stub must:
- Have a correct `package` declaration matching the folder path
- Contain a compilable class/interface/enum/record with `// TODO: Implement`
- Classes in `internal/` packages are package-private or public (doesn't matter — the module system hides the entire package)
- Use Java records for Core models where possible
- Enums must have their values defined (not empty)

---

## What NOT to Do

- **Never** add `requires dk.sdu.infrastructure.*` to another infrastructure module. The JVM enforces isolation.
- **Never** add `exports dk.sdu.infrastructure.*.internal` to any module-info.java. That destroys encapsulation.
- **Never** use `opens` on internal packages unless required by a framework (JPA, Jackson).
- **Never** hard-code machine types in the web or orchestration module. Use `ServiceLoader` + `@ComponentMetadata`.
- **Never** let controllers call machine services directly. Always go through `IProductionOrchestrator`.
- **Never** expose Core models from controllers. Map to DTOs in the web module.
- **Never** bypass `ServiceLoader` by manually instantiating implementations with `new`.
- **Never** add Spring dependencies to core or infrastructure modules. Only web and shared use Spring.
