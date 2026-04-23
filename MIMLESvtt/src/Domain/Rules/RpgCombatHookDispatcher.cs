using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules;

public class RpgCombatHookDispatcher : IRpgCombatHookDispatcher
{
    private readonly List<RpgCombatHookRegistration> _registrations = [];
    private readonly List<ObjectiveVictoryHookPayload> _evaluationEvents = [];

    public IReadOnlyList<ObjectiveVictoryHookPayload> EvaluationEvents => _evaluationEvents;

    public void RegisterHook(RpgCombatHookRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        ArgumentNullException.ThrowIfNull(registration.HookModule);

        _registrations.Add(registration);
    }

    public RpgCombatHookDispatchResult DispatchTurnStart(RulesContext context, RulesActionRequest request)
    {
        return Dispatch(registration => registration.HookModule.OnTurnStart(context, request), "Turn Start");
    }

    public RpgCombatHookDispatchResult DispatchActionValidation(RulesContext context, RulesActionRequest request)
    {
        return Dispatch(registration => registration.HookModule.OnActionValidation(context, request), "Action Validation");
    }

    public RpgCombatHookDispatchResult DispatchActionResolution(RulesContext context, RulesActionRequest request)
    {
        return Dispatch(registration => registration.HookModule.OnActionResolution(context, request), "Action Resolution");
    }

    public RpgCombatHookDispatchResult DispatchOutcomeApplication(RulesContext context, RulesActionRequest request, RulesActionResolutionResult resolution)
    {
        return Dispatch(registration => registration.HookModule.OnOutcomeApplication(context, request, resolution), "Outcome Application");
    }

    public RpgCombatHookDispatchResult DispatchTurnPhaseAdvancement(RulesContext context, RulesActionRequest request, ActionRecord appliedAction)
    {
        return Dispatch(registration => registration.HookModule.OnTurnPhaseAdvancement(context, request, appliedAction), "Turn/Phase Advancement");
    }

    public RpgCombatHookDispatchResult DispatchObjectiveVictoryCheck(RulesContext context, RulesActionRequest request, ActionRecord appliedAction)
    {
        return Dispatch(registration => registration.HookModule.OnObjectiveVictoryCheck(context, request, appliedAction), "Objective/Victory Check");
    }

    public RpgCombatHookDispatchResult DispatchObjectiveVictoryStateChangeEvaluation(RulesContext context, RulesActionRequest request, ActionRecord appliedAction)
    {
        return Dispatch(registration => registration.HookModule.OnObjectiveVictoryStateChangeEvaluation(context, request, appliedAction), "Objective/Victory State Change");
    }

    public RpgCombatHookDispatchResult DispatchObjectiveVictoryExplicitEvaluation(RulesContext context, RulesActionRequest request)
    {
        return Dispatch(registration => registration.HookModule.OnObjectiveVictoryExplicitEvaluation(context, request), "Objective/Victory Explicit");
    }

    private RpgCombatHookDispatchResult Dispatch(Func<RpgCombatHookRegistration, RpgCombatHookDispatchResult> invoke, string stage)
    {
        foreach (var registration in _registrations)
        {
            var result = invoke(registration);
            if (result.ObjectiveVictoryPayload.ObjectiveResults.Count > 0 || result.ObjectiveVictoryPayload.VictoryResult.IsTerminal)
            {
                _evaluationEvents.Add(result.ObjectiveVictoryPayload);
            }

            if (!result.IsAllowed)
            {
                var message = string.IsNullOrWhiteSpace(result.Message)
                    ? $"Combat hook '{stage}' rejected action."
                    : result.Message;

                return RpgCombatHookDispatchResult.Reject(message);
            }
        }

        return RpgCombatHookDispatchResult.Allow();
    }
}
