using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MIMLESvtt.Services.Authentication;

public class AuthDataSeeder
{
    private const string AdminRoleName = "Admin";
    private readonly IServiceProvider _serviceProvider;

    public AuthDataSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync(AdminRoleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Unable to create '{AdminRoleName}' role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AuthUser>>();
        var defaults = scope.ServiceProvider.GetRequiredService<IOptions<AuthSeedDefaults>>().Value;

        var user = await userManager.FindByNameAsync(defaults.UserName);
        if (user is null)
        {
            user = new AuthUser
            {
                UserName = defaults.UserName,
                Email = defaults.Email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, defaults.Password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Unable to create default admin user '{defaults.UserName}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, AdminRoleName))
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, AdminRoleName);
            if (!addRoleResult.Succeeded)
            {
                throw new InvalidOperationException($"Unable to grant '{AdminRoleName}' role to '{defaults.UserName}': {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
