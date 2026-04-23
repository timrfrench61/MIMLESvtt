using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules;

public class NoOpRulesActionModule : IRulesActionModule
{
    public RulesValidationResult ValidateAction(RulesContext context, RulesActionRequest request)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.ActionType))
        {
            return RulesValidationResult.Failure("ActionType is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ActorParticipantId))
        {
            return RulesValidationResult.Failure("ActorParticipantId is required.");
        }

        return RulesValidationResult.Success("No-op rules module accepted action.");
    }

    public RulesActionResolutionResult ResolveAction(RulesContext context, RulesActionRequest request)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(request);

        return RulesActionResolutionResult.Success(new ActionRequest
        {
            ActionType = request.ActionType,
            ActorParticipantId = request.ActorParticipantId,
            Payload = request.Payload
        }, "No-op resolution returned engine action request.");
    }
}
