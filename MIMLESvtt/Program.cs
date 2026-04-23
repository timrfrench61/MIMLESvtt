using MIMLESvtt.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIMLESvtt.src.Application.Authentication;
using MIMLESvtt.src;
using MIMLESvtt.src.Domain.Rules;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

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

builder.Services.AddScoped<VttSessionWorkspaceService>();
builder.Services.AddScoped<IVttSessionCommandService>(sp => sp.GetRequiredService<VttSessionWorkspaceService>());
builder.Services.AddScoped<IDiceRandomProvider, SystemDiceRandomProvider>();
builder.Services.AddScoped<IDiceRandomizationService, DiceRandomizationService>();

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
