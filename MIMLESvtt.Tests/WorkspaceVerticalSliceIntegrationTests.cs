using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class WorkspaceVerticalSliceIntegrationTests
{
    [TestMethod]
    public void WorkspaceBoard_RendersPiecesForActiveSurface()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-surface-a", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddSurface("surface-b", "def-surface-b", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-a", "def-piece-a", "surface-a", 1, 1, string.Empty, 0);
        workspace.AddPiece("piece-b", "def-piece-b", "surface-b", 2, 2, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-a"
        };

        var rendered = board.GetRenderedPiecesForActiveSurface(workspace.State.CurrentTableSession);

        Assert.AreEqual(1, rendered.Count);
        Assert.AreEqual("piece-a", rendered[0].Id);
    }

    [TestMethod]
    public void WorkspaceBoard_SelectPieceByClick_UpdatesSelectedPieceState()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        board.SelectPiece("piece-1");

        Assert.AreEqual("piece-1", board.SelectedPieceId);
    }

    [TestMethod]
    public void WorkspaceBoard_MoveSelectedPiece_UsesActionFlowAndUpdatesRenderedPosition()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };
        board.SelectPiece("piece-1");

        board.MoveSelectedPiece(workspace, 11, 12);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(11, piece.Location.Coordinate.X);
        Assert.AreEqual(12, piece.Location.Coordinate.Y);
        Assert.IsTrue(workspace.State.CurrentTableSession.ActionLog.Any(a => a.ActionType == "MovePiece"));
    }

    [TestMethod]
    public void WorkspaceBoard_RotateSelectedPiece_UsesActionFlowAndUpdatesRenderedState()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectPiece("piece-1");

        board.RotateSelectedPiece(workspace, 180);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(180, piece.Rotation.Degrees);
        Assert.IsTrue(workspace.State.CurrentTableSession.ActionLog.Any(a => a.ActionType == "RotatePiece"));
    }

    [TestMethod]
    public void WorkspaceBoard_ChangingActiveSurface_FiltersRenderedPiecesCorrectly()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-surface-a", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddSurface("surface-b", "def-surface-b", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-a", "def-piece-a", "surface-a", 1, 1, string.Empty, 0);
        workspace.AddPiece("piece-b", "def-piece-b", "surface-b", 2, 2, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-a"
        };

        var renderedA = board.GetRenderedPiecesForActiveSurface(workspace.State.CurrentTableSession);
        board.SetActiveSurface("surface-b");
        var renderedB = board.GetRenderedPiecesForActiveSurface(workspace.State.CurrentTableSession);

        Assert.AreEqual("piece-a", renderedA.Single().Id);
        Assert.AreEqual("piece-b", renderedB.Single().Id);
    }

    [TestMethod]
    public void WorkspaceVerticalSlice_LifecycleFlow_InvokesCreateOpenSaveImportOperationsThroughService()
    {
        var workspace = new SessionWorkspaceService();
        var sessionPath = CreateTempFilePath("vertical-slice-session", SnapshotFileExtensions.TableSession);
        var scenarioPath = CreateTempFilePath("vertical-slice-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            workspace.CreateNewSession();
            Assert.IsNotNull(workspace.State.CurrentTableSession);

            workspace.State.CurrentTableSession!.Title = "Vertical Slice Session";
            workspace.SaveCurrentSessionAs(sessionPath);

            var loadedSession = new TableSessionFilePersistenceService().LoadFromFile(sessionPath);
            Assert.AreEqual("Vertical Slice Session", loadedSession.Title);

            workspace.OpenTableSessionFromFile(sessionPath);
            Assert.AreEqual(Path.GetFullPath(sessionPath), workspace.State.CurrentFilePath);

            var scenarioSerializer = new ScenarioSnapshotSerializer();
            File.WriteAllText(scenarioPath, scenarioSerializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            workspace.ImportScenarioToPendingPlanFromFile(scenarioPath);
            Assert.IsNotNull(workspace.State.CurrentPendingScenarioPlan);
            Assert.AreEqual("Imported Scenario", workspace.State.CurrentPendingScenarioPlan!.ScenarioTitle);
        }
        finally
        {
            DeleteFileIfExists(sessionPath);
            DeleteFileIfExists(scenarioPath);
        }
    }

    [TestMethod]
    public void WorkspaceVerticalSlice_CreateSurface_UpdatesSessionAndMarksDirty()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();

        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        Assert.AreEqual(1, workspace.State.CurrentTableSession!.Surfaces.Count);
        Assert.AreEqual("surface-1", workspace.State.CurrentTableSession.Surfaces[0].Id);
        Assert.IsTrue(workspace.State.IsDirty);
    }

    [TestMethod]
    public void WorkspaceVerticalSlice_CreatePieceOnSurface_UpdatesSessionAndMarksDirty()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 3, 4, string.Empty, 15);

        Assert.AreEqual(1, workspace.State.CurrentTableSession!.Pieces.Count);
        Assert.AreEqual("piece-1", workspace.State.CurrentTableSession.Pieces[0].Id);
        Assert.AreEqual("surface-1", workspace.State.CurrentTableSession.Pieces[0].Location.SurfaceId);
        Assert.IsTrue(workspace.State.IsDirty);
    }

    [TestMethod]
    public void WorkspaceVerticalSlice_CreatePieceWithoutSurface_FailsClearly()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            workspace.AddPiece("piece-1", "def-piece-1", "missing-surface", 0, 0, string.Empty, 0));

        StringAssert.Contains(exception.Message, "surface");
    }

    [TestMethod]
    public void WorkspaceVerticalSlice_CreateSessionThenSurfaceThenPiece_AllowsImmediateMoveRotateMarkerStateActions()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "gm",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location
                {
                    SurfaceId = "surface-1",
                    Coordinate = new Coordinate { X = 5, Y = 6 }
                }
            }
        });

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "gm",
            Payload = new RotatePiecePayload
            {
                PieceId = "piece-1",
                NewRotation = new Rotation { Degrees = 90 }
            }
        });

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "gm",
            Payload = new AddMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-stunned"
            }
        });

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "ChangePieceState",
            ActorParticipantId = "gm",
            Payload = new ChangePieceStatePayload
            {
                PieceId = "piece-1",
                Key = "Status",
                Value = "Active"
            }
        });

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "RemoveMarker",
            ActorParticipantId = "gm",
            Payload = new RemoveMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-stunned"
            }
        });

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(5, piece.Location.Coordinate.X);
        Assert.AreEqual(6, piece.Location.Coordinate.Y);
        Assert.AreEqual(90, piece.Rotation.Degrees);
        Assert.IsFalse(piece.MarkerIds.Contains("marker-stunned"));
        Assert.AreEqual("Active", piece.State["Status"]);
    }

    [TestMethod]
    public void WorkspaceService_AddSurface_AppendsHistoryEntry()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();

        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var last = workspace.State.OperationHistory.Last();
        Assert.AreEqual(WorkspaceOperationKind.AddSurface, last.OperationKind);
        Assert.IsTrue(last.Success);
    }

    [TestMethod]
    public void WorkspaceService_AddPiece_AppendsHistoryEntry()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 1, 1, string.Empty, 0);

        var last = workspace.State.OperationHistory.Last();
        Assert.AreEqual(WorkspaceOperationKind.AddPiece, last.OperationKind);
        Assert.IsTrue(last.Success);
    }

    [TestMethod]
    public void WorkspaceVerticalSlice_PieceActionPath_WhenActionSucceeds_MarksWorkspaceDirty()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();

        workspace.State.CurrentTableSession!.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" });
        workspace.State.CurrentTableSession.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-piece-1",
            Location = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 0, Y = 0 } }
        });

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "gm",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 4, Y = 4 } }
            }
        });

        Assert.IsTrue(workspace.State.IsDirty);
        Assert.AreEqual(1, workspace.State.CurrentTableSession.ActionLog.Count);
    }

    [TestMethod]
    public void WorkspaceVerticalSlice_PendingScenarioActivation_UpdatesCurrentSessionWhenActivated()
    {
        var workspace = new SessionWorkspaceService();
        var scenarioPath = CreateTempFilePath("vertical-slice-activate-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            workspace.CreateNewSession();

            var serializer = new ScenarioSnapshotSerializer();
            File.WriteAllText(scenarioPath, serializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            workspace.ImportScenarioToPendingPlanFromFile(scenarioPath);
            workspace.ActivatePendingScenario();

            Assert.IsNotNull(workspace.State.CurrentTableSession);
            Assert.AreEqual("Imported Scenario", workspace.State.CurrentTableSession!.Title);
            Assert.IsNull(workspace.State.CurrentPendingScenarioPlan);
        }
        finally
        {
            DeleteFileIfExists(scenarioPath);
        }
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
                        Coordinate = new Coordinate { X = 2, Y = 3 }
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
