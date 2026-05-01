namespace MIMLESvtt.src.Domain.Rules;

public class RulesValidationResult
{
    public bool IsValid { get; init; }

    public string Message { get; init; } = string.Empty;

    public List<string> Notes { get; init; } = [];

    public static RulesValidationResult Success(params string[] notes)
    {
        return new RulesValidationResult
        {
            IsValid = true,
            Message = string.Empty,
            Notes = notes?.ToList() ?? []
        };
    }

    public static RulesValidationResult Failure(string message, params string[] notes)
    {
        return new RulesValidationResult
        {
            IsValid = false,
            Message = message,
            Notes = notes?.ToList() ?? []
        };
    }
}
