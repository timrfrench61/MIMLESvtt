using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules;

public interface IRpgCombatHookModule
{
    RpgCombatHookDispatchResult OnTurnStart(RulesContext context, RulesActionRequest request);

    RpgCombatHookDispatchResult OnActionValidation(RulesContext context, RulesActionRequest request);

    RpgCombatHookDispatchResult OnActionResolution(RulesContext context, RulesActionRequest request);

    RpgCombatHookDispatchResult OnOutcomeApplication(RulesContext context, RulesActionRequest request, RulesActionResolutionResult resolution);

    RpgCombatHookDispatchResult OnTurnPhaseAdvancement(RulesContext context, RulesActionRequest request, ActionRecord appliedAction);

    RpgCombatHookDispatchResult OnObjectiveVictoryCheck(RulesContext context, RulesActionRequest request, ActionRecord appliedAction);

    RpgCombatHookDispatchResult OnObjectiveVictoryStateChangeEvaluation(RulesContext context, RulesActionRequest request, ActionRecord appliedAction);

    RpgCombatHookDispatchResult OnObjectiveVictoryExplicitEvaluation(RulesContext context, RulesActionRequest request);
}
