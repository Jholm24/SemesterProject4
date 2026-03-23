using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orchestration.Lifecycle;
using Orchestration.Loading;
using Orchestration.Validation;
using Shared.Data;
using Shared.Identity;
using SkateboardAS.Components;

var builder = WebApplication.CreateBuilder(args);

// Layer 1 — AssemblyLoadContext isolation
// Note: plugins are loaded from the plugins/ directory at runtime
// var loader = new ComponentLoader();
// var agvAssembly = loader.LoadComponent("plugins/Infrastructure.Agv/Infrastructure.Agv.dll");
// var warehouseAssembly = loader.LoadComponent("plugins/Infrastructure.Warehouse/Infrastructure.Warehouse.dll");
// var assemblyAssembly = loader.LoadComponent("plugins/Infrastructure.Assembly/Infrastructure.Assembly.dll");

// Layer 2 — MEF composition (uncomment when plugins are built)
// var mefContainer = new MefCompositionBuilder()
//     .AddAssembly(agvAssembly)
//     .AddAssembly(warehouseAssembly)
//     .AddAssembly(assemblyAssembly)
//     .Build();
// ContractValidator.Validate(mefContainer);

// Layer 3 — ASP.NET Core DI
// builder.Services.AddSingleton(mefContainer.GetExport<IAgvService>());
// builder.Services.AddSingleton(mefContainer.GetExport<IWarehouseService>());
// builder.Services.AddSingleton(mefContainer.GetExport<IAssemblyService>());
// builder.Services.AddScoped<IProductionOrchestrator, ProductionOrchestrator>();
// builder.Services.AddHostedService<ComponentLifecycleManager>();

// Database
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
