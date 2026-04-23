using MIMLESvtt.src;
using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules;

public class RulesActionOrchestrator
{
    private readonly IRulesActionModule _rulesActionModule;
    private readonly IRpgCombatHookDispatcher _combatHookDispatcher;

    public RulesActionOrchestrator(IRulesActionModule rulesActionModule)
        : this(rulesActionModule, new RpgCombatHookDispatcher())
    {
    }

    public RulesActionOrchestrator(IRulesActionModule rulesActionModule, IRpgCombatHookDispatcher combatHookDispatcher)
    {
        _rulesActionModule = rulesActionModule ?? throw new ArgumentNullException(nameof(rulesActionModule));
        _combatHookDispatcher = combatHookDispatcher ?? throw new ArgumentNullException(nameof(combatHookDispatcher));
    }

    public ActionRecord Process(IVttSessionCommandService commandService, string moduleId, string moduleVersion, string scenarioMetadata, ActionRequest actionRequest)
    {
        ArgumentNullException.ThrowIfNull(commandService);
        ArgumentNullException.ThrowIfNull(actionRequest);

        if (commandService.CurrentVttSession is null)
        {
            throw new InvalidOperationException("CurrentVttSession is required to process rules action.");
        }

        var context = BuildContext(commandService.CurrentVttSession, moduleId, moduleVersion, scenarioMetadata, actionRequest.ActorParticipantId);
        var rulesRequest = new RulesActionRequest
        {
            ActionType = actionRequest.ActionType,
            ActorParticipantId = actionRequest.ActorParticipantId,
            Payload = actionRequest.Payload
        };

        EnsureAllowed(_combatHookDispatcher.DispatchTurnStart(context, rulesRequest));
        EnsureAllowed(_combatHookDispatcher.DispatchActionValidation(context, rulesRequest));

        var validation = _rulesActionModule.ValidateAction(context, rulesRequest);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(validation.Message)
                ? "Rules module rejected action."
                : validation.Message);
        }

        EnsureAllowed(_combatHookDispatcher.DispatchActionResolution(context, rulesRequest));

        var outcome = _rulesActionModule.ResolveAction(context, rulesRequest);
        if (!outcome.IsSuccess || outcome.EngineActionRequest is null)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(outcome.Message)
                ? "Rules module failed to resolve action."
                : outcome.Message);
        }

        EnsureAllowed(_combatHookDispatcher.DispatchOutcomeApplication(context, rulesRequest, outcome));

        var appliedAction = commandService.ProcessAction(outcome.EngineActionRequest);

        EnsureAllowed(_combatHookDispatcher.DispatchObjectiveVictoryStateChangeEvaluation(context, rulesRequest, appliedAction));

        EnsureAllowed(_combatHookDispatcher.DispatchTurnPhaseAdvancement(context, rulesRequest, appliedAction));
        EnsureAllowed(_combatHookDispatcher.DispatchObjectiveVictoryCheck(context, rulesRequest, appliedAction));
        EnsureAllowed(_combatHookDispatcher.DispatchObjectiveVictoryExplicitEvaluation(context, rulesRequest));

        return appliedAction;
    }

    private static void EnsureAllowed(RpgCombatHookDispatchResult hookResult)
    {
        if (hookResult.IsAllowed)
        {
            return;
        }

        var message = string.IsNullOrWhiteSpace(hookResult.Message)
            ? "Combat hook rejected action."
            : hookResult.Message;

        throw new InvalidOperationException(message);
    }

    private static RulesContext BuildContext(
        VttSession session,
        string moduleId,
        string moduleVersion,
        string scenarioMetadata,
        string actorParticipantId)
    {
        return new RulesContext
        {
            CurrentSession = session,
            CurrentTurnNumber = session.TurnNumber,
            CurrentTurnIndex = session.CurrentTurnIndex,
            CurrentPhase = session.CurrentPhase,
            ActorParticipantId = actorParticipantId,
            SelectedRulesModuleId = moduleId ?? string.Empty,
            SelectedRulesModuleVersion = moduleVersion ?? string.Empty,
            ScenarioMetadata = scenarioMetadata ?? string.Empty,
            EnableStrictValidation = true
        };
    }
}
