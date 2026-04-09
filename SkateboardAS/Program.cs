using Core.Interfaces;
using Core.Lifecycle;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orchestration;
using Orchestration.Lifecycle;
using Orchestration.Loading;
using Orchestration.Validation;
using Shared.Data;
using Shared.Identity;
using Shared.Repositories;
using Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using SkateboardAS.Components;
using SkateboardAS.Hubs;
using SkateboardAS.Services;

var builder = WebApplication.CreateBuilder(args);

// Layer 1 + 2 — Plugin loading via AssemblyLoadContext + MEF composition
var pluginPaths = new[]
{
    "plugins/Infrastructure.Agv/Infrastructure.Agv.dll",
    "plugins/Infrastructure.Warehouse/Infrastructure.Warehouse.dll",
    "plugins/Infrastructure.Assembly/Infrastructure.Assembly.dll"
};

var pluginsAvailable = pluginPaths.All(File.Exists);

if (pluginsAvailable)
{
    try
    {
        var loader = new ComponentLoader();
        var assemblies = pluginPaths.Select(p => loader.LoadComponent(p)).ToList();

        var mefContainer = new MefCompositionBuilder()
            .AddAssembly(assemblies[0])
            .AddAssembly(assemblies[1])
            .AddAssembly(assemblies[2])
            .Build();

        ContractValidator.Validate(mefContainer);

        // Layer 3 — Register plugin services into ASP.NET Core DI
        var agvService = mefContainer.GetExport<IAgvService>();
        var warehouseService = mefContainer.GetExport<IWarehouseService>();
        var assemblyService = mefContainer.GetExport<IAssemblyService>();

        builder.Services.AddSingleton(agvService);
        builder.Services.AddSingleton(warehouseService);
        builder.Services.AddSingleton(assemblyService);
        builder.Services.AddScoped<IProductionOrchestrator, ProductionOrchestrator>();

        var components = mefContainer.GetExports<IComponent>();
        foreach (var component in components)
            builder.Services.AddSingleton(component);

        var machineComponents = mefContainer.GetExports<IMachineComponent>();
        foreach (var mc in machineComponents)
            builder.Services.AddSingleton<IMachineComponent>(mc);

        builder.Services.AddHostedService<ComponentLifecycleManager>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[WARNING] Plugin loading failed: {ex.Message}. Starting without plugins.");
    }
}
else
{
    Console.WriteLine("[WARNING] Plugin DLLs not found. Starting in development mode without plugins.");
}

// Database
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (needed for UserRepository / UserManager)
builder.Services.AddIdentityCore<AppUser>(options => { })
    .AddRoles<IdentityRole>()
    .AddSignInManager()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Repositories
builder.Services.AddScoped<ProductionLineRepository>();
builder.Services.AddScoped<FormulaRepository>();
builder.Services.AddScoped<UserRepository>();

// Services
builder.Services.AddScoped<ProductionLineService>();
builder.Services.AddScoped<FormulaService>();
builder.Services.AddScoped<UserService>();

// Auth state for Blazor — role switcher (no login required)
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<RoleState>();
builder.Services.AddScoped<DevAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<DevAuthStateProvider>());
builder.Services.AddHttpClient();
builder.Services.AddScoped<ApiClient>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MVC Controllers
builder.Services.AddControllers();

// SignalR
builder.Services.AddSignalR();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapControllers();
app.MapHub<ProductionHub>("/hubs/production");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
