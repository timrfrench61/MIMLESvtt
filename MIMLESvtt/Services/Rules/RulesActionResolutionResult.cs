using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules;

public class RulesActionResolutionResult
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public ActionRequest? EngineActionRequest { get; init; }

    public bool ShouldAdvanceTurn { get; init; }

    public List<string> Diagnostics { get; init; } = [];

    public static RulesActionResolutionResult Success(ActionRequest engineActionRequest, params string[] diagnostics)
    {
        return new RulesActionResolutionResult
        {
            IsSuccess = true,
            EngineActionRequest = engineActionRequest,
            Diagnostics = diagnostics?.ToList() ?? []
        };
    }

    public static RulesActionResolutionResult Failure(string message, params string[] diagnostics)
    {
        return new RulesActionResolutionResult
        {
            IsSuccess = false,
            Message = message,
            Diagnostics = diagnostics?.ToList() ?? []
        };
    }
}
