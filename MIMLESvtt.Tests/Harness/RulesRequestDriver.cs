using MIMLESvtt.src;
using MIMLESvtt.src.Domain.Rules;

namespace MIMLESvtt.Tests.Harness;

public class RulesRequestDriver
{
    public RulesValidationResult Validate(IRulesActionModule module, RulesContext context, RulesActionRequest request)
    {
        return module.ValidateAction(context, request);
    }

    public RulesActionResolutionResult Resolve(IRulesActionModule module, RulesContext context, RulesActionRequest request)
    {
        return module.ResolveAction(context, request);
    }

    public RpgCombatHookDispatchResult TriggerObjectiveEvaluation(
        IRpgCombatHookModule hookModule,
        RulesContext context,
        RulesActionRequest request,
        ObjectiveVictoryTriggerKind triggerKind,
        ActionRecord? action = null)
    {
        var applied = action ?? new ActionRecord
        {
            Id = Guid.NewGuid().ToString("N"),
            ActionType = request.ActionType,
            ActorParticipantId = request.ActorParticipantId,
            TimestampUtc = DateTime.UtcNow,
            Payload = request.Payload
        };

        return triggerKind switch
        {
            ObjectiveVictoryTriggerKind.StateChange => hookModule.OnObjectiveVictoryStateChangeEvaluation(context, request, applied),
            ObjectiveVictoryTriggerKind.TurnPhaseProgression => hookModule.OnObjectiveVictoryCheck(context, request, applied),
            _ => hookModule.OnObjectiveVictoryExplicitEvaluation(context, request)
        };
    }
}
