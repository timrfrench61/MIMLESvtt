using MIMLESvtt.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIMLESvtt.Services.Authentication;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;
using MIMLESvtt.Services.Actions;
using MIMLESvtt.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var authConnectionString = builder.Configuration.GetConnectionString("AuthDatabase")
    ?? "Data Source=App_Data/mimlesvtt-auth.db";

EnsureSqliteDirectoryExists(authConnectionString);

builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlite(authConnectionString));
builder.Services.Configure<AuthSeedDefaults>(builder.Configuration.GetSection(AuthSeedDefaults.SectionName));

builder.Services
    .AddIdentityCore<AuthUser>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthDataSeeder>();

builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var configuredDataRoot = configuration["MimlesData:RootPath"];
    var dataRootPath = string.IsNullOrWhiteSpace(configuredDataRoot)
        ? Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "mimles_data"))
        : Path.GetFullPath(
            Path.IsPathRooted(configuredDataRoot)
                ? configuredDataRoot
                : Path.Combine(builder.Environment.ContentRootPath, configuredDataRoot));

    return new VttSessionWorkspaceService(dataRootPath);
});
builder.Services.AddScoped<ISessionCommandService>(sp => sp.GetRequiredService<VttSessionWorkspaceService>());
builder.Services.AddScoped<LoadGameService>();
builder.Services.AddScoped<SaveGameService>();
builder.Services.AddScoped<ModuleImportService>();
builder.Services.AddScoped<WorkspaceSetupOptionsService>();
builder.Services.AddScoped<SettingsPreferenceService>();
builder.Services.AddScoped<ContentImportWorkflowService>();
builder.Services.AddScoped<ContentManualEntryWorkflowService>();
builder.Services.AddScoped<CheckersMoveValidationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/auth/login", async (
    HttpContext httpContext,
    [FromForm] string userName,
    [FromForm] string password,
    [FromForm] string? returnUrl,
    SignInManager<AuthUser> signInManager) =>
{
    var signInResult = await signInManager.PasswordSignInAsync(userName, password, isPersistent: true, lockoutOnFailure: false);
    if (signInResult.Succeeded)
    {
        return Results.LocalRedirect(NormalizeReturnUrl(returnUrl));
    }

    var encodedReturnUrl = Uri.EscapeDataString(NormalizeReturnUrl(returnUrl));
    return Results.LocalRedirect($"/login?error=Invalid%20username%20or%20password&returnUrl={encodedReturnUrl}");
}).DisableAntiforgery();

app.MapPost("/auth/logout", async (
    [FromForm] string? returnUrl,
    SignInManager<AuthUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect(NormalizeReturnUrl(returnUrl));
}).DisableAntiforgery();

using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<AuthDataSeeder>();
    await dataSeeder.SeedAsync();
}

app.Run();

static string NormalizeReturnUrl(string? returnUrl)
{
    if (string.IsNullOrWhiteSpace(returnUrl))
    {
        return "/";
    }

    if (!returnUrl.StartsWith("/", StringComparison.Ordinal)
        || returnUrl.StartsWith("//", StringComparison.Ordinal)
        || returnUrl.StartsWith("/\\", StringComparison.Ordinal))
    {
        return "/";
    }

    return returnUrl;
}

static void EnsureSqliteDirectoryExists(string connectionString)
{
    const string dataSourcePrefix = "Data Source=";
    var index = connectionString.IndexOf(dataSourcePrefix, StringComparison.OrdinalIgnoreCase);
    if (index < 0)
    {
        return;
    }

    var databasePath = connectionString[(index + dataSourcePrefix.Length)..].Trim();
    if (string.IsNullOrWhiteSpace(databasePath))
    {
        return;
    }

    var directoryPath = Path.GetDirectoryName(databasePath);
    if (string.IsNullOrWhiteSpace(directoryPath))
    {
        return;
    }

    Directory.CreateDirectory(directoryPath);
}
