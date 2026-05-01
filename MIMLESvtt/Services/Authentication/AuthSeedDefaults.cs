namespace MIMLESvtt.Services.Authentication;

public sealed class AuthSeedDefaults
{
    public const string SectionName = "Authentication:DefaultAdmin";

    public string UserName { get; set; } = "admin";

    public string Email { get; set; } = "admin@local";

    public string Password { get; set; } = "admin123";
}
