using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules;

public class NoOpRpgCombatHookModule : IRpgCombatHookModule
{
    public RpgCombatHookDispatchResult OnTurnStart(RulesContext context, RulesActionRequest request) => RpgCombatHookDispatchResult.Allow();

    public RpgCombatHookDispatchResult OnActionValidation(RulesContext context, RulesActionRequest request) => RpgCombatHookDispatchResult.Allow();

    public RpgCombatHookDispatchResult OnActionResolution(RulesContext context, RulesActionRequest request) => RpgCombatHookDispatchResult.Allow();

    public RpgCombatHookDispatchResult OnOutcomeApplication(RulesContext context, RulesActionRequest request, RulesActionResolutionResult resolution) => RpgCombatHookDispatchResult.Allow();

    public RpgCombatHookDispatchResult OnTurnPhaseAdvancement(RulesContext context, RulesActionRequest request, ActionRecord appliedAction) => RpgCombatHookDispatchResult.Allow();

    public RpgCombatHookDispatchResult OnObjectiveVictoryCheck(RulesContext context, RulesActionRequest request, ActionRecord appliedAction) => RpgCombatHookDispatchResult.Allow();

    public RpgCombatHookDispatchResult OnObjectiveVictoryStateChangeEvaluation(RulesContext context, RulesActionRequest request, ActionRecord appliedAction) => RpgCombatHookDispatchResult.Allow();

    public RpgCombatHookDispatchResult OnObjectiveVictoryExplicitEvaluation(RulesContext context, RulesActionRequest request) => RpgCombatHookDispatchResult.Allow();
}
