using Microsoft.AspNetCore.Components;

namespace MIMLESvtt.Components.Pages;

public partial class Login : ComponentBase
{
    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery(Name = "error")]
    public string? Error { get; set; }

    private string userName = "admin";
    private string resolvedReturnUrl = "/";
    private string? errorMessage;

    protected override void OnParametersSet()
    {
        resolvedReturnUrl = NormalizeReturnUrl(ReturnUrl);
        errorMessage = string.IsNullOrWhiteSpace(Error) ? null : Uri.UnescapeDataString(Error);
    }

    private static string NormalizeReturnUrl(string? returnUrl)
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
}
