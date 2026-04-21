using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SessionWorkspaceServiceTests
{
    [TestMethod]
    public void Workspace_CreateNewSession_SetsCurrentSessionAndClearsFilePath()
    {
        var service = new SessionWorkspaceService();

        service.CreateNewSession();

        Assert.IsNotNull(service.State.CurrentTableSession);
        Assert.IsFalse(string.IsNullOrWhiteSpace(service.State.CurrentTableSession!.Id));
        Assert.IsNull(service.State.CurrentFilePath);
        Assert.IsFalse(service.State.IsDirty);
    }

    [TestMethod]
    public void Workspace_CreateNewSession_AppendsSuccessfulHistoryEntry()
    {
        var service = new SessionWorkspaceService();

        service.CreateNewSession();

        Assert.AreEqual(1, service.State.OperationHistory.Count);
        var entry = service.State.OperationHistory[0];
        Assert.AreEqual(WorkspaceOperationKind.CreateNewSession, entry.OperationKind);
        Assert.IsTrue(entry.Success);
    }

    [TestMethod]
    public void Workspace_OpenTableSessionFromFile_LoadsSessionSetsCurrentFilePathAndClearsDirty()
    {
        var service = new SessionWorkspaceService();
        var serializer = new TableSessionSnapshotSerializer();
        var path = CreateTempFilePath("workspace-open-session", SnapshotFileExtensions.TableSession);

        try
        {
            var session = CreateTableSessionFixture();
            File.WriteAllText(path, serializer.Save(session), Encoding.UTF8);

            service.OpenTableSessionFromFile(path);

            Assert.IsNotNull(service.State.CurrentTableSession);
            Assert.AreEqual("session-1", service.State.CurrentTableSession!.Id);
            Assert.AreEqual(Path.GetFullPath(path), service.State.CurrentFilePath);
            Assert.IsFalse(service.State.IsDirty);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithMissingPendingScenario_AndDefaultOptions_FailsAsCurrentBehavior()
    {
        var stateFilePath = CreateTempFilePath("workspace-options-default-fail", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-options-default-fail-session", SnapshotFileExtensions.TableSession);
        var missingScenarioPath = CreateTempFilePath("workspace-options-default-fail-missing-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var serializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, serializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(missingScenarioPath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var service = new SessionWorkspaceService();
            Assert.ThrowsException<InvalidOperationException>(() => service.RestoreWorkspaceStateWithDiagnostics(stateFilePath));

            Assert.IsNotNull(service.LastRestoreDiagnostics);
            Assert.IsFalse(service.LastRestoreDiagnostics!.IsSuccess);
            Assert.IsTrue(service.LastRestoreDiagnostics.Errors.Any(e => e.Contains("pending scenario file", StringComparison.OrdinalIgnoreCase)));
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(sessionFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithMissingPendingScenario_AndFailRestoreOption_FailsClearly()
    {
        var stateFilePath = CreateTempFilePath("workspace-options-explicit-fail", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-options-explicit-fail-session", SnapshotFileExtensions.TableSession);
        var missingScenarioPath = CreateTempFilePath("workspace-options-explicit-fail-missing-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var serializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, serializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(missingScenarioPath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var service = new SessionWorkspaceService();
            var options = new WorkspaceRestoreOptions
            {
                MissingPendingScenarioBehavior = MissingPendingScenarioMode.FailRestore
            };

            Assert.ThrowsException<InvalidOperationException>(() => service.RestoreWorkspaceStateWithDiagnostics(stateFilePath, options));

            Assert.IsNotNull(service.LastRestoreDiagnostics);
            Assert.IsFalse(service.LastRestoreDiagnostics!.IsSuccess);
            Assert.IsTrue(service.LastRestoreDiagnostics.Errors.Any(e => e.Contains("pending scenario file", StringComparison.OrdinalIgnoreCase)));
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(sessionFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithMissingPendingScenario_AndWarnAndContinue_RestoresCurrentSessionAndReturnsWarning()
    {
        var stateFilePath = CreateTempFilePath("workspace-options-warn-continue", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-options-warn-continue-session", SnapshotFileExtensions.TableSession);
        var missingScenarioPath = CreateTempFilePath("workspace-options-warn-continue-missing-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var serializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, serializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(missingScenarioPath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var service = new SessionWorkspaceService();
            var options = new WorkspaceRestoreOptions
            {
                MissingPendingScenarioBehavior = MissingPendingScenarioMode.WarnAndContinue
            };

            var diagnostics = service.RestoreWorkspaceStateWithDiagnostics(stateFilePath, options);

            Assert.IsTrue(diagnostics.IsSuccess);
            Assert.IsTrue(diagnostics.CurrentSessionRestored);
            Assert.IsFalse(diagnostics.PendingScenarioRestored);
            Assert.IsTrue(diagnostics.Warnings.Any(w => w.Contains("Pending scenario file", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(service.State.CurrentTableSession);
            Assert.AreEqual("session-1", service.State.CurrentTableSession!.Id);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(sessionFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithMissingPendingScenario_AndWarnAndContinue_DoesNotRestorePendingScenario()
    {
        var stateFilePath = CreateTempFilePath("workspace-options-warn-no-pending", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-options-warn-no-pending-session", SnapshotFileExtensions.TableSession);
        var missingScenarioPath = CreateTempFilePath("workspace-options-warn-no-pending-missing-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var serializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, serializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(missingScenarioPath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var service = new SessionWorkspaceService();
            var options = new WorkspaceRestoreOptions
            {
                MissingPendingScenarioBehavior = MissingPendingScenarioMode.WarnAndContinue
            };

            service.RestoreWorkspaceStateWithDiagnostics(stateFilePath, options);

            Assert.IsNull(service.State.CurrentPendingScenarioPlan);
            Assert.IsNull(service.State.PendingScenarioSourcePath);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(sessionFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithValidCurrentSessionAndValidPendingScenario_PreservesExistingRestoreBehaviorAcrossOptions()
    {
        var stateFilePath = CreateTempFilePath("workspace-options-valid-both", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-options-valid-both-session", SnapshotFileExtensions.TableSession);
        var scenarioFilePath = CreateTempFilePath("workspace-options-valid-both-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var sessionSerializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, sessionSerializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var scenarioSerializer = new ScenarioSnapshotSerializer();
            File.WriteAllText(scenarioFilePath, scenarioSerializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(scenarioFilePath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var strictService = new SessionWorkspaceService();
            var strictDiagnostics = strictService.RestoreWorkspaceStateWithDiagnostics(stateFilePath, new WorkspaceRestoreOptions
            {
                MissingPendingScenarioBehavior = MissingPendingScenarioMode.FailRestore
            });

            var bestEffortService = new SessionWorkspaceService();
            var bestEffortDiagnostics = bestEffortService.RestoreWorkspaceStateWithDiagnostics(stateFilePath, new WorkspaceRestoreOptions
            {
                MissingPendingScenarioBehavior = MissingPendingScenarioMode.WarnAndContinue
            });

            Assert.IsTrue(strictDiagnostics.IsSuccess);
            Assert.IsTrue(bestEffortDiagnostics.IsSuccess);
            Assert.IsTrue(strictDiagnostics.CurrentSessionRestored);
            Assert.IsTrue(bestEffortDiagnostics.CurrentSessionRestored);
            Assert.IsTrue(strictDiagnostics.PendingScenarioRestored);
            Assert.IsTrue(bestEffortDiagnostics.PendingScenarioRestored);
            Assert.IsNotNull(strictService.State.CurrentPendingScenarioPlan);
            Assert.IsNotNull(bestEffortService.State.CurrentPendingScenarioPlan);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(sessionFilePath);
            DeleteFileIfExists(scenarioFilePath);
        }
    }

    [TestMethod]
    public void Workspace_OpenTableSessionFromFile_AppendsSuccessfulHistoryEntry()
    {
        var service = new SessionWorkspaceService();
        var serializer = new TableSessionSnapshotSerializer();
        var path = CreateTempFilePath("workspace-history-open", SnapshotFileExtensions.TableSession);

        try
        {
            File.WriteAllText(path, serializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            service.OpenTableSessionFromFile(path);

            var entry = service.State.OperationHistory.Last();
            Assert.AreEqual(WorkspaceOperationKind.OpenTableSessionFromFile, entry.OperationKind);
            Assert.IsTrue(entry.Success);
            Assert.AreEqual(Path.GetFullPath(path), Path.GetFullPath(entry.FilePath!));
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void Workspace_SaveCurrentSession_WithCurrentFilePath_SavesAndClearsDirty()
    {
        var service = new SessionWorkspaceService();
        var path = CreateTempFilePath("workspace-save-current", SnapshotFileExtensions.TableSession);

        try
        {
            service.CreateNewSession();
            service.State.CurrentTableSession!.Title = "Session Before Save";
            service.SaveCurrentSessionAs(path);

            service.State.CurrentTableSession.Title = "Session Updated";

            service.SaveCurrentSession();

            Assert.IsFalse(service.State.IsDirty);

            var loaded = new TableSessionFilePersistenceService().LoadFromFile(path);
            Assert.AreEqual("Session Updated", loaded.Title);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void Workspace_SaveCurrentSessionAs_AppendsSuccessfulHistoryEntry()
    {
        var service = new SessionWorkspaceService();
        var path = CreateTempFilePath("workspace-history-saveas", SnapshotFileExtensions.TableSession);

        try
        {
            service.CreateNewSession();
            service.SaveCurrentSessionAs(path);

            var entry = service.State.OperationHistory.Last();
            Assert.AreEqual(WorkspaceOperationKind.SaveCurrentSessionAs, entry.OperationKind);
            Assert.IsTrue(entry.Success);
            Assert.AreEqual(Path.GetFullPath(path), Path.GetFullPath(entry.FilePath!));
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void Workspace_SaveCurrentSession_WithoutCurrentFilePath_FailsClearly()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.SaveCurrentSession());
        StringAssert.Contains(exception.Message, "CurrentFilePath");
    }

    [TestMethod]
    public void Workspace_SaveCurrentSessionAs_SavesSetsCurrentFilePathAndClearsDirty()
    {
        var service = new SessionWorkspaceService();
        var path = CreateTempFilePath("workspace-save-as", SnapshotFileExtensions.TableSession);

        try
        {
            service.CreateNewSession();
            service.State.CurrentTableSession!.Title = "Saved As Session";

            service.SaveCurrentSessionAs(path);

            Assert.AreEqual(Path.GetFullPath(path), service.State.CurrentFilePath);
            Assert.IsFalse(service.State.IsDirty);

            var loaded = new TableSessionFilePersistenceService().LoadFromFile(path);
            Assert.AreEqual("Saved As Session", loaded.Title);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void Workspace_ImportScenarioToPendingPlanFromFile_AppendsSuccessfulHistoryEntry()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();
        var serializer = new ScenarioSnapshotSerializer();
        var path = CreateTempFilePath("workspace-history-import-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            File.WriteAllText(path, serializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            service.ImportScenarioToPendingPlanFromFile(path);

            var entry = service.State.OperationHistory.Last();
            Assert.AreEqual(WorkspaceOperationKind.ImportScenarioToPendingPlanFromFile, entry.OperationKind);
            Assert.IsTrue(entry.Success);
            Assert.AreEqual(Path.GetFullPath(path), Path.GetFullPath(entry.FilePath!));
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void Workspace_ImportScenarioToPendingPlanFromFile_CreatesPendingPlanWithoutReplacingCurrentSession()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();
        var serializer = new ScenarioSnapshotSerializer();
        var path = CreateTempFilePath("workspace-import-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var currentSession = service.State.CurrentTableSession;

            var scenario = CreateScenarioFixture();
            File.WriteAllText(path, serializer.SerializeScenario(scenario), Encoding.UTF8);

            service.ImportScenarioToPendingPlanFromFile(path);

            Assert.IsNotNull(service.State.CurrentPendingScenarioPlan);
            Assert.AreEqual("Imported Scenario", service.State.CurrentPendingScenarioPlan!.ScenarioTitle);
            Assert.AreSame(currentSession, service.State.CurrentTableSession);
            Assert.IsNull(service.State.CurrentFilePath);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void Workspace_ActivatePendingScenario_AppendsSuccessfulHistoryEntry()
    {
        var service = new SessionWorkspaceService();
        var serializer = new ScenarioSnapshotSerializer();
        var path = CreateTempFilePath("workspace-history-activate-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            service.CreateNewSession();
            File.WriteAllText(path, serializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);
            service.ImportScenarioToPendingPlanFromFile(path);

            service.ActivatePendingScenario();

            var entry = service.State.OperationHistory.Last();
            Assert.AreEqual(WorkspaceOperationKind.ActivatePendingScenario, entry.OperationKind);
            Assert.IsTrue(entry.Success);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void Workspace_ActivatePendingScenario_ReplacesCurrentSessionAndSetsDirtyAccordingToRule()
    {
        var service = new SessionWorkspaceService();
        var serializer = new ScenarioSnapshotSerializer();
        var path = CreateTempFilePath("workspace-activate-scenario", SnapshotFileExtensions.Scenario);
        var savePath = CreateTempFilePath("workspace-before-activate", SnapshotFileExtensions.TableSession);

        try
        {
            service.CreateNewSession();
            File.WriteAllText(path, serializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            service.SaveCurrentSessionAs(savePath);
            service.ImportScenarioToPendingPlanFromFile(path);

            service.ActivatePendingScenario();

            Assert.IsNotNull(service.State.CurrentTableSession);
            Assert.AreEqual("Imported Scenario", service.State.CurrentTableSession!.Title);
            Assert.IsNull(service.State.CurrentFilePath);
            Assert.IsNull(service.State.CurrentPendingScenarioPlan);
            Assert.IsTrue(service.State.IsDirty);
        }
        finally
        {
            DeleteFileIfExists(path);
            DeleteFileIfExists(savePath);
        }
    }

    [TestMethod]
    public void Workspace_ActivatePendingScenario_WithoutPendingPlan_FailsClearly()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.ActivatePendingScenario());
        StringAssert.Contains(exception.Message, "Pending scenario plan");
    }

    [TestMethod]
    public void Workspace_SaveCurrentSession_WithoutCurrentFilePath_AppendsFailedHistoryEntry()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();

        Assert.ThrowsException<InvalidOperationException>(() => service.SaveCurrentSession());

        var entry = service.State.OperationHistory.Last();
        Assert.AreEqual(WorkspaceOperationKind.SaveCurrentSession, entry.OperationKind);
        Assert.IsFalse(entry.Success);
    }

    [TestMethod]
    public void Workspace_ActivatePendingScenario_WithoutPendingPlan_AppendsFailedHistoryEntry()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();

        Assert.ThrowsException<InvalidOperationException>(() => service.ActivatePendingScenario());

        var entry = service.State.OperationHistory.Last();
        Assert.AreEqual(WorkspaceOperationKind.ActivatePendingScenario, entry.OperationKind);
        Assert.IsFalse(entry.Success);
    }

    [TestMethod]
    public void Workspace_ProcessAction_WithSupportedAction_MarksWorkspaceDirty()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();
        service.State.CurrentTableSession!.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" });
        service.State.CurrentTableSession.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-piece-1",
            Location = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 1, Y = 1 } }
        });

        service.ProcessAction(new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 4, Y = 5 } }
            }
        });

        Assert.IsTrue(service.State.IsDirty);
    }

    [TestMethod]
    public void Workspace_ProcessAction_WithSupportedAction_AppendsToCurrentSessionActionLog()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();
        service.State.CurrentTableSession!.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" });
        service.State.CurrentTableSession.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-piece-1",
            Location = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 1, Y = 1 } }
        });

        var record = service.ProcessAction(new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 2, Y = 2 } }
            }
        });

        Assert.AreEqual(1, service.State.CurrentTableSession.ActionLog.Count);
        Assert.AreEqual(record, service.State.CurrentTableSession.ActionLog[0]);
    }

    [TestMethod]
    public void Workspace_ProcessAction_WhenNoCurrentSession_FailsClearly()
    {
        var service = new SessionWorkspaceService();

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.ProcessAction(new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 2, Y = 2 } }
            }
        }));

        StringAssert.Contains(exception.Message, "CurrentTableSession");
    }

    [TestMethod]
    public void Workspace_ProcessAction_WhenActionFails_DoesNotMarkDirty()
    {
        var service = new SessionWorkspaceService();
        service.CreateNewSession();
        service.State.CurrentTableSession!.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" });

        Assert.ThrowsException<InvalidOperationException>(() => service.ProcessAction(new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "missing-piece",
                NewLocation = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 2, Y = 2 } }
            }
        }));

        Assert.IsFalse(service.State.IsDirty);
        var entry = service.State.OperationHistory.Last();
        Assert.AreEqual(WorkspaceOperationKind.ProcessAction, entry.OperationKind);
        Assert.IsFalse(entry.Success);
    }

    [TestMethod]
    public void WorkspaceState_SaveAndLoad_RoundTripsEssentialWorkspaceFields()
    {
        var service = new SessionWorkspaceService();
        var persistence = new SessionWorkspaceStatePersistenceService();
        var stateFilePath = CreateTempFilePath("workspace-state-roundtrip", ".workspace.json");
        var currentSessionPath = CreateTempFilePath("workspace-state-current", SnapshotFileExtensions.TableSession);
        var pendingScenarioPath = CreateTempFilePath("workspace-state-pending", SnapshotFileExtensions.Scenario);

        try
        {
            var scenarioSerializer = new ScenarioSnapshotSerializer();
            File.WriteAllText(pendingScenarioPath, scenarioSerializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            service.CreateNewSession();
            service.SaveCurrentSessionAs(currentSessionPath);
            service.ImportScenarioToPendingPlanFromFile(pendingScenarioPath);

            persistence.SaveWorkspaceState(service.State, stateFilePath);
            var loaded = persistence.LoadWorkspaceState(stateFilePath);

            Assert.AreEqual(Path.GetFullPath(currentSessionPath), loaded.CurrentFilePath);
            Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, loaded.CurrentSnapshotFormat);
            Assert.AreEqual(Path.GetFullPath(pendingScenarioPath), loaded.PendingScenarioSourcePath);
            Assert.IsTrue(loaded.RecentOperationHistory.Count > 0);
            Assert.AreEqual(WorkspaceOperationKind.ImportScenarioToPendingPlanFromFile, loaded.RecentOperationHistory.Last().OperationKind);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(currentSessionPath);
            DeleteFileIfExists(pendingScenarioPath);
        }
    }

    [TestMethod]
    public void Workspace_RestoreWorkspaceState_WithCurrentSessionFile_RehydratesCurrentTableSession()
    {
        var service = new SessionWorkspaceService();
        var stateFilePath = CreateTempFilePath("workspace-restore-current", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-restore-current-session", SnapshotFileExtensions.TableSession);

        try
        {
            var sessionSerializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, sessionSerializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = null,
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var rehydratedService = new SessionWorkspaceService();
            rehydratedService.RestoreWorkspaceState(stateFilePath);

            Assert.IsNotNull(rehydratedService.State.CurrentTableSession);
            Assert.AreEqual("session-1", rehydratedService.State.CurrentTableSession!.Id);
            Assert.AreEqual(Path.GetFullPath(sessionFilePath), rehydratedService.State.CurrentFilePath);
            Assert.IsFalse(rehydratedService.State.IsDirty);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(sessionFilePath);
        }
    }

    [TestMethod]
    public void Workspace_RestoreWorkspaceState_WithPendingScenarioPath_RehydratesPendingScenarioPlanWithoutActivating()
    {
        var service = new SessionWorkspaceService();
        var stateFilePath = CreateTempFilePath("workspace-restore-pending", ".workspace.json");
        var scenarioPath = CreateTempFilePath("workspace-restore-pending-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var scenarioSerializer = new ScenarioSnapshotSerializer();
            File.WriteAllText(scenarioPath, scenarioSerializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = null,
                CurrentSnapshotFormat = SnapshotFormatKind.ScenarioSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(scenarioPath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var restoredService = new SessionWorkspaceService();
            restoredService.CreateNewSession();
            restoredService.RestoreWorkspaceState(stateFilePath);

            Assert.IsNotNull(restoredService.State.CurrentPendingScenarioPlan);
            Assert.AreEqual("Imported Scenario", restoredService.State.CurrentPendingScenarioPlan!.ScenarioTitle);
            Assert.IsNotNull(restoredService.State.CurrentTableSession);
            Assert.AreNotEqual("Imported Scenario", restoredService.State.CurrentTableSession!.Title);
            Assert.IsFalse(restoredService.State.IsDirty);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(scenarioPath);
        }
    }

    [TestMethod]
    public void Workspace_RestoreWorkspaceState_WithMissingCurrentSessionFile_FailsClearlyAndKeepsWorkspaceCoherent()
    {
        var service = new SessionWorkspaceService();
        var stateFilePath = CreateTempFilePath("workspace-restore-missing-current", ".workspace.json");
        var missingSessionPath = CreateTempFilePath("workspace-restore-missing-current-session", SnapshotFileExtensions.TableSession);

        try
        {
            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(missingSessionPath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = null,
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            service.CreateNewSession();
            var beforeId = service.State.CurrentTableSession!.Id;

            var exception = Assert.ThrowsException<InvalidOperationException>(() => service.RestoreWorkspaceState(stateFilePath));
            StringAssert.Contains(exception.Message, "current session file was not found");

            Assert.IsNotNull(service.State.CurrentTableSession);
            Assert.AreEqual(beforeId, service.State.CurrentTableSession!.Id);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
        }
    }

    [TestMethod]
    public void WorkspaceState_SaveWorkspaceState_WritesMainFileSuccessfully()
    {
        var persistence = new SessionWorkspaceStatePersistenceService();
        var stateFilePath = CreateTempFilePath("workspace-safety-main", ".workspace.json");

        try
        {
            var state = new SessionWorkspaceState();
            persistence.SaveWorkspaceState(state, stateFilePath);

            Assert.IsTrue(File.Exists(stateFilePath));
            var loaded = persistence.LoadWorkspaceState(stateFilePath);
            Assert.IsNotNull(loaded);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(stateFilePath + ".bak");
            DeleteFileIfExists(stateFilePath + ".tmp");
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithValidMainFile_ReturnsDiagnostics_MainSourceUsed()
    {
        var stateFilePath = CreateTempFilePath("workspace-diagnostics-main", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-diagnostics-main-session", SnapshotFileExtensions.TableSession);

        try
        {
            var sessionSerializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, sessionSerializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = null,
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var service = new SessionWorkspaceService();
            var diagnostics = service.RestoreWorkspaceStateWithDiagnostics(stateFilePath);

            Assert.IsTrue(diagnostics.IsSuccess);
            Assert.AreEqual(WorkspaceRecoverySource.Main, diagnostics.SourceUsed);
            Assert.IsTrue(diagnostics.MainFileAttempted);
            Assert.IsTrue(diagnostics.MainFileValid);
            Assert.IsFalse(diagnostics.BackupAttempted);
            Assert.IsTrue(diagnostics.CurrentSessionRestored);
            Assert.IsFalse(diagnostics.PendingScenarioRestored);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(stateFilePath + ".bak");
            DeleteFileIfExists(sessionFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithCorruptedMainAndValidBackup_ReturnsDiagnostics_BackupSourceUsedAndWarning()
    {
        var persistence = new SessionWorkspaceStatePersistenceService();
        var stateFilePath = CreateTempFilePath("workspace-diagnostics-backup", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-diagnostics-backup-session", SnapshotFileExtensions.TableSession);

        try
        {
            var sessionSerializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, sessionSerializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var state = new SessionWorkspaceState();
            state.OperationHistory.Add(new WorkspaceOperationEntry
            {
                OperationKind = WorkspaceOperationKind.CreateNewSession,
                TimestampUtc = DateTime.UtcNow,
                Success = true,
                Message = "history"
            });

            persistence.SaveWorkspaceState(state, stateFilePath);
            var backupDocument = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = null,
                RecentOperationHistory = []
            };
            File.WriteAllText(stateFilePath + ".bak", System.Text.Json.JsonSerializer.Serialize(backupDocument), Encoding.UTF8);

            File.WriteAllText(stateFilePath, "{\"CurrentFilePath\":", Encoding.UTF8);

            var service = new SessionWorkspaceService();
            var diagnostics = service.RestoreWorkspaceStateWithDiagnostics(stateFilePath);

            Assert.IsTrue(diagnostics.IsSuccess);
            Assert.AreEqual(WorkspaceRecoverySource.Backup, diagnostics.SourceUsed);
            Assert.IsTrue(diagnostics.MainFileAttempted);
            Assert.IsFalse(diagnostics.MainFileValid);
            Assert.IsTrue(diagnostics.BackupAttempted);
            Assert.IsTrue(diagnostics.BackupValid);
            Assert.IsTrue(diagnostics.Warnings.Count > 0);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(stateFilePath + ".bak");
            DeleteFileIfExists(stateFilePath + ".tmp");
            DeleteFileIfExists(sessionFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithCorruptedMainAndMissingBackup_ReturnsDiagnostics_Failure()
    {
        var stateFilePath = CreateTempFilePath("workspace-diagnostics-main-invalid", ".workspace.json");

        try
        {
            File.WriteAllText(stateFilePath, "{\"CurrentFilePath\":", Encoding.UTF8);

            var service = new SessionWorkspaceService();

            Assert.ThrowsException<InvalidOperationException>(() => service.RestoreWorkspaceStateWithDiagnostics(stateFilePath));

            Assert.IsNotNull(service.LastRestoreDiagnostics);
            Assert.IsFalse(service.LastRestoreDiagnostics!.IsSuccess);
            Assert.AreEqual(WorkspaceRecoverySource.None, service.LastRestoreDiagnostics.SourceUsed);
            Assert.IsTrue(service.LastRestoreDiagnostics.MainFileAttempted);
            Assert.IsFalse(service.LastRestoreDiagnostics.MainFileValid);
            Assert.IsTrue(service.LastRestoreDiagnostics.Errors.Count > 0);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithMissingSessionFile_ReportsErrorInDiagnostics()
    {
        var stateFilePath = CreateTempFilePath("workspace-diagnostics-missing-session", ".workspace.json");
        var missingSessionPath = CreateTempFilePath("workspace-diagnostics-missing-session-file", SnapshotFileExtensions.TableSession);

        try
        {
            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(missingSessionPath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = null,
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var service = new SessionWorkspaceService();

            Assert.ThrowsException<InvalidOperationException>(() => service.RestoreWorkspaceStateWithDiagnostics(stateFilePath));

            Assert.IsNotNull(service.LastRestoreDiagnostics);
            Assert.IsFalse(service.LastRestoreDiagnostics!.IsSuccess);
            Assert.IsTrue(service.LastRestoreDiagnostics.Errors.Any(e => e.Contains("current session file", StringComparison.OrdinalIgnoreCase)));
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_WithMissingScenarioFile_ReportsWarningOrErrorCorrectly()
    {
        var stateFilePath = CreateTempFilePath("workspace-diagnostics-missing-scenario", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-diagnostics-missing-scenario-session", SnapshotFileExtensions.TableSession);
        var missingScenarioPath = CreateTempFilePath("workspace-diagnostics-missing-scenario-file", SnapshotFileExtensions.Scenario);

        try
        {
            var sessionSerializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, sessionSerializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(missingScenarioPath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var service = new SessionWorkspaceService();

            Assert.ThrowsException<InvalidOperationException>(() => service.RestoreWorkspaceStateWithDiagnostics(stateFilePath));

            Assert.IsNotNull(service.LastRestoreDiagnostics);
            Assert.IsTrue(service.LastRestoreDiagnostics!.CurrentSessionRestored);
            Assert.IsFalse(service.LastRestoreDiagnostics.PendingScenarioRestored);
            Assert.IsTrue(service.LastRestoreDiagnostics.Errors.Any(e => e.Contains("pending scenario file", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(service.LastRestoreDiagnostics.Warnings.Any(w => w.Contains("Pending scenario file", StringComparison.OrdinalIgnoreCase)));
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(sessionFilePath);
        }
    }

    [TestMethod]
    public void Workspace_Restore_DiagnosticsFlags_AreConsistentWithRestoreOutcome()
    {
        var stateFilePath = CreateTempFilePath("workspace-diagnostics-consistency", ".workspace.json");
        var sessionFilePath = CreateTempFilePath("workspace-diagnostics-consistency-session", SnapshotFileExtensions.TableSession);
        var scenarioFilePath = CreateTempFilePath("workspace-diagnostics-consistency-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var sessionSerializer = new TableSessionSnapshotSerializer();
            File.WriteAllText(sessionFilePath, sessionSerializer.Save(CreateTableSessionFixture()), Encoding.UTF8);

            var scenarioSerializer = new ScenarioSnapshotSerializer();
            File.WriteAllText(scenarioFilePath, scenarioSerializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = Path.GetFullPath(sessionFilePath),
                CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(scenarioFilePath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            var service = new SessionWorkspaceService();
            var diagnostics = service.RestoreWorkspaceStateWithDiagnostics(stateFilePath);

            Assert.IsTrue(diagnostics.IsSuccess);
            Assert.AreEqual(WorkspaceRecoverySource.Main, diagnostics.SourceUsed);
            Assert.IsTrue(diagnostics.MainFileAttempted);
            Assert.IsTrue(diagnostics.MainFileValid);
            Assert.IsTrue(diagnostics.CurrentSessionRestored);
            Assert.IsTrue(diagnostics.PendingScenarioRestored);
            Assert.AreEqual(0, diagnostics.Errors.Count);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(sessionFilePath);
            DeleteFileIfExists(scenarioFilePath);
        }
    }

    [TestMethod]
    public void WorkspaceState_SaveWorkspaceState_ReplacesViaTempFileWithoutLeavingPartialMainFile()
    {
        var persistence = new SessionWorkspaceStatePersistenceService();
        var stateFilePath = CreateTempFilePath("workspace-safety-temp", ".workspace.json");

        try
        {
            persistence.SaveWorkspaceState(new SessionWorkspaceState(), stateFilePath);

            Assert.IsFalse(File.Exists(stateFilePath + ".tmp"));
            var json = File.ReadAllText(stateFilePath, Encoding.UTF8);
            Assert.IsTrue(json.Contains("RecentOperationHistory"));
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(stateFilePath + ".bak");
            DeleteFileIfExists(stateFilePath + ".tmp");
        }
    }

    [TestMethod]
    public void WorkspaceState_SaveWorkspaceState_PreservesBackupWhenReplacingExistingFile()
    {
        var persistence = new SessionWorkspaceStatePersistenceService();
        var stateFilePath = CreateTempFilePath("workspace-safety-backup", ".workspace.json");

        try
        {
            var first = new SessionWorkspaceState();
            first.OperationHistory.Add(new WorkspaceOperationEntry
            {
                OperationKind = WorkspaceOperationKind.CreateNewSession,
                TimestampUtc = DateTime.UtcNow,
                Success = true,
                Message = "first"
            });

            persistence.SaveWorkspaceState(first, stateFilePath);

            var second = new SessionWorkspaceState();
            second.OperationHistory.Add(new WorkspaceOperationEntry
            {
                OperationKind = WorkspaceOperationKind.SaveCurrentSession,
                TimestampUtc = DateTime.UtcNow,
                Success = true,
                Message = "second"
            });

            persistence.SaveWorkspaceState(second, stateFilePath);

            var backupPath = stateFilePath + ".bak";
            Assert.IsTrue(File.Exists(backupPath));

            var backupLoaded = persistence.LoadWorkspaceState(backupPath);
            Assert.AreEqual(1, backupLoaded.RecentOperationHistory.Count);
            Assert.AreEqual(WorkspaceOperationKind.CreateNewSession, backupLoaded.RecentOperationHistory[0].OperationKind);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(stateFilePath + ".bak");
            DeleteFileIfExists(stateFilePath + ".tmp");
        }
    }

    [TestMethod]
    public void WorkspaceState_LoadWorkspaceState_WhenMainFileCorruptedAndBackupValid_LoadsFromBackup()
    {
        var persistence = new SessionWorkspaceStatePersistenceService();
        var stateFilePath = CreateTempFilePath("workspace-safety-fallback", ".workspace.json");

        try
        {
            var state = new SessionWorkspaceState();
            state.OperationHistory.Add(new WorkspaceOperationEntry
            {
                OperationKind = WorkspaceOperationKind.OpenTableSessionFromFile,
                TimestampUtc = DateTime.UtcNow,
                Success = true,
                Message = "ok"
            });

            persistence.SaveWorkspaceState(state, stateFilePath);
            persistence.SaveWorkspaceState(new SessionWorkspaceState(), stateFilePath);

            File.WriteAllText(stateFilePath, "{\"CurrentFilePath\":", Encoding.UTF8);

            var loaded = persistence.LoadWorkspaceState(stateFilePath);
            Assert.IsNotNull(loaded);
            Assert.IsTrue(loaded.RecentOperationHistory.Count > 0);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(stateFilePath + ".bak");
            DeleteFileIfExists(stateFilePath + ".tmp");
        }
    }

    [TestMethod]
    public void WorkspaceState_LoadWorkspaceState_WhenMainFileCorruptedAndBackupMissing_FailsClearly()
    {
        var persistence = new SessionWorkspaceStatePersistenceService();
        var stateFilePath = CreateTempFilePath("workspace-safety-main-invalid", ".workspace.json");

        try
        {
            File.WriteAllText(stateFilePath, "{\"CurrentFilePath\":", Encoding.UTF8);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => persistence.LoadWorkspaceState(stateFilePath));
            StringAssert.Contains(exception.Message, "malformed JSON");
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(stateFilePath + ".bak");
            DeleteFileIfExists(stateFilePath + ".tmp");
        }
    }

    [TestMethod]
    public void WorkspaceState_LoadWorkspaceState_WhenMainAndBackupBothInvalid_FailsClearly()
    {
        var persistence = new SessionWorkspaceStatePersistenceService();
        var stateFilePath = CreateTempFilePath("workspace-safety-both-invalid", ".workspace.json");

        try
        {
            File.WriteAllText(stateFilePath, "{\"CurrentFilePath\":", Encoding.UTF8);
            File.WriteAllText(stateFilePath + ".bak", "{\"PendingScenarioSourcePath\":", Encoding.UTF8);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => persistence.LoadWorkspaceState(stateFilePath));
            StringAssert.Contains(exception.Message, "backup files are invalid");
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(stateFilePath + ".bak");
            DeleteFileIfExists(stateFilePath + ".tmp");
        }
    }

    [TestMethod]
    public void Workspace_RestoreWorkspaceState_WithMissingPendingScenarioFile_FailsClearlyAndKeepsWorkspaceCoherent()
    {
        var service = new SessionWorkspaceService();
        var stateFilePath = CreateTempFilePath("workspace-restore-missing-pending", ".workspace.json");
        var missingScenarioPath = CreateTempFilePath("workspace-restore-missing-pending-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = null,
                CurrentSnapshotFormat = null,
                PendingScenarioSourcePath = Path.GetFullPath(missingScenarioPath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            service.CreateNewSession();
            var beforeId = service.State.CurrentTableSession!.Id;

            var exception = Assert.ThrowsException<InvalidOperationException>(() => service.RestoreWorkspaceState(stateFilePath));
            StringAssert.Contains(exception.Message, "pending scenario file was not found");

            Assert.IsNotNull(service.State.CurrentTableSession);
            Assert.AreEqual(beforeId, service.State.CurrentTableSession!.Id);
            Assert.IsNull(service.State.CurrentPendingScenarioPlan);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
        }
    }

    [TestMethod]
    public void Workspace_RestoreWorkspaceState_DoesNotAutoActivatePendingScenario()
    {
        var service = new SessionWorkspaceService();
        var stateFilePath = CreateTempFilePath("workspace-restore-no-auto-activate", ".workspace.json");
        var scenarioPath = CreateTempFilePath("workspace-restore-no-auto-activate-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            var serializer = new ScenarioSnapshotSerializer();
            File.WriteAllText(scenarioPath, serializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            var recoveryState = new SessionWorkspaceRecoveryState
            {
                CurrentFilePath = null,
                CurrentSnapshotFormat = SnapshotFormatKind.ScenarioSnapshot,
                PendingScenarioSourcePath = Path.GetFullPath(scenarioPath),
                RecentOperationHistory = []
            };

            File.WriteAllText(stateFilePath, System.Text.Json.JsonSerializer.Serialize(recoveryState), Encoding.UTF8);

            service.CreateNewSession();
            var originalSessionId = service.State.CurrentTableSession!.Id;

            service.RestoreWorkspaceState(stateFilePath);

            Assert.IsNotNull(service.State.CurrentPendingScenarioPlan);
            Assert.AreEqual(originalSessionId, service.State.CurrentTableSession!.Id);
            Assert.AreNotEqual("Imported Scenario", service.State.CurrentTableSession.Title);
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
            DeleteFileIfExists(scenarioPath);
        }
    }

    [TestMethod]
    public void Workspace_SaveWorkspaceState_PersistsChosenHistorySubsetOrSummaryCorrectly()
    {
        var service = new SessionWorkspaceService();
        var stateFilePath = CreateTempFilePath("workspace-save-state-history", ".workspace.json");

        try
        {
            service.CreateNewSession();
            for (var i = 0; i < 12; i++)
            {
                service.ProcessAction(new ActionRequest
                {
                    ActionType = "NoOpAction",
                    ActorParticipantId = "participant-1",
                    Payload = new { Index = i }
                });
            }

            service.SaveWorkspaceState(stateFilePath);

            var persistence = new SessionWorkspaceStatePersistenceService();
            var loaded = persistence.LoadWorkspaceState(stateFilePath);

            Assert.AreEqual(10, loaded.RecentOperationHistory.Count);
            Assert.IsTrue(loaded.RecentOperationHistory.All(e => e.OperationKind == WorkspaceOperationKind.ProcessAction));
        }
        finally
        {
            DeleteFileIfExists(stateFilePath);
        }
    }

    private static TableSession CreateTableSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Opened Session",
            Surfaces =
            [
                new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" }
            ],
            Pieces =
            [
                new PieceInstance
                {
                    Id = "piece-1",
                    DefinitionId = "def-piece-1",
                    Location = new Location
                    {
                        SurfaceId = "surface-1",
                        Coordinate = new Coordinate { X = 2, Y = 2 }
                    }
                }
            ]
        };
    }

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Imported Scenario",
            Surfaces =
            [
                new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" }
            ],
            Pieces =
            [
                new PieceInstance
                {
                    Id = "piece-1",
                    DefinitionId = "def-piece-1",
                    Location = new Location
                    {
                        SurfaceId = "surface-1",
                        Coordinate = new Coordinate { X = 5, Y = 5 }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = false
            }
        };
    }

    private static string CreateTempFilePath(string prefix, string extension)
    {
        return Path.Combine(Path.GetTempPath(), $"{prefix}-{Guid.NewGuid():N}{extension}");
    }

    private static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
