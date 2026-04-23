using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules;

public interface IRpgCombatHookDispatcher
{
    void RegisterHook(RpgCombatHookRegistration registration);

    RpgCombatHookDispatchResult DispatchTurnStart(RulesContext context, RulesActionRequest request);

    RpgCombatHookDispatchResult DispatchActionValidation(RulesContext context, RulesActionRequest request);

    RpgCombatHookDispatchResult DispatchActionResolution(RulesContext context, RulesActionRequest request);

    RpgCombatHookDispatchResult DispatchOutcomeApplication(RulesContext context, RulesActionRequest request, RulesActionResolutionResult resolution);

    RpgCombatHookDispatchResult DispatchTurnPhaseAdvancement(RulesContext context, RulesActionRequest request, ActionRecord appliedAction);

    RpgCombatHookDispatchResult DispatchObjectiveVictoryCheck(RulesContext context, RulesActionRequest request, ActionRecord appliedAction);

    RpgCombatHookDispatchResult DispatchObjectiveVictoryStateChangeEvaluation(RulesContext context, RulesActionRequest request, ActionRecord appliedAction);

    RpgCombatHookDispatchResult DispatchObjectiveVictoryExplicitEvaluation(RulesContext context, RulesActionRequest request);
}
