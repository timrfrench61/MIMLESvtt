using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Rules;

namespace MIMLESvtt.Tests;

[TestClass]
public class RulesActionOrchestratorTests
{
    [TestMethod]
    public void Process_WhenValidationFails_RejectsActionAndDoesNotMutateSession()
    {
        var session = new VttSession
        {
            Id = "session-1",
            Title = "Rules Test Session"
        };

        var commandService = new RecordingCommandService(session);
        var orchestrator = new RulesActionOrchestrator(new RejectingRulesModule("Action rejected by rules."));
        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "player-1",
            Payload = new { PieceId = "piece-1" }
        };

        var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            orchestrator.Process(commandService, "checkers-ref", "v1", "scenario-a", request));

        Assert.AreEqual("Action rejected by rules.", ex.Message);
        Assert.AreEqual(0, commandService.Requests.Count);
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WhenRulesResolveSuccessfully_HandsOffOutcomeToEngineApplyAndLogFlow()
    {
        var session = new VttSession
        {
            Id = "session-2",
            Title = "Rules Success Session"
        };

        var commandService = new RecordingCommandService(session);
        var orchestrator = new RulesActionOrchestrator(new NoOpRulesActionModule());
        var request = new ActionRequest
        {
            ActionType = "ChangePieceState",
            ActorParticipantId = "gm",
            Payload = new { PieceId = "piece-5", Key = "state", Value = "active" }
        };

        var record = orchestrator.Process(commandService, "checkers-ref", "v1", "scenario-b", request);

        Assert.AreEqual(1, commandService.Requests.Count);
        Assert.AreEqual("ChangePieceState", commandService.Requests[0].ActionType);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(record.Id, session.ActionLog[0].Id);
        Assert.AreEqual("ChangePieceState", session.ActionLog[0].ActionType);
    }

    [TestMethod]
    public void Process_InvokesCombatHooks_InExpectedOrder()
    {
        var session = new VttSession
        {
            Id = "session-3",
            Title = "Hook Order Session"
        };

        var commandService = new RecordingCommandService(session);
        var hookDispatcher = new RpgCombatHookDispatcher();
        var recordingHook = new RecordingHookModule();
        hookDispatcher.RegisterHook(new RpgCombatHookRegistration
        {
            ModuleId = "combat-hooks",
            ModuleVersion = "v1",
            HookModule = recordingHook
        });

        var orchestrator = new RulesActionOrchestrator(new NoOpRulesActionModule(), hookDispatcher);
        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "player-1",
            Payload = new { PieceId = "piece-1" }
        };

        orchestrator.Process(commandService, "checkers-ref", "v1", "scenario-hook-order", request);

        CollectionAssert.AreEqual(
            new[]
            {
                "TurnStart",
                "ActionValidation",
                "ActionResolution",
                "OutcomeApplication",
                "ObjectiveVictoryStateChange",
                "TurnPhaseAdvancement",
                "ObjectiveVictoryCheck",
                "ObjectiveVictoryExplicit"
            },
            recordingHook.Calls);
    }

    [TestMethod]
    public void Process_WhenHookRejectsAtValidation_ShortCircuitsAndDoesNotApplyOrLog()
    {
        var session = new VttSession
        {
            Id = "session-4",
            Title = "Hook Reject Session"
        };

        var commandService = new RecordingCommandService(session);
        var hookDispatcher = new RpgCombatHookDispatcher();
        hookDispatcher.RegisterHook(new RpgCombatHookRegistration
        {
            ModuleId = "combat-hooks",
            ModuleVersion = "v1",
            HookModule = new RejectingAtValidationHookModule("Combat validation hook rejected action.")
        });

        var orchestrator = new RulesActionOrchestrator(new NoOpRulesActionModule(), hookDispatcher);
        var request = new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "player-2",
            Payload = new { PieceId = "piece-3" }
        };

        var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            orchestrator.Process(commandService, "checkers-ref", "v1", "scenario-hook-reject", request));

        Assert.AreEqual("Combat validation hook rejected action.", ex.Message);
        Assert.AreEqual(0, commandService.Requests.Count);
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_ObjectiveVictoryEvaluation_RecordsLayeredOutcomeEvents()
    {
        var session = new VttSession
        {
            Id = "session-5",
            Title = "Objective Evaluation Session"
        };

        var commandService = new RecordingCommandService(session);
        var hookDispatcher = new RpgCombatHookDispatcher();
        var recordingHook = new RecordingHookModule();
        hookDispatcher.RegisterHook(new RpgCombatHookRegistration
        {
            ModuleId = "combat-hooks",
            ModuleVersion = "v1",
            HookModule = recordingHook
        });

        var orchestrator = new RulesActionOrchestrator(new NoOpRulesActionModule(), hookDispatcher);
        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "player-1",
            Payload = new { PieceId = "piece-1" }
        };

        orchestrator.Process(commandService, "checkers-ref", "v1", "scenario-objective", request);

        Assert.IsTrue(hookDispatcher.EvaluationEvents.Count >= 2);
        var terminal = hookDispatcher.EvaluationEvents.LastOrDefault(e => e.VictoryResult.IsTerminal);
        Assert.IsNotNull(terminal);
        Assert.AreEqual("player-1", terminal!.VictoryResult.WinnerParticipantId);
        Assert.AreEqual("LayeredObjective", terminal.VictoryResult.VictoryType);
        CollectionAssert.AreEqual(new[] { "obj-hold-a", "obj-eliminate-b" }, terminal.VictoryResult.SatisfiedConditions);
    }

    private sealed class RejectingRulesModule : IRulesActionModule
    {
        private readonly string _message;

        public RejectingRulesModule(string message)
        {
            _message = message;
        }

    private sealed class RecordingHookModule : IRpgCombatHookModule
    {
        public List<string> Calls { get; } = [];

        public RpgCombatHookDispatchResult OnTurnStart(RulesContext context, RulesActionRequest request)
        {
            Calls.Add("TurnStart");
            return RpgCombatHookDispatchResult.Allow();
        }

        public RpgCombatHookDispatchResult OnActionValidation(RulesContext context, RulesActionRequest request)
        {
            Calls.Add("ActionValidation");
            return RpgCombatHookDispatchResult.Allow();
        }

        public RpgCombatHookDispatchResult OnActionResolution(RulesContext context, RulesActionRequest request)
        {
            Calls.Add("ActionResolution");
            return RpgCombatHookDispatchResult.Allow();
        }

        public RpgCombatHookDispatchResult OnOutcomeApplication(RulesContext context, RulesActionRequest request, RulesActionResolutionResult resolution)
        {
            Calls.Add("OutcomeApplication");
            return RpgCombatHookDispatchResult.Allow();
        }

        public RpgCombatHookDispatchResult OnTurnPhaseAdvancement(RulesContext context, RulesActionRequest request, ActionRecord appliedAction)
        {
            Calls.Add("TurnPhaseAdvancement");
            return RpgCombatHookDispatchResult.Allow();
        }

        public RpgCombatHookDispatchResult OnObjectiveVictoryCheck(RulesContext context, RulesActionRequest request, ActionRecord appliedAction)
        {
            Calls.Add("ObjectiveVictoryCheck");
            return RpgCombatHookDispatchResult.Allow();
        }

        public RpgCombatHookDispatchResult OnObjectiveVictoryStateChangeEvaluation(RulesContext context, RulesActionRequest request, ActionRecord appliedAction)
        {
            Calls.Add("ObjectiveVictoryStateChange");
            return RpgCombatHookDispatchResult.AllowWithEvaluation(new ObjectiveVictoryHookPayload
            {
                TriggerKind = ObjectiveVictoryTriggerKind.StateChange,
                ObjectiveResults =
                [
                    new ObjectiveEvaluationResult
                    {
                        ObjectiveId = "obj-capture-a",
                        Status = ObjectiveEvaluationStatus.Met,
                        SummaryMessage = "Objective A captured."
                    }
                ],
                VictoryResult = new VictoryEvaluationResult
                {
                    IsTerminal = false,
                    VictoryType = "Objective",
                    Message = "Objective progress updated."
                },
                Message = "State-change evaluation complete.",
                RelatedAction = appliedAction
            });
        }

        public RpgCombatHookDispatchResult OnObjectiveVictoryExplicitEvaluation(RulesContext context, RulesActionRequest request)
        {
            Calls.Add("ObjectiveVictoryExplicit");
            return RpgCombatHookDispatchResult.AllowWithEvaluation(new ObjectiveVictoryHookPayload
            {
                TriggerKind = ObjectiveVictoryTriggerKind.Explicit,
                ObjectiveResults =
                [
                    new ObjectiveEvaluationResult
                    {
                        ObjectiveId = "obj-hold-a",
                        Status = ObjectiveEvaluationStatus.InProgress,
                        SummaryMessage = "Hold objective A for 2 rounds."
                    },
                    new ObjectiveEvaluationResult
                    {
                        ObjectiveId = "obj-eliminate-b",
                        Status = ObjectiveEvaluationStatus.Met,
                        SummaryMessage = "Opposing leader eliminated."
                    }
                ],
                VictoryResult = new VictoryEvaluationResult
                {
                    IsTerminal = true,
                    WinnerParticipantId = "player-1",
                    WinnerSide = "Blue",
                    VictoryType = "LayeredObjective",
                    SatisfiedConditions = ["obj-hold-a", "obj-eliminate-b"],
                    Message = "Layered victory conditions met."
                },
                Message = "Explicit evaluation complete."
            });
        }
    }

    private sealed class RejectingAtValidationHookModule : IRpgCombatHookModule
    {
        private readonly string _message;

        public RejectingAtValidationHookModule(string message)
        {
            _message = message;
        }

        public RpgCombatHookDispatchResult OnTurnStart(RulesContext context, RulesActionRequest request) => RpgCombatHookDispatchResult.Allow();

        public RpgCombatHookDispatchResult OnActionValidation(RulesContext context, RulesActionRequest request)
            => RpgCombatHookDispatchResult.Reject(_message);

        public RpgCombatHookDispatchResult OnActionResolution(RulesContext context, RulesActionRequest request) => RpgCombatHookDispatchResult.Allow();

        public RpgCombatHookDispatchResult OnOutcomeApplication(RulesContext context, RulesActionRequest request, RulesActionResolutionResult resolution) => RpgCombatHookDispatchResult.Allow();

        public RpgCombatHookDispatchResult OnTurnPhaseAdvancement(RulesContext context, RulesActionRequest request, ActionRecord appliedAction) => RpgCombatHookDispatchResult.Allow();

        public RpgCombatHookDispatchResult OnObjectiveVictoryCheck(RulesContext context, RulesActionRequest request, ActionRecord appliedAction) => RpgCombatHookDispatchResult.Allow();

        public RpgCombatHookDispatchResult OnObjectiveVictoryStateChangeEvaluation(RulesContext context, RulesActionRequest request, ActionRecord appliedAction) => RpgCombatHookDispatchResult.Allow();

        public RpgCombatHookDispatchResult OnObjectiveVictoryExplicitEvaluation(RulesContext context, RulesActionRequest request) => RpgCombatHookDispatchResult.Allow();
    }

        public RulesValidationResult ValidateAction(RulesContext context, RulesActionRequest request)
        {
            return RulesValidationResult.Failure(_message);
        }

        public RulesActionResolutionResult ResolveAction(RulesContext context, RulesActionRequest request)
        {
            return RulesActionResolutionResult.Failure("Not used in rejection path.");
        }
    }

    private sealed class RecordingCommandService : IVttSessionCommandService
    {
        public RecordingCommandService(VttSession session)
        {
            CurrentVttSession = session;
        }

        public VttSession? CurrentVttSession { get; }

        public List<ActionRequest> Requests { get; } = [];

        public ActionRecord ProcessAction(ActionRequest request)
        {
            Requests.Add(request);

            var record = new ActionRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                ActionType = request.ActionType,
                ActorParticipantId = request.ActorParticipantId,
                TimestampUtc = DateTime.UtcNow,
                Payload = request.Payload
            };

            CurrentVttSession!.ActionLog.Add(record);
            return record;
        }
    }
}
