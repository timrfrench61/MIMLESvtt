namespace MIMLESvtt.src.Domain.Rules;

public interface IRulesActionModule
{
    RulesValidationResult ValidateAction(RulesContext context, RulesActionRequest request);

    RulesActionResolutionResult ResolveAction(RulesContext context, RulesActionRequest request);
}
