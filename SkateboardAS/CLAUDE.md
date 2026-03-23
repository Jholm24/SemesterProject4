# SkateboardAS — Claude Code Instructions

## Project Overview
ASP.NET Core 9 Blazor Server application. UI components live under `Components/`.

## Tech Stack
- **Framework**: .NET 9, ASP.NET Core, Blazor Server (Interactive Server render mode)
- **UI**: Razor components (`.razor`)
- **Project file**: `SkateboardAS.csproj`

## Build & Run
```bash
dotnet build
dotnet run
```

## Key Conventions
- Razor components go in `Components/Pages/` (pages) or `Components/` (shared)
- Keep `Program.cs` minimal — wire up services and middleware only
- Use `appsettings.json` for shared config; `appsettings.Development.json` is gitignored (local only)
- No secrets in committed files — use `dotnet user-secrets` or environment variables

## Do / Don't
- **Do** enable nullable reference types (`<Nullable>enable</Nullable>` is set)
- **Do** use implicit usings (already enabled)
- **Don't** commit `appsettings.Development.json` or `.env.*` files
- **Don't** add NuGet packages without checking existing ones in `.csproj` first
