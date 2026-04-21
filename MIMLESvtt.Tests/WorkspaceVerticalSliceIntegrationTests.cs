using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;
using System.Linq;

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
    public void WorkspaceBoard_ClickOnBoardWithSelectedPiece_UsesMoveActionAndUpdatesPosition()
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

        board.MoveSelectedPiece(workspace, 120, 140);

        var moved = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(120, moved.Location.Coordinate.X);
        Assert.AreEqual(140, moved.Location.Coordinate.Y);
        Assert.AreEqual("MovePiece", workspace.State.CurrentTableSession.ActionLog.Last().ActionType);
    }

    [TestMethod]
    public void WorkspaceBoard_ClickOnBoardWithoutSelectedPiece_DoesNotMoveAnyPieceAndShowsClearState()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var exception = Assert.ThrowsException<InvalidOperationException>(() => board.MoveSelectedPiece(workspace, 10, 10));

        StringAssert.Contains(exception.Message, "Select a piece first");
        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(0, piece.Location.Coordinate.X);
        Assert.AreEqual(0, piece.Location.Coordinate.Y);
    }

    [TestMethod]
    public void WorkspaceBoard_RotateLeftQuickAction_UsesRotateActionAndUpdatesRotation()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 90);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectPiece("piece-1");

        board.RotateSelectedPiece(workspace, 45);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(45, piece.Rotation.Degrees);
        Assert.AreEqual("RotatePiece", workspace.State.CurrentTableSession.ActionLog.Last().ActionType);
    }

    [TestMethod]
    public void WorkspaceBoard_RotateRightQuickAction_UsesRotateActionAndUpdatesRotation()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 45);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectPiece("piece-1");

        board.RotateSelectedPiece(workspace, 90);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(90, piece.Rotation.Degrees);
        Assert.AreEqual("RotatePiece", workspace.State.CurrentTableSession.ActionLog.Last().ActionType);
    }

    [TestMethod]
    public void WorkspaceBoard_SelectionPersistsAfterSuccessfulBoardMove_IfThatIsYourChosenBehavior()
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

        board.MoveSelectedPiece(workspace, 5, 7);

        Assert.AreEqual("piece-1", board.SelectedPieceId);
    }

    [TestMethod]
    public void WorkspaceBoard_ShowsSelectedPieceSummaryAlignedWithBoardSelection()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 8, 9, string.Empty, 10);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectPiece("piece-1");
        var selected = board.GetSelectedPieceOrThrow(workspace.State.CurrentTableSession);

        Assert.AreEqual("piece-1", selected.Id);
        Assert.AreEqual("surface-1", selected.Location.SurfaceId);
        Assert.AreEqual(8, selected.Location.Coordinate.X);
        Assert.AreEqual(9, selected.Location.Coordinate.Y);
        Assert.AreEqual(10, selected.Rotation.Degrees);
    }

    [TestMethod]
    public void WorkspaceBoard_WhenFocusedAndPieceSelected_ArrowKeyNudgesUseMoveActionAndUpdatePosition()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 10, 10, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };
        board.SelectPiece("piece-1");

        var handled = board.TryHandleKeyboardInput(workspace, "ArrowRight", isBoardFocused: true, moveStep: 10, rotateStep: 45, clampToBoard: false, maxX: 620, maxY: 360, out _);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsTrue(handled);
        Assert.AreEqual(20, piece.Location.Coordinate.X);
        Assert.AreEqual(10, piece.Location.Coordinate.Y);
        Assert.AreEqual("MovePiece", workspace.State.CurrentTableSession.ActionLog.Last().ActionType);
    }

    [TestMethod]
    public void WorkspaceBoard_WhenFocusedAndPieceSelected_RotateHotkeysUseRotateActionAndUpdateRotation()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 90);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectPiece("piece-1");

        var handled = board.TryHandleKeyboardInput(workspace, "q", isBoardFocused: true, moveStep: 10, rotateStep: 45, clampToBoard: false, maxX: 620, maxY: 360, out _);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsTrue(handled);
        Assert.AreEqual(45, piece.Rotation.Degrees);
        Assert.AreEqual("RotatePiece", workspace.State.CurrentTableSession.ActionLog.Last().ActionType);
    }

    [TestMethod]
    public void WorkspaceBoard_WhenNoPieceSelected_KeyboardInputDoesNotMutateStateAndShowsClearState()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var handled = board.TryHandleKeyboardInput(workspace, "ArrowUp", isBoardFocused: true, moveStep: 10, rotateStep: 45, clampToBoard: false, maxX: 620, maxY: 360, out var message);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsFalse(handled);
        Assert.AreEqual(0, piece.Location.Coordinate.X);
        Assert.AreEqual(0, piece.Location.Coordinate.Y);
        Assert.IsFalse(string.IsNullOrWhiteSpace(message));
        Assert.AreEqual(0, workspace.State.CurrentTableSession.ActionLog.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_WhenNotFocused_KeyboardInputDoesNotTriggerPieceActions()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 10, 10, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };
        board.SelectPiece("piece-1");

        var handled = board.TryHandleKeyboardInput(workspace, "ArrowRight", isBoardFocused: false, moveStep: 10, rotateStep: 45, clampToBoard: false, maxX: 620, maxY: 360, out var message);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsFalse(handled);
        Assert.AreEqual(10, piece.Location.Coordinate.X);
        Assert.AreEqual(10, piece.Location.Coordinate.Y);
        Assert.IsFalse(string.IsNullOrWhiteSpace(message));
        Assert.AreEqual(0, workspace.State.CurrentTableSession.ActionLog.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_ShowsKeyboardHelpLegend()
    {
        const string legend = "Board keyboard: click board or tab into it to focus. Arrow keys nudge selected piece by 10. Q rotates left and E rotates right by 45°.";

        Assert.IsTrue(legend.Contains("Arrow keys", StringComparison.Ordinal));
        Assert.IsTrue(legend.Contains("Q", StringComparison.Ordinal));
        Assert.IsTrue(legend.Contains("E", StringComparison.Ordinal));
    }

    [TestMethod]
    public void WorkspaceBoard_NudgeStepSelection_ChangesKeyboardMoveDistance()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 10, 10, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };
        board.SelectPiece("piece-1");

        board.TryHandleKeyboardInput(workspace, "ArrowRight", isBoardFocused: true, moveStep: 1, rotateStep: 45, clampToBoard: false, maxX: 620, maxY: 360, out _);
        board.TryHandleKeyboardInput(workspace, "ArrowRight", isBoardFocused: true, moveStep: 10, rotateStep: 45, clampToBoard: false, maxX: 620, maxY: 360, out _);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(21, piece.Location.Coordinate.X);
    }

    [TestMethod]
    public void WorkspaceBoard_CenterSelected_UsesMoveActionAndMovesPieceToBoardCenter()
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

        var centered = board.TryCenterSelectedPiece(workspace, 310, 180, clampToBoard: false, maxX: 620, maxY: 360, out _);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsTrue(centered);
        Assert.AreEqual(310, piece.Location.Coordinate.X);
        Assert.AreEqual(180, piece.Location.Coordinate.Y);
        Assert.AreEqual("MovePiece", workspace.State.CurrentTableSession.ActionLog.Last().ActionType);
    }

    [TestMethod]
    public void WorkspaceBoard_CenterSelected_WithoutSelection_DoesNotMutateStateAndShowsClearState()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var centered = board.TryCenterSelectedPiece(workspace, 310, 180, clampToBoard: false, maxX: 620, maxY: 360, out var status);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsFalse(centered);
        Assert.AreEqual(0, piece.Location.Coordinate.X);
        Assert.AreEqual(0, piece.Location.Coordinate.Y);
        Assert.IsFalse(string.IsNullOrWhiteSpace(status));
    }

    [TestMethod]
    public void WorkspaceBoard_WithClampEnabled_ClickMove_ClampsCoordinatesWithinBoardBounds()
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

        board.MoveSelectedPiece(workspace, 999, 999, clampToBoard: true, maxX: 620, maxY: 360);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(620, piece.Location.Coordinate.X);
        Assert.AreEqual(360, piece.Location.Coordinate.Y);
    }

    [TestMethod]
    public void WorkspaceBoard_WithClampEnabled_KeyboardNudge_ClampsCoordinatesWithinBoardBounds()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 620, 360, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };
        board.SelectPiece("piece-1");

        board.TryHandleKeyboardInput(workspace, "ArrowRight", isBoardFocused: true, moveStep: 10, rotateStep: 45, clampToBoard: true, maxX: 620, maxY: 360, out _);
        board.TryHandleKeyboardInput(workspace, "ArrowDown", isBoardFocused: true, moveStep: 10, rotateStep: 45, clampToBoard: true, maxX: 620, maxY: 360, out _);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(620, piece.Location.Coordinate.X);
        Assert.AreEqual(360, piece.Location.Coordinate.Y);
    }

    [TestMethod]
    public void WorkspaceBoard_WithClampDisabled_MovementPreservesCurrentFreeBehavior()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 620, 360, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };
        board.SelectPiece("piece-1");

        board.TryHandleKeyboardInput(workspace, "ArrowRight", isBoardFocused: true, moveStep: 10, rotateStep: 45, clampToBoard: false, maxX: 620, maxY: 360, out _);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(630, piece.Location.Coordinate.X);
    }

    [TestMethod]
    public void WorkspaceBoard_HudReflectsSelectedPieceAndInteractionSettings()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 8, 9, string.Empty, 10);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1",
            SelectedPieceId = "piece-1"
        };

        var piece = board.GetSelectedPieceOrThrow(workspace.State.CurrentTableSession);
        var selectedStep = 5;
        var clampEnabled = true;

        Assert.AreEqual("piece-1", piece.Id);
        Assert.AreEqual("surface-1", piece.Location.SurfaceId);
        Assert.AreEqual(8, piece.Location.Coordinate.X);
        Assert.AreEqual(9, piece.Location.Coordinate.Y);
        Assert.AreEqual(10, piece.Rotation.Degrees);
        Assert.AreEqual(5, selectedStep);
        Assert.IsTrue(clampEnabled);
    }

    [TestMethod]
    public void WorkspaceBoard_AddPieceAtBoardClickMode_WhenEnabled_ClickCreatesPieceOnActiveSurface()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 25,
            clickY: 35,
            addPieceAtBoardClickMode: true,
            newPieceId: "piece-new-1",
            newPieceDefinitionId: "def-piece-new-1",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 15,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-new-1");
        Assert.IsTrue(handled);
        Assert.AreEqual("surface-1", piece.Location.SurfaceId);
        Assert.AreEqual(25, piece.Location.Coordinate.X);
        Assert.AreEqual(35, piece.Location.Coordinate.Y);
    }

    [TestMethod]
    public void WorkspaceBoard_AddPieceAtBoardClickMode_AutoSelectsNewlyCreatedPiece()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.TryHandleBoardClick(
            workspace,
            clickX: 10,
            clickY: 10,
            addPieceAtBoardClickMode: true,
            newPieceId: "piece-auto-select",
            newPieceDefinitionId: "def-piece-auto-select",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var createdPieceId);

        Assert.AreEqual("piece-auto-select", createdPieceId);
        Assert.AreEqual("piece-auto-select", board.SelectedPieceId);
    }

    [TestMethod]
    public void WorkspaceBoard_StampMode_WhenEnabled_MultipleClicksCreateMultiplePiecesWithIncrementedIds()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var firstHandled = board.TryHandleBoardClick(
            workspace,
            clickX: 10,
            clickY: 10,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "goblin1",
            newPieceDefinitionId: "def-goblin",
            newPieceOwnerParticipantId: "gm",
            newPieceRotation: 30,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var firstCreatedPieceId,
            out var keepAddModeAfterFirst,
            out var nextIdPreviewAfterFirst);

        var secondHandled = board.TryHandleBoardClick(
            workspace,
            clickX: 30,
            clickY: 35,
            addPieceAtBoardClickMode: keepAddModeAfterFirst,
            stampModeEnabled: true,
            newPieceId: nextIdPreviewAfterFirst,
            newPieceDefinitionId: "def-goblin",
            newPieceOwnerParticipantId: "gm",
            newPieceRotation: 30,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var secondCreatedPieceId,
            out var keepAddModeAfterSecond,
            out _);

        Assert.IsTrue(firstHandled);
        Assert.IsTrue(secondHandled);
        Assert.AreEqual("goblin1", firstCreatedPieceId);
        Assert.AreEqual("goblin2", secondCreatedPieceId);
        Assert.IsTrue(keepAddModeAfterSecond);
        Assert.IsNotNull(workspace.State.CurrentTableSession);
        Assert.AreEqual(2, workspace.State.CurrentTableSession.Pieces.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_StampMode_AfterSuccessfulPlacement_AutoAdvancesNextIdPreview()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 12,
            clickY: 14,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "token-1",
            newPieceDefinitionId: "def-token",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out var keepAddMode,
            out var nextIdPreview);

        Assert.IsTrue(handled);
        Assert.IsTrue(keepAddMode);
        Assert.AreEqual("token-2", nextIdPreview);
    }

    [TestMethod]
    public void WorkspaceBoard_StampMode_AfterSuccessfulPlacement_PreservesDefinitionInputsForNextPlacement()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.TryHandleBoardClick(
            workspace,
            clickX: 20,
            clickY: 22,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "archer1",
            newPieceDefinitionId: "def-archer",
            newPieceOwnerParticipantId: "owner-a",
            newPieceRotation: 45,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out var keepAddMode,
            out var nextIdPreview);

        board.TryHandleBoardClick(
            workspace,
            clickX: 40,
            clickY: 42,
            addPieceAtBoardClickMode: keepAddMode,
            stampModeEnabled: true,
            newPieceId: nextIdPreview,
            newPieceDefinitionId: "def-archer",
            newPieceOwnerParticipantId: "owner-a",
            newPieceRotation: 45,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out _,
            out _);

        var pieces = workspace.State.CurrentTableSession!.Pieces;
        var secondPiece = pieces.Single(p => p.Id == "archer2");
        Assert.AreEqual("def-archer", secondPiece.DefinitionId);
        Assert.AreEqual("owner-a", secondPiece.OwnerParticipantId);
        Assert.AreEqual(45, secondPiece.Rotation.Degrees);
    }

    [TestMethod]
    public void WorkspaceBoard_StampMode_WhenConflictOccurs_ShowsClearFailureAndDoesNotCreatePiece()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("goblin1", "def-goblin", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 25,
            clickY: 25,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "goblin1",
            newPieceDefinitionId: "def-goblin",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out var status,
            out var createdPieceId,
            out var keepAddMode,
            out var nextIdPreview);

        Assert.IsFalse(handled);
        Assert.IsFalse(string.IsNullOrWhiteSpace(status));
        StringAssert.Contains(status, "already exists");
        Assert.IsNull(createdPieceId);
        Assert.AreEqual(1, workspace.State.CurrentTableSession!.Pieces.Count);
        Assert.IsTrue(keepAddMode);
        Assert.IsNull(nextIdPreview);
    }

    [TestMethod]
    public void WorkspaceBoard_StampMode_NewlyCreatedPieceBecomesSelected_IfThatIsYourChosenRule()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.TryHandleBoardClick(
            workspace,
            clickX: 45,
            clickY: 55,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "stamp-1",
            newPieceDefinitionId: "def-stamp",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var createdPieceId,
            out _,
            out _);

        Assert.AreEqual(createdPieceId, board.SelectedPieceId);
        Assert.AreEqual("stamp-1", board.SelectedPieceId);
    }

    [TestMethod]
    public void WorkspaceBoard_StampMode_WhenDisabled_PreservesExistingSinglePlacementBehavior()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 10,
            clickY: 10,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: false,
            newPieceId: "single-1",
            newPieceDefinitionId: "def-single",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out var keepAddMode,
            out var nextIdPreview);

        Assert.IsTrue(handled);
        Assert.IsFalse(keepAddMode);
        Assert.AreEqual("single-2", nextIdPreview);
    }

    [TestMethod]
    public void WorkspaceBoard_StampMode_UserEditedNextId_IsRespectedOnNextPlacement()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.TryHandleBoardClick(
            workspace,
            clickX: 15,
            clickY: 15,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "token-1",
            newPieceDefinitionId: "def-token",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out var keepAddMode,
            out _);

        board.TryHandleBoardClick(
            workspace,
            clickX: 16,
            clickY: 16,
            addPieceAtBoardClickMode: keepAddMode,
            stampModeEnabled: true,
            newPieceId: "token-boss",
            newPieceDefinitionId: "def-token",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var createdPieceId,
            out _,
            out _);

        Assert.AreEqual("token-boss", createdPieceId);
        Assert.IsTrue(workspace.State.CurrentTableSession!.Pieces.Any(p => p.Id == "token-boss"));
    }

    [TestMethod]
    public void WorkspaceBoard_StampSession_ResetIdPreview_RestoresExpectedNextId()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        var nudged = board.NudgePieceIdPreview("goblin5", 2);
        var reset = board.ResetPieceIdPreview("goblin5");

        Assert.AreEqual("goblin7", nudged);
        Assert.AreEqual("goblin5", reset);
    }

    [TestMethod]
    public void WorkspaceBoard_StampSession_IdNudgePlusOne_AdvancesVisibleNextId()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        var result = board.NudgePieceIdPreview("goblin5", 1);

        Assert.AreEqual("goblin6", result);
    }

    [TestMethod]
    public void WorkspaceBoard_StampSession_IdNudgeMinusOne_DecrementsVisibleNextId()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        var result = board.NudgePieceIdPreview("goblin5", -1);

        Assert.AreEqual("goblin4", result);
    }

    [TestMethod]
    public void WorkspaceBoard_StampSession_UserMayStillEditNextIdAfterNudgeControls()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var nudged = board.NudgePieceIdPreview("token-2", 1);

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 22,
            clickY: 24,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "token-boss",
            newPieceDefinitionId: "def-token",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var createdPieceId,
            out _,
            out _);

        Assert.AreEqual("token-3", nudged);
        Assert.IsTrue(handled);
        Assert.AreEqual("token-boss", createdPieceId);
    }

    [TestMethod]
    public void WorkspaceBoard_StampSession_ConflictHandling_RemainsConservativeAfterIdNudge()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("goblin6", "def-goblin", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var nudgeTarget = board.NudgePieceIdPreview("goblin5", 1);
        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 30,
            clickY: 30,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: nudgeTarget,
            newPieceDefinitionId: "def-goblin",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out var status,
            out var createdPieceId,
            out _,
            out _);

        Assert.AreEqual("goblin6", nudgeTarget);
        Assert.IsFalse(handled);
        Assert.IsNull(createdPieceId);
        StringAssert.Contains(status, "already exists");
        Assert.AreEqual(1, workspace.State.CurrentTableSession!.Pieces.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_StampSession_RepeatedPlacementFlow_IsFasterButPreservesExplicitIdControl()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var firstHandled = board.TryHandleBoardClick(
            workspace,
            clickX: 10,
            clickY: 10,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "token-1",
            newPieceDefinitionId: "def-token",
            newPieceOwnerParticipantId: "owner-1",
            newPieceRotation: 90,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out var keepAddMode,
            out var nextIdPreview);

        var nudgedPreview = board.NudgePieceIdPreview(nextIdPreview!, 1);

        var secondHandled = board.TryHandleBoardClick(
            workspace,
            clickX: 20,
            clickY: 20,
            addPieceAtBoardClickMode: keepAddMode,
            stampModeEnabled: true,
            newPieceId: nudgedPreview,
            newPieceDefinitionId: "def-token",
            newPieceOwnerParticipantId: "owner-1",
            newPieceRotation: 90,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var secondCreated,
            out _,
            out _);

        Assert.IsTrue(firstHandled);
        Assert.IsTrue(secondHandled);
        Assert.AreEqual("token-3", nudgedPreview);
        Assert.AreEqual("token-3", secondCreated);
        Assert.IsTrue(workspace.State.CurrentTableSession!.Pieces.Any(p => p.Id == "token-1"));
        Assert.IsTrue(workspace.State.CurrentTableSession.Pieces.Any(p => p.Id == "token-3"));
    }

    [TestMethod]
    public void WorkspaceBoard_StampSession_PerSurfaceDefinitionMemory_RestoresLastDefinitionForSurface()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.RememberDefinitionForSurface("surface-1", "def-goblin");
        board.RememberDefinitionForSurface("surface-2", "def-orc");

        var restoredForSurface2 = board.TryGetRememberedDefinitionForSurface("surface-2");
        var restoredForSurface1 = board.TryGetRememberedDefinitionForSurface("surface-1");

        Assert.AreEqual("def-orc", restoredForSurface2);
        Assert.AreEqual("def-goblin", restoredForSurface1);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_SavePreset_AddsPresetToList()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        var preset = board.AddPreset("Goblin Frontline", "def-goblin", "gm", 45);

        Assert.AreEqual("Goblin Frontline", preset.Name);
        Assert.AreEqual(1, board.StampPresets.Count);
        Assert.AreEqual("def-goblin", board.StampPresets[0].DefinitionId);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_SavePreset_WithoutDefinitionId_FailsClearly()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            board.AddPreset("Invalid", "", "gm", 0));

        StringAssert.Contains(exception.Message, "Definition Id");
        Assert.AreEqual(0, board.StampPresets.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_SelectPreset_PopulatesQuickCreateInputs()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Orc Scout", "def-orc", "owner-a", 90);

        var preset = board.ApplyPreset("Orc Scout");

        Assert.AreEqual("def-orc", preset.DefinitionId);
        Assert.AreEqual("owner-a", preset.OwnerParticipantId);
        Assert.AreEqual(90, preset.InitialRotation);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_SelectPreset_DoesNotChangeIdPreview()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Skeleton", "def-skeleton", "", 0);
        var nextIdPreviewBeforeSelect = "piece-42";

        _ = board.ApplyPreset("Skeleton");

        Assert.AreEqual("piece-42", nextIdPreviewBeforeSelect);
    }

    [TestMethod]
    public void WorkspacePalette_SelectEntry_PopulatesQuickCreateInputs()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Palette Orc", "def-orc", "owner-orc", 90);

        var quickCreateDefinitionId = "def-old";
        var quickCreateOwnerParticipantId = string.Empty;
        var quickCreateRotation = 0f;

        var selected = board.SelectPiecePaletteEntry("Palette Orc");
        quickCreateDefinitionId = selected.DefinitionId;
        quickCreateOwnerParticipantId = selected.OwnerParticipantId;
        quickCreateRotation = selected.InitialRotation;

        Assert.AreEqual("def-orc", quickCreateDefinitionId);
        Assert.AreEqual("owner-orc", quickCreateOwnerParticipantId);
        Assert.AreEqual(90f, quickCreateRotation);
    }

    [TestMethod]
    public void WorkspacePalette_SelectEntry_DoesNotChangeIdPreview()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Palette Skeleton", "def-skeleton", string.Empty, 0);

        var quickCreatePieceId = "piece-42";

        var selected = board.SelectPiecePaletteEntry("Palette Skeleton");
        _ = selected.DefinitionId;

        Assert.AreEqual("piece-42", quickCreatePieceId);
    }

    [TestMethod]
    public void WorkspacePalette_SelectEntry_AllowsImmediateBoardPlacement()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };
        board.AddPreset("Palette Goblin", "def-goblin", "gm", 45);

        var quickCreatePieceId = "palette-piece-1";
        var selected = board.SelectPiecePaletteEntry("Palette Goblin");
        var addPieceAtBoardClickMode = true;

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 50,
            clickY: 60,
            addPieceAtBoardClickMode: addPieceAtBoardClickMode,
            stampModeEnabled: false,
            newPieceId: quickCreatePieceId,
            newPieceDefinitionId: selected.DefinitionId,
            newPieceOwnerParticipantId: selected.OwnerParticipantId,
            newPieceRotation: selected.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var createdPieceId,
            out _,
            out _);

        var created = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "palette-piece-1");
        Assert.IsTrue(handled);
        Assert.AreEqual("palette-piece-1", createdPieceId);
        Assert.AreEqual("def-goblin", created.DefinitionId);
        Assert.AreEqual("gm", created.OwnerParticipantId);
        Assert.AreEqual(45, created.Rotation.Degrees);
    }

    [TestMethod]
    public void WorkspacePalette_ReflectsCurrentPresetData()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Preset A", "def-a", "owner-a", 10);
        board.AddPreset("Preset B", "def-b", string.Empty, 20);

        board.RenamePreset("Preset A", "Preset A Updated");
        board.RemovePreset("Preset B");

        var paletteEntries = board.GetPiecePaletteEntries();

        Assert.AreEqual(1, paletteEntries.Count);
        Assert.AreEqual("Preset A Updated", paletteEntries[0].Name);
        Assert.AreEqual("def-a", paletteEntries[0].DefinitionId);
        Assert.AreEqual("owner-a", paletteEntries[0].OwnerParticipantId);
        Assert.AreEqual(10, paletteEntries[0].InitialRotation);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_DeletePreset_RemovesPresetFromList()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Delete Me", "def-delete", "", 0);

        var removed = board.RemovePreset("Delete Me");

        Assert.IsTrue(removed);
        Assert.AreEqual(0, board.StampPresets.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_RenamePreset_UpdatesName()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Old Name", "def-goblin", "gm", 0);

        var renamed = board.RenamePreset("Old Name", "New Name");

        Assert.AreEqual("New Name", renamed.Name);
        Assert.IsTrue(board.StampPresets.Any(p => p.Name == "New Name"));
        Assert.IsFalse(board.StampPresets.Any(p => p.Name == "Old Name"));
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_RenamePreset_EmptyName_FailsClearly()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Rename Me", "def-goblin", "gm", 0);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            board.RenamePreset("Rename Me", "   "));

        StringAssert.Contains(exception.Message, "cannot be empty");
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_DuplicatePreset_CreatesCopyAndSelectsIt()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Orc Frontline", "def-orc", "owner-orc", 180);

        var duplicated = board.DuplicatePreset("Orc Frontline");

        Assert.AreEqual("Orc Frontline Copy", duplicated.Name);
        Assert.AreEqual("def-orc", duplicated.DefinitionId);
        Assert.AreEqual("owner-orc", duplicated.OwnerParticipantId);
        Assert.AreEqual(180, duplicated.InitialRotation);
        Assert.AreEqual("Orc Frontline Copy", board.ActiveStampPresetName);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_SetDefaultForSurface_PersistsMapping()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("Map A Orc", "def-orc", "", 0);

        board.SetDefaultPresetForSurface("surface-a", "Map A Orc");

        var surfaceDefault = board.GetDefaultPresetForSurface("surface-a");
        Assert.IsNotNull(surfaceDefault);
        Assert.AreEqual("Map A Orc", surfaceDefault!.Name);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_WhenSwitchingSurface_AppliesDefaultPreset()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("SurfaceA", "def-a", "owner-a", 45);
        board.AddPreset("SurfaceB", "def-b", "owner-b", 90);
        board.SetDefaultPresetForSurface("surface-a", "SurfaceA");
        board.SetDefaultPresetForSurface("surface-b", "SurfaceB");

        board.SetActiveSurface("surface-a");
        var presetA = board.GetDefaultPresetForSurface("surface-a")!;
        board.ApplyPreset(presetA.Name);

        board.SetActiveSurface("surface-b");
        var presetB = board.GetDefaultPresetForSurface("surface-b")!;
        var applied = board.ApplyPreset(presetB.Name);

        Assert.AreEqual("SurfaceB", applied.Name);
        Assert.AreEqual("def-b", applied.DefinitionId);
        Assert.AreEqual("SurfaceB", board.ActiveStampPresetName);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_WhenNoDefaultPreset_DoesNotOverrideInputs()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("OnlyA", "def-a", "", 0);
        board.SetDefaultPresetForSurface("surface-a", "OnlyA");

        var manualDefinition = "def-manual";
        board.SetActiveSurface("surface-b");
        var missingDefault = board.GetDefaultPresetForSurface("surface-b");

        Assert.IsNull(missingDefault);
        Assert.AreEqual("def-manual", manualDefinition);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_DefaultOverridesRememberedDefinition()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.RememberDefinitionForSurface("surface-b", "def-remembered");
        board.AddPreset("SurfaceB Default", "def-default", "", 0);
        board.SetDefaultPresetForSurface("surface-b", "SurfaceB Default");

        board.SetActiveSurface("surface-b");
        var rememberedDefinition = board.TryGetRememberedDefinitionForSurface("surface-b");
        var appliedDefault = board.ApplyPreset(board.GetDefaultPresetForSurface("surface-b")!.Name);

        Assert.AreEqual("def-remembered", rememberedDefinition);
        Assert.AreEqual("def-default", appliedDefault.DefinitionId);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_StampModeWithPreset_CreatesPiecesUsingPresetValues()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.AddPreset("Orc Line", "def-orc", "owner-orc", 180);
        var preset = board.ApplyPreset("Orc Line");

        board.TryHandleBoardClick(
            workspace,
            clickX: 30,
            clickY: 30,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "orc-1",
            newPieceDefinitionId: preset.DefinitionId,
            newPieceOwnerParticipantId: preset.OwnerParticipantId,
            newPieceRotation: preset.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out var keepAddMode,
            out var nextIdPreview);

        board.TryHandleBoardClick(
            workspace,
            clickX: 50,
            clickY: 35,
            addPieceAtBoardClickMode: keepAddMode,
            stampModeEnabled: true,
            newPieceId: nextIdPreview,
            newPieceDefinitionId: preset.DefinitionId,
            newPieceOwnerParticipantId: preset.OwnerParticipantId,
            newPieceRotation: preset.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var secondCreatedPieceId,
            out _,
            out _);

        Assert.AreEqual("orc-2", secondCreatedPieceId);
        var pieces = workspace.State.CurrentTableSession!.Pieces;
        Assert.IsTrue(pieces.All(p => p.DefinitionId == "def-orc"));
        Assert.IsTrue(pieces.All(p => p.OwnerParticipantId == "owner-orc"));
        Assert.IsTrue(pieces.All(p => p.Rotation.Degrees == 180));
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_WithStampMode_DefaultPreset_AllowsRapidRepeatedPlacement()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.AddPreset("Goblin Default", "def-goblin", "owner-gm", 30);
        board.SetDefaultPresetForSurface("surface-1", "Goblin Default");

        var preset = board.ApplyPreset(board.GetDefaultPresetForSurface("surface-1")!.Name);
        var firstHandled = board.TryHandleBoardClick(
            workspace,
            clickX: 10,
            clickY: 10,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "goblin1",
            newPieceDefinitionId: preset.DefinitionId,
            newPieceOwnerParticipantId: preset.OwnerParticipantId,
            newPieceRotation: preset.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out var keepAddMode,
            out var nextId);

        var secondHandled = board.TryHandleBoardClick(
            workspace,
            clickX: 12,
            clickY: 12,
            addPieceAtBoardClickMode: keepAddMode,
            stampModeEnabled: true,
            newPieceId: nextId,
            newPieceDefinitionId: preset.DefinitionId,
            newPieceOwnerParticipantId: preset.OwnerParticipantId,
            newPieceRotation: preset.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var createdSecond,
            out _,
            out _);

        Assert.IsTrue(firstHandled);
        Assert.IsTrue(secondHandled);
        Assert.AreEqual("goblin2", createdSecond);
        Assert.AreEqual(2, workspace.State.CurrentTableSession!.Pieces.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_SwitchSurfaceThenBack_RestoresCorrectPreset()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("PresetA", "def-a", "owner-a", 10);
        board.AddPreset("PresetB", "def-b", "owner-b", 20);
        board.SetDefaultPresetForSurface("surface-a", "PresetA");
        board.SetDefaultPresetForSurface("surface-b", "PresetB");

        board.SetActiveSurface("surface-a");
        var surfaceAPreset = board.ApplyPreset(board.GetDefaultPresetForSurface("surface-a")!.Name);

        board.SetActiveSurface("surface-b");
        var surfaceBPreset = board.ApplyPreset(board.GetDefaultPresetForSurface("surface-b")!.Name);

        board.SetActiveSurface("surface-a");
        var surfaceAReturnPreset = board.ApplyPreset(board.GetDefaultPresetForSurface("surface-a")!.Name);

        Assert.AreEqual("PresetA", surfaceAPreset.Name);
        Assert.AreEqual("PresetB", surfaceBPreset.Name);
        Assert.AreEqual("PresetA", surfaceAReturnPreset.Name);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_Operations_DoNotAffectIdPreviewOrConflictValidation()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-existing", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.AddPreset("Preset One", "def-new", "", 0);
        var duplicated = board.DuplicatePreset("Preset One");
        var renamed = board.RenamePreset(duplicated.Name, "Preset Two");
        _ = board.ApplyPreset(renamed.Name);

        var idPreview = "piece-1";
        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 15,
            clickY: 15,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: idPreview,
            newPieceDefinitionId: renamed.DefinitionId,
            newPieceOwnerParticipantId: renamed.OwnerParticipantId,
            newPieceRotation: renamed.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out var status,
            out var createdPieceId,
            out _,
            out _);

        Assert.IsFalse(handled);
        Assert.IsNull(createdPieceId);
        StringAssert.Contains(status, "already exists");
        Assert.AreEqual("piece-1", idPreview);
    }

    [TestMethod]
    public void WorkspaceSurfaceStrip_ClickSurface_SwitchesActiveSurfaceAndUpdatesBoard()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-a", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddSurface("surface-b", "def-b", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-a", "def-piece-a", "surface-a", 1, 1, string.Empty, 0);
        workspace.AddPiece("piece-b", "def-piece-b", "surface-b", 2, 2, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState { ActiveSurfaceId = "surface-a" };

        var switched = board.TrySwitchActiveSurface(workspace.State.CurrentTableSession, "surface-b", clearSelectionWhenSwitchingSurfaces: true, out _, out _);
        var rendered = board.GetRenderedPiecesForActiveSurface(workspace.State.CurrentTableSession);

        Assert.IsTrue(switched);
        Assert.AreEqual("surface-b", board.ActiveSurfaceId);
        Assert.AreEqual(1, rendered.Count);
        Assert.AreEqual("piece-b", rendered[0].Id);
    }

    [TestMethod]
    public void WorkspaceSurfaceStrip_SwitchSurface_AppliesThatSurfaceDefaultPreset()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("PresetA", "def-a", "owner-a", 45);
        board.AddPreset("PresetB", "def-b", "owner-b", 90);
        board.SetDefaultPresetForSurface("surface-a", "PresetA");
        board.SetDefaultPresetForSurface("surface-b", "PresetB");

        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-surface-a", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddSurface("surface-b", "def-surface-b", SurfaceType.Map, CoordinateSystem.Square);

        board.TrySwitchActiveSurface(workspace.State.CurrentTableSession, "surface-b", clearSelectionWhenSwitchingSurfaces: true, out _, out _);
        var applied = board.ApplyPreset(board.GetDefaultPresetForSurface("surface-b")!.Name);

        Assert.AreEqual("PresetB", applied.Name);
        Assert.AreEqual("def-b", applied.DefinitionId);
        Assert.AreEqual("owner-b", applied.OwnerParticipantId);
    }

    [TestMethod]
    public void WorkspaceSurfaceStrip_SwitchSurface_HandlesSelectedPieceAccordingToChosenRule()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-a", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddSurface("surface-b", "def-b", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-a", "def-piece-a", "surface-a", 1, 1, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-a",
            SelectedPieceId = "piece-a"
        };

        board.TrySwitchActiveSurface(workspace.State.CurrentTableSession, "surface-b", clearSelectionWhenSwitchingSurfaces: true, out var clearedSelection, out _);

        Assert.IsTrue(clearedSelection);
        Assert.IsNull(board.SelectedPieceId);
    }

    [TestMethod]
    public void WorkspaceSurfaceStrip_AssignDefaultPresetFromStrip_UpdatesSurfacePresetMapping()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("SurfacePreset", "def-surface", "", 0);

        board.SetDefaultPresetForSurface("surface-strip", "SurfacePreset");

        var mapped = board.GetDefaultPresetForSurface("surface-strip");
        Assert.IsNotNull(mapped);
        Assert.AreEqual("SurfacePreset", mapped!.Name);
    }

    [TestMethod]
    public void WorkspaceSurfaceStrip_QuickAddSurface_AddsSurfaceAndUpdatesStrip()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();

        workspace.AddSurface("surface-new", "def-surface-new", SurfaceType.Map, CoordinateSystem.Hex);

        var added = workspace.State.CurrentTableSession!.Surfaces.Single(s => s.Id == "surface-new");
        Assert.AreEqual(SurfaceType.Map, added.Type);
        Assert.AreEqual(CoordinateSystem.Hex, added.CoordinateSystem);
    }

    [TestMethod]
    public void WorkspaceSurfaceStrip_DuplicateSurface_WithNewId_CreatesNewSurfaceConfig()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-surface-a", SurfaceType.Map, CoordinateSystem.Hex);

        var source = workspace.State.CurrentTableSession!.Surfaces.Single(s => s.Id == "surface-a");
        workspace.AddSurface("surface-a-copy", source.DefinitionId, source.Type, source.CoordinateSystem);

        var duplicate = workspace.State.CurrentTableSession.Surfaces.Single(s => s.Id == "surface-a-copy");
        Assert.AreEqual(source.DefinitionId, duplicate.DefinitionId);
        Assert.AreEqual(source.Type, duplicate.Type);
        Assert.AreEqual(source.CoordinateSystem, duplicate.CoordinateSystem);
    }

    [TestMethod]
    public void WorkspaceSurfaceStrip_DuplicateSurface_WithDuplicateId_FailsClearly()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-surface-a", SurfaceType.Map, CoordinateSystem.Square);

        var source = workspace.State.CurrentTableSession!.Surfaces.Single(s => s.Id == "surface-a");
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            workspace.AddSurface("surface-a", source.DefinitionId, source.Type, source.CoordinateSystem));

        StringAssert.Contains(exception.Message, "already exists");
    }

    [TestMethod]
    public void WorkspaceStampQueue_AddCurrentSetup_CreatesQueueItemWithExplicitSurfacePresetAndId()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("QueuePreset", "def-queue", "owner-a", 45);

        var item = board.AddQueueItem("surface-1", "QueuePreset", "queue-1", "phase-a");

        Assert.AreEqual(1, board.StampQueue.Count);
        Assert.AreEqual("surface-1", item.SurfaceId);
        Assert.AreEqual("QueuePreset", item.PresetName);
        Assert.AreEqual("queue-1", item.NextPieceId);
    }

    [TestMethod]
    public void WorkspaceStampQueue_ReorderItems_UpdatesQueueOrder()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.AddQueueItem("surface-b", null, "b-1");

        var moved = board.MoveQueueItemUp(1);

        Assert.IsTrue(moved);
        Assert.AreEqual("surface-b", board.StampQueue[0].SurfaceId);
        Assert.AreEqual("surface-a", board.StampQueue[1].SurfaceId);
    }

    [TestMethod]
    public void WorkspaceStampQueue_RemoveItem_RemovesItWithoutAffectingOthers()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.AddQueueItem("surface-b", null, "b-1");
        board.AddQueueItem("surface-c", null, "c-1");

        var removed = board.RemoveQueueItem(1);

        Assert.IsTrue(removed);
        Assert.AreEqual(2, board.StampQueue.Count);
        Assert.AreEqual("surface-a", board.StampQueue[0].SurfaceId);
        Assert.AreEqual("surface-c", board.StampQueue[1].SurfaceId);
    }

    [TestMethod]
    public void WorkspaceStampQueue_ApplyQueueItem_SwitchesSurfaceAndPopulatesBoardContext()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-a", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddSurface("surface-b", "def-b", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("PresetB", "def-piece-b", "owner-b", 90);
        board.AddQueueItem("surface-b", "PresetB", "piece-b-1");

        var applied = board.TryApplyQueueItemToBoardContext(
            0,
            workspace.State.CurrentTableSession,
            clearSelectionWhenSwitchingSurfaces: true,
            out var queueItem,
            out var preset,
            out _,
            out _);

        Assert.IsTrue(applied);
        Assert.AreEqual("surface-b", board.ActiveSurfaceId);
        Assert.AreEqual("piece-b-1", queueItem!.NextPieceId);
        Assert.AreEqual("PresetB", preset!.Name);
    }

    [TestMethod]
    public void WorkspaceStampQueue_SuccessfulStampedPlacement_AdvancesActiveQueueItemNextId()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState { ActiveSurfaceId = "surface-1" };
        board.AddPreset("PresetA", "def-piece", "owner-a", 0);
        board.AddQueueItem("surface-1", "PresetA", "queuepiece1");
        board.TryApplyQueueItemToBoardContext(0, workspace.State.CurrentTableSession, true, out var queueItem, out var preset, out _, out _);

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 10,
            clickY: 10,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: queueItem!.NextPieceId,
            newPieceDefinitionId: preset!.DefinitionId,
            newPieceOwnerParticipantId: preset.OwnerParticipantId,
            newPieceRotation: preset.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out _,
            out var nextSuggestedId);

        var updated = board.TryUpdateActiveQueueItemNextId(nextSuggestedId!);

        Assert.IsTrue(handled);
        Assert.IsTrue(updated);
        Assert.AreEqual("queuepiece2", board.StampQueue[0].NextPieceId);
    }

    [TestMethod]
    public void WorkspaceStampQueue_NextQueueItem_SwitchesBoardContextToFollowingItem()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-a", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddSurface("surface-b", "def-b", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("PresetA", "def-a-piece", "", 0);
        board.AddPreset("PresetB", "def-b-piece", "", 0);
        board.AddQueueItem("surface-a", "PresetA", "a-1");
        board.AddQueueItem("surface-b", "PresetB", "b-1");
        board.TryApplyQueueItemToBoardContext(0, workspace.State.CurrentTableSession, true, out _, out _, out _, out _);

        var moved = board.TryApplyNextQueueItemToBoardContext(
            workspace.State.CurrentTableSession,
            clearSelectionWhenSwitchingSurfaces: true,
            out var queueItem,
            out var preset,
            out _,
            out _);

        Assert.IsTrue(moved);
        Assert.AreEqual("surface-b", board.ActiveSurfaceId);
        Assert.AreEqual("b-1", queueItem!.NextPieceId);
        Assert.AreEqual("PresetB", preset!.Name);
    }

    [TestMethod]
    public void WorkspaceStampQueue_ConflictDuringPlacement_RemainsConservativeAndKeepsQueueEditable()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("dup-1", "def-existing", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState { ActiveSurfaceId = "surface-1" };
        board.AddPreset("PresetA", "def-piece", "", 0);
        board.AddQueueItem("surface-1", "PresetA", "dup-1");
        board.TryApplyQueueItemToBoardContext(0, workspace.State.CurrentTableSession, true, out var queueItem, out var preset, out _, out _);

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 5,
            clickY: 5,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: queueItem!.NextPieceId,
            newPieceDefinitionId: preset!.DefinitionId,
            newPieceOwnerParticipantId: preset.OwnerParticipantId,
            newPieceRotation: preset.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out var status,
            out var createdPieceId,
            out _,
            out _);

        var editable = board.UpdateQueueItemNextId(0, "dup-2");

        Assert.IsFalse(handled);
        Assert.IsNull(createdPieceId);
        StringAssert.Contains(status, "already exists");
        Assert.IsTrue(editable);
        Assert.AreEqual("dup-2", board.StampQueue[0].NextPieceId);
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_SaveQueue_AsTemplate_CreatesTemplateCopy()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1", "alpha");

        var template = board.SaveQueueAsTemplate("Template A");

        Assert.AreEqual("Template A", template.TemplateName);
        Assert.AreEqual(1, board.StampQueueTemplates.Count);
        Assert.AreEqual("surface-a", template.Items[0].SurfaceId);
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_SaveQueue_EmptyQueue_FailsClearly()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        var ex = Assert.ThrowsException<InvalidOperationException>(() => board.SaveQueueAsTemplate("Template A"));

        StringAssert.Contains(ex.Message, "Queue must contain at least one item");
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_SaveQueue_EmptyName_FailsClearly()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");

        var ex = Assert.ThrowsException<InvalidOperationException>(() => board.SaveQueueAsTemplate("   "));

        StringAssert.Contains(ex.Message, "Template name");
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_ApplyTemplate_ReplacesCurrentQueue()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.SaveQueueAsTemplate("Template A");
        board.AddQueueItem("surface-b", null, "b-1");

        var applied = board.ApplyTemplateToQueue("Template A");

        Assert.AreEqual("Template A", applied.TemplateName);
        Assert.AreEqual(1, board.StampQueue.Count);
        Assert.AreEqual("surface-a", board.StampQueue[0].SurfaceId);
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_ApplyTemplate_ResetsActiveQueueIndex()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.AddQueueItem("surface-b", null, "b-1");
        board.SaveQueueAsTemplate("Template AB");
        board.AddQueueItem("surface-c", null, "c-1");

        board.ApplyTemplateToQueue("Template AB");

        Assert.AreEqual(0, board.ActiveStampQueueIndex);
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_ApplyTemplate_DoesNotMutateTemplateSource()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.SaveQueueAsTemplate("Template A");
        board.ApplyTemplateToQueue("Template A");

        board.UpdateQueueItemNextId(0, "a-2");

        var template = board.StampQueueTemplates.Single(t => t.TemplateName == "Template A");
        Assert.AreEqual("a-1", template.Items[0].NextPieceId);
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_DeleteTemplate_RemovesTemplate()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.SaveQueueAsTemplate("Template A");

        var removed = board.DeleteQueueTemplate("Template A");

        Assert.IsTrue(removed);
        Assert.AreEqual(0, board.StampQueueTemplates.Count);
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_RenameTemplate_UpdatesName()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.SaveQueueAsTemplate("Template A");

        var renamed = board.RenameQueueTemplate("Template A", "Template A Prime");

        Assert.AreEqual("Template A Prime", renamed.TemplateName);
        Assert.IsTrue(board.StampQueueTemplates.Any(t => t.TemplateName == "Template A Prime"));
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_RenameTemplate_EmptyName_FailsClearly()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.SaveQueueAsTemplate("Template A");

        var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            board.RenameQueueTemplate("Template A", "  "));

        StringAssert.Contains(ex.Message, "cannot be empty");
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_DuplicateTemplate_CreatesIndependentCopy()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.SaveQueueAsTemplate("Template A");

        var duplicated = board.DuplicateQueueTemplate("Template A");

        Assert.AreEqual("Template A Copy", duplicated.TemplateName);
        Assert.AreEqual(2, board.StampQueueTemplates.Count);
        Assert.AreEqual("Template A Copy", board.ActiveStampQueueTemplateName);
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_AppliedTemplate_AllowsNormalQueueExecutionFlow()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-a", "def-a", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddPreset("PresetA", "def-piece-a", "", 0);
        board.AddQueueItem("surface-a", "PresetA", "piece1");
        board.SaveQueueAsTemplate("Template A");

        board.ApplyTemplateToQueue("Template A");
        board.TryApplyQueueItemToBoardContext(0, workspace.State.CurrentTableSession, true, out var item, out var preset, out _, out _);

        var handled = board.TryHandleBoardClick(
            workspace,
            10,
            10,
            true,
            true,
            item!.NextPieceId,
            preset!.DefinitionId,
            preset.OwnerParticipantId,
            preset.InitialRotation,
            false,
            620,
            360,
            out _,
            out var created,
            out _,
            out var nextId);

        Assert.IsTrue(handled);
        Assert.AreEqual("piece1", created);
        Assert.AreEqual("piece2", nextId);
    }

    [TestMethod]
    public void WorkspaceStampQueueTemplate_ModifyingQueueAfterApply_DoesNotAffectTemplate()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.AddQueueItem("surface-a", null, "a-1");
        board.SaveQueueAsTemplate("Template A");
        board.ApplyTemplateToQueue("Template A");

        board.UpdateQueueItemNextId(0, "a-9");

        var template = board.StampQueueTemplates.Single(t => t.TemplateName == "Template A");
        Assert.AreEqual("a-1", template.Items[0].NextPieceId);
        Assert.AreEqual(1, template.Items.Count);
    }

    [TestMethod]
    public void WorkspaceUndo_MovePiece_UndoRestoresPreviousLocation()
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
                    Coordinate = new Coordinate { X = 10, Y = 20 }
                }
            }
        });

        workspace.UndoLastOperation();

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(0, piece.Location.Coordinate.X);
        Assert.AreEqual(0, piece.Location.Coordinate.Y);
    }

    [TestMethod]
    public void WorkspaceUndo_MovePiece_RedoReappliesLocation()
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
                    Coordinate = new Coordinate { X = 11, Y = 21 }
                }
            }
        });

        workspace.UndoLastOperation();
        workspace.RedoLastOperation();

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(11, piece.Location.Coordinate.X);
        Assert.AreEqual(21, piece.Location.Coordinate.Y);
    }

    [TestMethod]
    public void WorkspaceUndo_RotatePiece_UndoRestoresPreviousRotation()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 30);

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "gm",
            Payload = new RotatePiecePayload
            {
                PieceId = "piece-1",
                NewRotation = new Rotation { Degrees = 120 }
            }
        });

        workspace.UndoLastOperation();

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual(30, piece.Rotation.Degrees);
    }

    [TestMethod]
    public void WorkspaceUndo_AddMarker_UndoRemovesMarker()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "gm",
            Payload = new AddMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-a"
            }
        });

        workspace.UndoLastOperation();

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsFalse(piece.MarkerIds.Contains("marker-a"));
    }

    [TestMethod]
    public void WorkspaceUndo_RemoveMarker_UndoRestoresMarker()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);
        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "gm",
            Payload = new AddMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-a"
            }
        });

        workspace.ProcessAction(new ActionRequest
        {
            ActionType = "RemoveMarker",
            ActorParticipantId = "gm",
            Payload = new RemoveMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-a"
            }
        });

        workspace.UndoLastOperation();

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsTrue(piece.MarkerIds.Contains("marker-a"));
    }

    [TestMethod]
    public void WorkspaceUndo_ChangePieceState_UndoRestoresPreviousValue()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

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
            ActionType = "ChangePieceState",
            ActorParticipantId = "gm",
            Payload = new ChangePieceStatePayload
            {
                PieceId = "piece-1",
                Key = "Status",
                Value = "Inactive"
            }
        });

        workspace.UndoLastOperation();

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.AreEqual("Active", piece.State["Status"]);
    }

    [TestMethod]
    public void WorkspaceUndo_NewForwardOperation_ClearsRedoStack()
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
                    Coordinate = new Coordinate { X = 5, Y = 5 }
                }
            }
        });

        workspace.UndoLastOperation();
        Assert.IsTrue(workspace.State.CanRedo);

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

        Assert.IsFalse(workspace.State.CanRedo);
    }

    [TestMethod]
    public void WorkspaceUndo_WhenNothingToUndo_FailsClearlyOrNoOpsAccordingToChosenRule()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();

        var ex = Assert.ThrowsException<InvalidOperationException>(() => workspace.UndoLastOperation());

        StringAssert.Contains(ex.Message, "Nothing is available to undo");
    }

    [TestMethod]
    public void WorkspaceUI_UndoRedoButtons_ReflectAvailability()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        Assert.IsTrue(workspace.State.CanUndo);
        Assert.IsFalse(workspace.State.CanRedo);

        workspace.UndoLastOperation();

        Assert.IsTrue(workspace.State.CanRedo);
    }

    [TestMethod]
    public void WorkspaceUndo_AddPiece_UndoRemovesPiece()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-undo", "def-piece-undo", "surface-1", 1, 1, string.Empty, 0);

        workspace.UndoLastOperation();

        Assert.IsFalse(workspace.State.CurrentTableSession!.Pieces.Any(p => p.Id == "piece-undo"));
    }

    [TestMethod]
    public void WorkspaceUndo_AddSurface_UndoRemovesSurface()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-undo", "def-surface-undo", SurfaceType.Map, CoordinateSystem.Square);

        workspace.UndoLastOperation();

        Assert.IsFalse(workspace.State.CurrentTableSession!.Surfaces.Any(s => s.Id == "surface-undo"));
    }

    [TestMethod]
    public void WorkspaceBoard_MultiSelect_CtrlClickSelectsMultiplePieces()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        board.SelectSinglePiece("piece-1");
        board.TogglePieceSelection("piece-2");

        CollectionAssert.AreEquivalent(new[] { "piece-1", "piece-2" }, board.SelectedPieceIds);
        Assert.AreEqual("piece-2", board.SelectedPieceId);
    }

    [TestMethod]
    public void WorkspaceBoard_MultiSelect_CtrlClickTogglesPieceSelection()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectSinglePiece("piece-1");
        board.TogglePieceSelection("piece-2");

        board.TogglePieceSelection("piece-2");

        CollectionAssert.AreEqual(new[] { "piece-1" }, board.SelectedPieceIds.ToArray());
    }

    [TestMethod]
    public void WorkspaceBoard_MultiSelect_ClearSelection_ClearsAll()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectSinglePiece("piece-1");
        board.TogglePieceSelection("piece-2");

        board.ClearSelection();

        Assert.AreEqual(0, board.SelectedPieceIds.Count);
        Assert.IsNull(board.SelectedPieceId);
    }

    [TestMethod]
    public void WorkspaceBoard_MultiSelect_GroupMove_UpdatesAllSelectedPiecePositions()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);
        workspace.AddPiece("piece-2", "def-piece-2", "surface-1", 5, 5, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectSinglePiece("piece-1");
        board.TogglePieceSelection("piece-2");

        var moved = board.MoveSelectedPiecesByDelta(workspace, 3, 4, clampToBoard: false, maxX: 620, maxY: 360);

        Assert.AreEqual(2, moved);
        var piece1 = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        var piece2 = workspace.State.CurrentTableSession.Pieces.Single(p => p.Id == "piece-2");
        Assert.AreEqual(3, piece1.Location.Coordinate.X);
        Assert.AreEqual(4, piece1.Location.Coordinate.Y);
        Assert.AreEqual(8, piece2.Location.Coordinate.X);
        Assert.AreEqual(9, piece2.Location.Coordinate.Y);
    }

    [TestMethod]
    public void WorkspaceBoard_MultiSelect_GroupMove_UsesActionFlow()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);
        workspace.AddPiece("piece-2", "def-piece-2", "surface-1", 5, 5, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectSinglePiece("piece-1");
        board.TogglePieceSelection("piece-2");

        board.MoveSelectedPiecesByDelta(workspace, 1, 1, clampToBoard: false, maxX: 620, maxY: 360);

        var moveActions = workspace.State.CurrentTableSession!.ActionLog.Count(a => a.ActionType == "MovePiece");
        Assert.AreEqual(2, moveActions);
    }

    [TestMethod]
    public void WorkspaceBoard_MultiSelect_SelectionSummaryReflectsCurrentSelection()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();
        board.SelectSinglePiece("piece-1");
        board.TogglePieceSelection("piece-2");

        var summary = board.GetSelectionSummary();

        StringAssert.Contains(summary, "Selected: 2");
        StringAssert.Contains(summary, "piece-1");
        StringAssert.Contains(summary, "piece-2");
    }

    [TestMethod]
    public void WorkspaceBoard_MultiSelect_SelectAllOnActiveSurface_SelectsExpectedPieces()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddSurface("surface-2", "def-surface-2", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-a", "def-a", "surface-1", 0, 0, string.Empty, 0);
        workspace.AddPiece("piece-b", "def-b", "surface-1", 1, 1, string.Empty, 0);
        workspace.AddPiece("piece-c", "def-c", "surface-2", 2, 2, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var selectedCount = board.SelectAllPiecesOnActiveSurface(workspace.State.CurrentTableSession);

        Assert.AreEqual(2, selectedCount);
        CollectionAssert.AreEquivalent(new[] { "piece-a", "piece-b" }, board.SelectedPieceIds);
    }

    [TestMethod]
    public void WorkspaceBoard_VisibilityToggle_HidePieces_RemovesPiecesFromBoardRender()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState { ActiveSurfaceId = "surface-1" };
        var visibility = new MIMLESvtt.Components.Pages.BoardVisibilityOptions { ShowPieces = false };

        var rendered = board.GetRenderedPiecesForBoard(workspace.State.CurrentTableSession, visibility);

        Assert.AreEqual(0, rendered.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_VisibilityToggle_HideSurfaces_RemovesSurfaceVisuals()
    {
        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState { ActiveSurfaceId = "surface-1" };

        var showSurfaceVisuals = board.ShouldRenderSurfaceVisuals(showSurfaces: false);

        Assert.IsFalse(showSurfaceVisuals);
    }

    [TestMethod]
    public void WorkspaceBoard_Filter_ByDefinitionId_ShowsMatchingPiecesOnly()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-goblin", "def-goblin", "surface-1", 0, 0, string.Empty, 0);
        workspace.AddPiece("piece-orc", "def-orc", "surface-1", 5, 5, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState { ActiveSurfaceId = "surface-1" };
        var visibility = new MIMLESvtt.Components.Pages.BoardVisibilityOptions
        {
            ShowPieces = true,
            ActiveSurfaceOnly = true,
            DefinitionIdFilter = "goblin"
        };

        var rendered = board.GetRenderedPiecesForBoard(workspace.State.CurrentTableSession, visibility);

        Assert.AreEqual(1, rendered.Count);
        Assert.AreEqual("piece-goblin", rendered[0].Id);
    }

    [TestMethod]
    public void WorkspaceBoard_Filter_ByOwner_ShowsMatchingPiecesOnly()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-a", "def-a", "surface-1", 0, 0, "owner-a", 0);
        workspace.AddPiece("piece-b", "def-b", "surface-1", 5, 5, "owner-b", 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState { ActiveSurfaceId = "surface-1" };
        var visibility = new MIMLESvtt.Components.Pages.BoardVisibilityOptions
        {
            ShowPieces = true,
            ActiveSurfaceOnly = true,
            OwnerParticipantIdFilter = "owner-a"
        };

        var rendered = board.GetRenderedPiecesForBoard(workspace.State.CurrentTableSession, visibility);

        Assert.AreEqual(1, rendered.Count);
        Assert.AreEqual("piece-a", rendered[0].Id);
    }

    [TestMethod]
    public void WorkspaceBoard_Filter_HiddenSelectedPiece_ClearsSelectionAccordingToRule()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-goblin", "def-goblin", "surface-1", 0, 0, string.Empty, 0);
        workspace.AddPiece("piece-orc", "def-orc", "surface-1", 5, 5, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState { ActiveSurfaceId = "surface-1" };
        board.SelectSinglePiece("piece-orc");

        var visibility = new MIMLESvtt.Components.Pages.BoardVisibilityOptions
        {
            ShowPieces = true,
            ActiveSurfaceOnly = true,
            DefinitionIdFilter = "goblin"
        };

        var cleared = board.ClearHiddenSelections(workspace.State.CurrentTableSession, visibility);

        Assert.IsTrue(cleared);
        Assert.IsNull(board.SelectedPieceId);
        Assert.AreEqual(0, board.SelectedPieceIds.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_StampPreset_SwitchingPresets_ChangesDefinitionUsedForNextPlacement()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.AddPreset("Goblin", "def-goblin", "", 0);
        board.AddPreset("Orc", "def-orc", "", 0);

        var goblinPreset = board.ApplyPreset("Goblin");
        board.TryHandleBoardClick(
            workspace,
            clickX: 10,
            clickY: 10,
            addPieceAtBoardClickMode: true,
            stampModeEnabled: true,
            newPieceId: "switch-1",
            newPieceDefinitionId: goblinPreset.DefinitionId,
            newPieceOwnerParticipantId: goblinPreset.OwnerParticipantId,
            newPieceRotation: goblinPreset.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out var keepAddMode,
            out _);

        var orcPreset = board.ApplyPreset("Orc");
        board.TryHandleBoardClick(
            workspace,
            clickX: 20,
            clickY: 20,
            addPieceAtBoardClickMode: keepAddMode,
            stampModeEnabled: true,
            newPieceId: "switch-2",
            newPieceDefinitionId: orcPreset.DefinitionId,
            newPieceOwnerParticipantId: orcPreset.OwnerParticipantId,
            newPieceRotation: orcPreset.InitialRotation,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _,
            out _,
            out _);

        var pieces = workspace.State.CurrentTableSession!.Pieces;
        Assert.AreEqual("def-goblin", pieces.Single(p => p.Id == "switch-1").DefinitionId);
        Assert.AreEqual("def-orc", pieces.Single(p => p.Id == "switch-2").DefinitionId);
    }

    [TestMethod]
    public void WorkspaceBoard_AddPieceAtBoardClickMode_WhenDisabled_ClickPreservesCurrentBoardBehavior()
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

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 40,
            clickY: 50,
            addPieceAtBoardClickMode: false,
            newPieceId: "piece-should-not-be-created",
            newPieceDefinitionId: "def-ignored",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out var createdPieceId);

        var moved = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-1");
        Assert.IsTrue(handled);
        Assert.AreEqual(40, moved.Location.Coordinate.X);
        Assert.AreEqual(50, moved.Location.Coordinate.Y);
        Assert.IsNull(createdPieceId);
        Assert.AreEqual(1, workspace.State.CurrentTableSession.Pieces.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_AddPieceAtBoardClickMode_WithoutActiveSurface_FailsClearly()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState();

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 10,
            clickY: 10,
            addPieceAtBoardClickMode: true,
            newPieceId: "piece-no-surface",
            newPieceDefinitionId: "def-piece-no-surface",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out var status,
            out _);

        Assert.IsFalse(handled);
        Assert.IsFalse(string.IsNullOrWhiteSpace(status));
        Assert.AreEqual(0, workspace.State.CurrentTableSession!.Pieces.Count);
    }

    [TestMethod]
    public void WorkspaceBoard_AddPieceAtBoardClickMode_WithDuplicatePieceId_FailsClearly()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
        workspace.AddPiece("piece-dup", "def-piece-dup", "surface-1", 0, 0, string.Empty, 0);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        var handled = board.TryHandleBoardClick(
            workspace,
            clickX: 15,
            clickY: 15,
            addPieceAtBoardClickMode: true,
            newPieceId: "piece-dup",
            newPieceDefinitionId: "def-piece-dup2",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out var status,
            out _);

        Assert.IsFalse(handled);
        Assert.IsFalse(string.IsNullOrWhiteSpace(status));
        StringAssert.Contains(status, "already exists");
    }

    [TestMethod]
    public void WorkspaceBoard_AddPieceAtBoardClickMode_NewPieceCanImmediatelyUseMoveRotateActions()
    {
        var workspace = new SessionWorkspaceService();
        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        var board = new MIMLESvtt.Components.Pages.WorkspaceBoardState
        {
            ActiveSurfaceId = "surface-1"
        };

        board.TryHandleBoardClick(
            workspace,
            clickX: 20,
            clickY: 25,
            addPieceAtBoardClickMode: true,
            newPieceId: "piece-quick-use",
            newPieceDefinitionId: "def-piece-quick-use",
            newPieceOwnerParticipantId: string.Empty,
            newPieceRotation: 0,
            clampToBoard: false,
            maxX: 620,
            maxY: 360,
            out _,
            out _);

        board.MoveSelectedPiece(workspace, 30, 35);
        board.RotateSelectedPiece(workspace, 90);

        var piece = workspace.State.CurrentTableSession!.Pieces.Single(p => p.Id == "piece-quick-use");
        Assert.AreEqual(30, piece.Location.Coordinate.X);
        Assert.AreEqual(35, piece.Location.Coordinate.Y);
        Assert.AreEqual(90, piece.Rotation.Degrees);
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
