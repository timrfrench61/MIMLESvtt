using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ScenarioPlanApplyServiceTests
{
    [TestMethod]
    public void ScenarioPlanApply_WithValidPendingPlan_ReturnsSuccessfulTableSessionCandidate()
    {
        var service = new ScenarioPlanApplyService();
        var request = new ScenarioPlanApplyRequest
        {
            PendingScenarioPlan = CreatePendingScenarioPlan()
        };

        var result = service.Apply(request);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsRuntimeStateMutated);
        Assert.IsNotNull(result.TableSessionCandidate);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TableSessionCandidate!.Id));
    }

    [TestMethod]
    public void ScenarioPlanApply_WithValidPendingPlan_DoesNotMutateActiveRuntimeState()
    {
        var service = new ScenarioPlanApplyService();
        var activeRuntime = new TableSession
        {
            Id = "active-session",
            Title = "Active Runtime",
            Surfaces =
            [
                new SurfaceInstance { Id = "active-surface", DefinitionId = "active-def" }
            ],
            Pieces =
            [
                new PieceInstance { Id = "active-piece", DefinitionId = "active-piece-def" }
            ]
        };

        var request = new ScenarioPlanApplyRequest
        {
            PendingScenarioPlan = CreatePendingScenarioPlan(),
            ActiveRuntimeTableSession = activeRuntime
        };

        var result = service.Apply(request);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("active-session", activeRuntime.Id);
        Assert.AreEqual("Active Runtime", activeRuntime.Title);
        Assert.AreEqual(1, activeRuntime.Surfaces.Count);
        Assert.AreEqual("active-surface", activeRuntime.Surfaces[0].Id);
        Assert.AreEqual(1, activeRuntime.Pieces.Count);
        Assert.AreEqual("active-piece", activeRuntime.Pieces[0].Id);
    }

    [TestMethod]
    public void ScenarioPlanApply_CopiesScenarioTitleSurfacesPiecesAndOptionsIntoCandidate()
    {
        var service = new ScenarioPlanApplyService();
        var request = new ScenarioPlanApplyRequest
        {
            PendingScenarioPlan = CreatePendingScenarioPlan()
        };

        var result = service.Apply(request);

        var candidate = result.TableSessionCandidate!;
        Assert.AreEqual("Imported Scenario", candidate.Title);
        Assert.AreEqual(2, candidate.Surfaces.Count);
        Assert.AreEqual("surface-1", candidate.Surfaces[0].Id);
        Assert.AreEqual(2, candidate.Pieces.Count);
        Assert.AreEqual("piece-1", candidate.Pieces[0].Id);
        Assert.AreEqual("surface-1", candidate.Pieces[0].Location.SurfaceId);
        Assert.IsTrue(candidate.Options.EnableFog);
        Assert.IsFalse(candidate.Options.EnableTurnTracker);
        Assert.IsTrue(candidate.Options.Options.TryGetValue("Grid", out var gridValue));
        Assert.AreEqual("Square", gridValue);
    }

    [TestMethod]
    public void ScenarioPlanApply_ProducesEmptyParticipantsEmptyActionLogAndEmptyModuleState()
    {
        var service = new ScenarioPlanApplyService();
        var request = new ScenarioPlanApplyRequest
        {
            PendingScenarioPlan = CreatePendingScenarioPlan()
        };

        var result = service.Apply(request);

        var candidate = result.TableSessionCandidate!;
        Assert.AreEqual(0, candidate.Participants.Count);
        Assert.AreEqual(0, candidate.ActionLog.Count);
        Assert.AreEqual(0, candidate.ModuleState.Count);
    }

    [TestMethod]
    public void ScenarioPlanApply_WithMissingPlan_FailsClearly()
    {
        var service = new ScenarioPlanApplyService();
        var request = new ScenarioPlanApplyRequest
        {
            PendingScenarioPlan = null
        };

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.Apply(request));
        StringAssert.Contains(exception.Message, "PendingScenarioPlan");
    }

    [TestMethod]
    public void ScenarioPlanApply_WithMissingScenario_FailsClearly()
    {
        var service = new ScenarioPlanApplyService();
        var request = new ScenarioPlanApplyRequest
        {
            PendingScenarioPlan = new PendingScenarioApplicationPlan
            {
                ScenarioTitle = "Invalid Scenario",
                Scenario = null!,
                IntendedOperationKind = SnapshotImportApplyOperationKind.CreateScenarioFromImport,
                IsReadyForApply = true,
                SurfaceCount = 0,
                PieceCount = 0
            }
        };

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.Apply(request));
        StringAssert.Contains(exception.Message, "PendingScenarioPlan.Scenario");
    }

    private static PendingScenarioApplicationPlan CreatePendingScenarioPlan()
    {
        var scenario = new ScenarioExport
        {
            Title = "Imported Scenario",
            Surfaces =
            [
                new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" },
                new SurfaceInstance { Id = "surface-2", DefinitionId = "def-surface-2" }
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
                        Coordinate = new Coordinate { X = 3, Y = 4 }
                    },
                    Rotation = new Rotation { Degrees = 90f }
                },
                new PieceInstance
                {
                    Id = "piece-2",
                    DefinitionId = "def-piece-2",
                    Location = new Location
                    {
                        SurfaceId = "surface-2",
                        Coordinate = new Coordinate { X = 7, Y = 2 }
                    },
                    Rotation = new Rotation { Degrees = 180f }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = false,
                Options =
                {
                    ["Grid"] = "Square"
                }
            }
        };

        return new PendingScenarioApplicationPlan
        {
            ScenarioTitle = scenario.Title,
            Scenario = scenario,
            IntendedOperationKind = SnapshotImportApplyOperationKind.CreateScenarioFromImport,
            IsReadyForApply = true,
            SurfaceCount = scenario.Surfaces.Count,
            PieceCount = scenario.Pieces.Count
        };
    }
}
