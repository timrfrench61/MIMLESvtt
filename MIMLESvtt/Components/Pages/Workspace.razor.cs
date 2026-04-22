using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MIMLESvtt.src;

namespace MIMLESvtt.Components.Pages;

public partial class Workspace : ComponentBase
{
    private enum TabletopSurfaceState
    {
        DefaultXyGrid,
        MissingTabletop,
        PageNotLoading
    }

    private readonly WorkspaceBoardState board = new();
    private bool isWorkspacePanelOpen = true;
    private TabletopSurfaceState tabletopSurfaceState = TabletopSurfaceState.DefaultXyGrid;

    private string? sessionPath;
    private string? scenarioPath;
    private string scenarioTitle = "Current Workspace Layout";
    private string sessionTitleInput = string.Empty;
    private string turnOrderInput = "player-1, player-2";
    private string phaseInput = "Setup";
    private string participantIdInput = "player-1";
    private string participantNameInput = "Player 1";
    private string removeParticipantIdInput = string.Empty;
    private string renameParticipantIdInput = string.Empty;
    private string renameParticipantNameInput = string.Empty;
    private string selectedPieceOwnerParticipantId = string.Empty;
    private string removeStateKey = string.Empty;
    private int quickHpValue;

    private string? selectedPieceId;

    private string newSurfaceId = "surface-1";
    private string newSurfaceDefinitionId = "def-surface-1";
    private SurfaceType newSurfaceType = SurfaceType.Map;
    private CoordinateSystem newCoordinateSystem = CoordinateSystem.Square;

    private string newPieceId = "piece-1";
    private string newPieceDefinitionId = "def-piece-1";
    private string newPieceSurfaceId = "surface-1";
    private float newPieceX;
    private float newPieceY;
    private float newPieceRotation;
    private string newPieceOwnerParticipantId = string.Empty;

    private string moveSurfaceId = "surface-1";
    private float moveX;
    private float moveY;
    private float rotateDegrees;
    private const float QuickRotateIncrement = 45f;
    private int selectedNudgeStep = 10;
    private bool clampToBoard;
    private bool addPieceAtBoardClickMode;
    private bool stampModeEnabled;
    private string quickCreatePieceId = "piece-quick-1";
    private string quickCreateBasePieceId = "piece-quick-1";
    private string quickCreateDefinitionId = "def-piece-quick-1";
    private string quickCreateOwnerParticipantId = string.Empty;
    private float quickCreateRotation;
    private bool rememberDefinitionPerSurface;
    private string? previousActiveSurfaceId;
    private string stampPresetNameInput = string.Empty;
    private string stampPresetRenameInput = string.Empty;
    private string? selectedStampPresetName;
    private string quickSurfaceId = "surface-2";
    private string quickSurfaceDefinitionId = "";
    private SurfaceType quickSurfaceType = SurfaceType.Map;
    private CoordinateSystem quickSurfaceCoordinateSystem = CoordinateSystem.Square;
    private string duplicateSurfaceId = "surface-2-copy";
    private string stampQueueNote = string.Empty;
    private string queueTemplateNameInput = string.Empty;
    private string? selectedQueueTemplateName;
    private readonly Dictionary<string, string> queueTemplateRenameBufferByName = new(StringComparer.Ordinal);
    private string groupLabelInput = string.Empty;
    private string? selectedGroupId;
    private int sessionCounter;

    private const float BoardMaxX = 620f;
    private const float BoardMaxY = 360f;

    private float lastBoardClickX;
    private float lastBoardClickY;
    private bool isBoardFocused;
    private string markerId = "marker-1";
    private string stateKey = "Status";
    private string stateValue = "Active";
    private float groupMoveDeltaX = 10;
    private float groupMoveDeltaY;
    private readonly BoardVisibilityOptions boardVisibility = new();
    private string boardVisibilityDefinitionFilter = string.Empty;
    private string boardVisibilityOwnerFilter = string.Empty;

    protected override void OnInitialized()
    {
        if (WorkspaceService.State.CurrentTableSession is null)
        {
            WorkspaceService.CreateNewSession();
            sessionCounter = 1;
        }
        else
        {
            sessionCounter = 1;
        }

        board.EnsureActiveSurface(WorkspaceService.State.CurrentTableSession);

        if (WorkspaceService.State.CurrentTableSession!.Pieces.Count > 0)
        {
            selectedPieceId = WorkspaceService.State.CurrentTableSession.Pieces[0].Id;
            board.SelectedPieceId = selectedPieceId;
            selectedPieceOwnerParticipantId = WorkspaceService.State.CurrentTableSession.Pieces[0].OwnerParticipantId;
        }

        previousActiveSurfaceId = board.ActiveSurfaceId;
        phaseInput = WorkspaceService.State.CurrentTableSession.CurrentPhase;
        sessionTitleInput = WorkspaceService.State.CurrentTableSession.Title;
    }

    private void AddSurface()
    {
        Execute(() =>
        {
            WorkspaceService.AddSurface(newSurfaceId, newSurfaceDefinitionId, newSurfaceType, newCoordinateSystem);
            board.EnsureActiveSurface(WorkspaceService.State.CurrentTableSession);
            moveSurfaceId = newSurfaceId;
            if (string.IsNullOrWhiteSpace(newPieceSurfaceId))
            {
                newPieceSurfaceId = newSurfaceId;
            }
        }, "Added surface.");
    }

    private void AddPiece()
    {
        Execute(() =>
        {
            WorkspaceService.AddPiece(
                newPieceId,
                newPieceDefinitionId,
                newPieceSurfaceId,
                newPieceX,
                newPieceY,
                newPieceOwnerParticipantId,
                newPieceRotation);

            selectedPieceId = newPieceId;
            board.SelectedPieceId = selectedPieceId;
            selectedPieceOwnerParticipantId = newPieceOwnerParticipantId;
            board.EnsureActiveSurface(WorkspaceService.State.CurrentTableSession);
        }, "Added piece.");
    }

    private void CreateNewSession()
    {
        Execute(() =>
        {
            WorkspaceService.CreateNewSession();
            sessionCounter++;
            board.EnsureActiveSurface(WorkspaceService.State.CurrentTableSession);
            selectedPieceId = WorkspaceService.State.CurrentTableSession?.Pieces.FirstOrDefault()?.Id;
            board.SelectedPieceId = selectedPieceId;
            selectedPieceOwnerParticipantId = WorkspaceService.State.CurrentTableSession?.Pieces.FirstOrDefault()?.OwnerParticipantId ?? string.Empty;
            phaseInput = WorkspaceService.State.CurrentTableSession?.CurrentPhase ?? string.Empty;
            sessionTitleInput = WorkspaceService.State.CurrentTableSession?.Title ?? string.Empty;

            var sessionId = WorkspaceService.State.CurrentTableSession?.Id;
            var shortId = !string.IsNullOrWhiteSpace(sessionId) && sessionId.Length >= 8 ? sessionId[..8] : sessionId;
            WorkspaceService.State.LastOperationMessage = $"Created new workspace session ({shortId}) at {DateTime.Now:T}.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;

            StateHasChanged();
        }, "Created new session.");
    }

    private void OpenSessionFromPath()
    {
        Execute(() =>
        {
            WorkspaceService.OpenTableSessionFromFile(sessionPath ?? string.Empty);
            board.EnsureActiveSurface(WorkspaceService.State.CurrentTableSession);
            selectedPieceId = WorkspaceService.State.CurrentTableSession?.Pieces.FirstOrDefault()?.Id;
            board.SelectedPieceId = selectedPieceId;
            selectedPieceOwnerParticipantId = WorkspaceService.State.CurrentTableSession?.Pieces.FirstOrDefault()?.OwnerParticipantId ?? string.Empty;
            phaseInput = WorkspaceService.State.CurrentTableSession?.CurrentPhase ?? string.Empty;
            sessionTitleInput = WorkspaceService.State.CurrentTableSession?.Title ?? string.Empty;
        }, "Opened session from file.");
    }

    private void SaveCurrentSession()
    {
        Execute(() => WorkspaceService.SaveCurrentSession(), "Saved current session.");
    }

    private void SaveCurrentSessionAs()
    {
        Execute(() => WorkspaceService.SaveCurrentSessionAs(sessionPath ?? string.Empty), "Saved current session as file.");
    }

    private void SaveCurrentLayoutAsScenario()
    {
        Execute(() => WorkspaceService.SaveCurrentLayoutAsScenario(scenarioTitle, scenarioPath ?? string.Empty), "Saved current layout as scenario file.");
    }

    private void OpenScenarioForPendingPlan()
    {
        Execute(() => WorkspaceService.ImportScenarioToPendingPlanFromFile(scenarioPath ?? string.Empty), "Opened scenario for pending plan workflow.");
    }

    private void ActivatePendingScenario()
    {
        Execute(() =>
        {
            WorkspaceService.ActivatePendingScenario();
            board.EnsureActiveSurface(WorkspaceService.State.CurrentTableSession);
            selectedPieceId = WorkspaceService.State.CurrentTableSession?.Pieces.FirstOrDefault()?.Id;
            board.SelectedPieceId = selectedPieceId;
            selectedPieceOwnerParticipantId = WorkspaceService.State.CurrentTableSession?.Pieces.FirstOrDefault()?.OwnerParticipantId ?? string.Empty;
            phaseInput = WorkspaceService.State.CurrentTableSession?.CurrentPhase ?? string.Empty;
            sessionTitleInput = WorkspaceService.State.CurrentTableSession?.Title ?? string.Empty;
        }, "Activated pending scenario.");
    }

    private void UpdateSessionTitle()
    {
        try
        {
            WorkspaceService.UpdateSessionTitle(sessionTitleInput);
            sessionTitleInput = WorkspaceService.State.CurrentTableSession?.Title ?? sessionTitleInput;
            WorkspaceService.State.LastOperationMessage = $"Updated session title to '{sessionTitleInput}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            StateHasChanged();
        }
    }

    private void InitializeTurnOrderFromInput()
    {
        Execute(() =>
        {
            var ids = turnOrderInput
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            WorkspaceService.InitializeTurnOrder(ids);
        }, "Initialized turn order.");
    }

    private void AdvanceTurn()
    {
        Execute(() => WorkspaceService.AdvanceTurn(), "Advanced to next turn.");
    }

    private void PreviousTurn()
    {
        Execute(() => WorkspaceService.PreviousTurn(), "Moved to previous turn.");
    }

    private void MoveTurnParticipantUp(string participantId)
    {
        Execute(() => WorkspaceService.MoveTurnParticipantUp(participantId), "Moved turn participant up.");
    }

    private void MoveTurnParticipantDown(string participantId)
    {
        Execute(() => WorkspaceService.MoveTurnParticipantDown(participantId), "Moved turn participant down.");
    }

    private void SetCurrentPhase()
    {
        Execute(() => WorkspaceService.SetPhase(phaseInput), "Updated current phase.");
    }

    private void SwitchToEditMode()
    {
        Execute(() => WorkspaceService.SetWorkspaceMode(WorkspaceMode.Edit), "Switched workspace mode to Edit.");
    }

    private void SwitchToPlayMode()
    {
        Execute(() => WorkspaceService.SetWorkspaceMode(WorkspaceMode.Play), "Switched workspace mode to Play.");
    }

    private void AddParticipantToSession()
    {
        Execute(() =>
        {
            WorkspaceService.AddParticipant(participantIdInput, participantNameInput);
            removeParticipantIdInput = participantIdInput;
            renameParticipantIdInput = participantIdInput;
            renameParticipantNameInput = participantNameInput;
            if (!string.IsNullOrWhiteSpace(turnOrderInput))
            {
                turnOrderInput += ", ";
            }

            turnOrderInput += participantIdInput;
        }, "Added participant.");
    }

    private void RemoveParticipantFromSession()
    {
        Execute(() =>
        {
            WorkspaceService.RemoveParticipant(removeParticipantIdInput);
            if (string.Equals(selectedPieceOwnerParticipantId, removeParticipantIdInput, StringComparison.Ordinal))
            {
                selectedPieceOwnerParticipantId = string.Empty;
            }
        }, "Removed participant.");
    }

    private void RenameParticipantInSession()
    {
        Execute(() => WorkspaceService.RenameParticipant(renameParticipantIdInput, renameParticipantNameInput), "Renamed participant.");
    }

    private void AssignOwnerToSelectedPiece()
    {
        Execute(() =>
        {
            var pieceId = RequireSelectedPiece();
            WorkspaceService.AssignPieceOwner(pieceId, selectedPieceOwnerParticipantId);
        }, "Updated selected piece owner.");
    }

    private void OnSelectedPieceChangedFromList()
    {
        var piece = WorkspaceService.State.CurrentTableSession?.Pieces.FirstOrDefault(p => p.Id == selectedPieceId);
        selectedPieceOwnerParticipantId = piece?.OwnerParticipantId ?? string.Empty;
        board.SelectedPieceId = selectedPieceId;
    }

    private string GetCurrentTurnParticipant()
    {
        var session = WorkspaceService.State.CurrentTableSession;
        if (session is null || session.TurnOrder.Count == 0)
        {
            return "(none)";
        }

        if (session.CurrentTurnIndex < 0 || session.CurrentTurnIndex >= session.TurnOrder.Count)
        {
            return "(invalid index)";
        }

        var currentParticipantId = session.TurnOrder[session.CurrentTurnIndex];
        return GetParticipantDisplayById(currentParticipantId);
    }

    private string GetOwnerDisplayForPiece(PieceInstance piece)
    {
        if (string.IsNullOrWhiteSpace(piece.OwnerParticipantId))
        {
            return "(none)";
        }

        return GetParticipantDisplayById(piece.OwnerParticipantId);
    }

    private string GetParticipantDisplayById(string participantId)
    {
        var session = WorkspaceService.State.CurrentTableSession;
        if (session is null)
        {
            return participantId;
        }

        var participant = session.Participants.FirstOrDefault(p => string.Equals(p.Id, participantId, StringComparison.Ordinal));
        if (participant is null)
        {
            return participantId;
        }

        return $"{participant.Name} ({participant.Id})";
    }

    private string GetOwnedPiecesSummary(string participantId)
    {
        var session = WorkspaceService.State.CurrentTableSession;
        if (session is null)
        {
            return "0";
        }

        var ownedPieceIds = session.Pieces
            .Where(p => string.Equals(p.OwnerParticipantId, participantId, StringComparison.Ordinal))
            .Select(p => p.Id)
            .ToList();

        if (ownedPieceIds.Count == 0)
        {
            return "0";
        }

        return $"{ownedPieceIds.Count} ({string.Join(", ", ownedPieceIds)})";
    }

    private void UndoLastWorkspaceOperation()
    {
        try
        {
            WorkspaceService.UndoLastOperation();
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void CreateGroupFromSelection()
    {
        Execute(() =>
        {
            var group = board.CreateGroupFromSelection(groupLabelInput);
            selectedGroupId = group.GroupId;
        }, "Created piece group from selection.");
    }

    private void DisbandGroup(string groupId)
    {
        Execute(() =>
        {
            if (!board.DisbandGroup(groupId))
            {
                throw new InvalidOperationException("Selected group was not found.");
            }

            if (string.Equals(selectedGroupId, groupId, StringComparison.Ordinal))
            {
                selectedGroupId = null;
            }
        }, "Disbanded piece group.");
    }

    private void SelectGroup(string groupId)
    {
        var selectedCount = board.SelectGroup(groupId);
        selectedPieceId = board.SelectedPieceId;
        selectedGroupId = groupId;
        WorkspaceService.State.LastOperationMessage = $"Selected {selectedCount} piece(s) in group.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void MoveGroupByDelta(string groupId)
    {
        Execute(() =>
        {
            selectedGroupId = groupId;
            board.MoveGroupByDelta(WorkspaceService, groupId, groupMoveDeltaX, groupMoveDeltaY, clampToBoard, BoardMaxX, BoardMaxY);
            selectedPieceId = board.SelectedPieceId;
        }, "Moved group by delta.");
    }

    private void MoveSelectedGroupByDeltaControl()
    {
        Execute(() =>
        {
            if (string.IsNullOrWhiteSpace(selectedGroupId))
            {
                throw new InvalidOperationException("Select a group first.");
            }

            board.MoveGroupByDelta(WorkspaceService, selectedGroupId, groupMoveDeltaX, groupMoveDeltaY, clampToBoard, BoardMaxX, BoardMaxY);
            selectedPieceId = board.SelectedPieceId;
        }, "Moved selected group by delta.");
    }

    private void OnDefinitionFilterChanged()
    {
        boardVisibility.DefinitionIdFilter = boardVisibilityDefinitionFilter;
        ApplyBoardVisibilityRules();
    }

    private void OnOwnerFilterChanged()
    {
        boardVisibility.OwnerParticipantIdFilter = boardVisibilityOwnerFilter;
        ApplyBoardVisibilityRules();
    }

    private void ApplyBoardVisibilityRules()
    {
        if (board.ClearHiddenSelections(WorkspaceService.State.CurrentTableSession, boardVisibility))
        {
            selectedPieceId = board.SelectedPieceId;
            WorkspaceService.State.LastOperationMessage = "Cleared selection for pieces hidden by current board visibility filters.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
        }
    }

    private void RedoLastWorkspaceOperation()
    {
        try
        {
            WorkspaceService.RedoLastOperation();
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void OnBoardPieceClicked(string pieceId, MouseEventArgs args)
    {
        if (args.CtrlKey || args.MetaKey)
        {
            board.TogglePieceSelection(pieceId);
            selectedPieceId = board.SelectedPieceId;
        }
        else
        {
            selectedPieceId = pieceId;
            board.SelectSinglePiece(pieceId);
        }

        var piece = WorkspaceService.State.CurrentTableSession?.Pieces.FirstOrDefault(p => p.Id == pieceId);
        if (piece is not null)
        {
            moveSurfaceId = piece.Location.SurfaceId;
            moveX = piece.Location.Coordinate.X;
            moveY = piece.Location.Coordinate.Y;
            rotateDegrees = piece.Rotation.Degrees;
            selectedPieceOwnerParticipantId = piece.OwnerParticipantId;
        }
    }

    private void ClearBoardSelection()
    {
        board.ClearSelection();
        selectedPieceId = null;
        WorkspaceService.State.LastOperationMessage = "Cleared board selection.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
    }

    private void SelectAllOnActiveSurface()
    {
        var selectedCount = board.SelectAllPiecesOnActiveSurface(WorkspaceService.State.CurrentTableSession);
        selectedPieceId = board.SelectedPieceId;
        WorkspaceService.State.LastOperationMessage = $"Selected {selectedCount} piece(s) on active surface.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void MoveSelectedGroupByDelta()
    {
        try
        {
            var moved = board.MoveSelectedPiecesByDelta(
                CommandService,
                groupMoveDeltaX,
                groupMoveDeltaY,
                clampToBoard,
                BoardMaxX,
                BoardMaxY);

            WorkspaceService.State.LastOperationMessage = $"Moved {moved} selected piece(s) by ΔX={groupMoveDeltaX}, ΔY={groupMoveDeltaY}.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void SaveStampPreset()
    {
        try
        {
            var preset = board.AddPreset(
                stampPresetNameInput,
                quickCreateDefinitionId,
                quickCreateOwnerParticipantId,
                quickCreateRotation);

            selectedStampPresetName = preset.Name;
            board.ApplyPreset(preset.Name);
            stampPresetNameInput = string.Empty;
            WorkspaceService.State.LastOperationMessage = $"Saved stamp preset '{preset.Name}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void OnStampPresetSelected(ChangeEventArgs args)
    {
        selectedStampPresetName = args.Value?.ToString();

        if (string.IsNullOrWhiteSpace(selectedStampPresetName))
        {
            board.ClearActivePresetSelection();
            return;
        }

        try
        {
            var preset = board.ApplyPreset(selectedStampPresetName);
            ApplyPresetToQuickCreate(preset, setMessage: true);
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void UseSelectedStampPreset()
    {
        if (string.IsNullOrWhiteSpace(selectedStampPresetName))
        {
            WorkspaceService.State.LastOperationMessage = "Select a preset first.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        try
        {
            var preset = board.ApplyPreset(selectedStampPresetName);
            ApplyPresetToQuickCreate(preset, setMessage: true);
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void ArmPlacementFromPalette(string presetName)
    {
        try
        {
            var preset = board.SelectPiecePaletteEntry(presetName);
            ApplyPresetToQuickCreate(preset, setMessage: false);
            selectedStampPresetName = preset.Name;
            addPieceAtBoardClickMode = true;
            WorkspaceService.State.LastOperationMessage = $"Armed placement from palette entry '{preset.Name}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void ClearPresetSelection()
    {
        selectedStampPresetName = string.Empty;
        board.ClearActivePresetSelection();
        WorkspaceService.State.LastOperationMessage = "Cleared active preset selection.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
    }

    private void RenameSelectedStampPreset()
    {
        if (string.IsNullOrWhiteSpace(selectedStampPresetName))
        {
            WorkspaceService.State.LastOperationMessage = "Select a preset first.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        try
        {
            var renamed = board.RenamePreset(selectedStampPresetName, stampPresetRenameInput);
            selectedStampPresetName = renamed.Name;
            stampPresetRenameInput = string.Empty;
            WorkspaceService.State.LastOperationMessage = $"Renamed preset to '{renamed.Name}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void DuplicateSelectedStampPreset()
    {
        if (string.IsNullOrWhiteSpace(selectedStampPresetName))
        {
            WorkspaceService.State.LastOperationMessage = "Select a preset first.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        try
        {
            var duplicated = board.DuplicatePreset(selectedStampPresetName);
            selectedStampPresetName = duplicated.Name;
            ApplyPresetToQuickCreate(duplicated, setMessage: false);
            WorkspaceService.State.LastOperationMessage = $"Duplicated preset as '{duplicated.Name}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void SetSelectedPresetAsDefaultForSurface()
    {
        if (string.IsNullOrWhiteSpace(selectedStampPresetName) || string.IsNullOrWhiteSpace(board.ActiveSurfaceId))
        {
            WorkspaceService.State.LastOperationMessage = "Select a preset and active surface first.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        try
        {
            board.SetDefaultPresetForSurface(board.ActiveSurfaceId, selectedStampPresetName);
            WorkspaceService.State.LastOperationMessage = $"Set '{selectedStampPresetName}' as default for surface '{board.ActiveSurfaceId}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void ClearDefaultPresetForSurface()
    {
        if (string.IsNullOrWhiteSpace(board.ActiveSurfaceId))
        {
            WorkspaceService.State.LastOperationMessage = "Select an active surface first.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        board.ClearDefaultPresetForSurface(board.ActiveSurfaceId);
        WorkspaceService.State.LastOperationMessage = $"Cleared default preset for surface '{board.ActiveSurfaceId}'.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void DeleteSelectedStampPreset()
    {
        var deletedPresetName = selectedStampPresetName;
        if (!board.RemovePreset(selectedStampPresetName))
        {
            WorkspaceService.State.LastOperationMessage = "Select a preset to delete.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        WorkspaceService.State.LastOperationMessage = $"Deleted stamp preset '{deletedPresetName}'.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        selectedStampPresetName = string.Empty;
    }

    private string? GetSurfaceDefaultPresetName()
    {
        return board.GetDefaultPresetForSurface(board.ActiveSurfaceId)?.Name;
    }

    private bool IsActivePresetSurfaceDefault()
    {
        var surfaceDefaultName = GetSurfaceDefaultPresetName();
        return !string.IsNullOrWhiteSpace(board.ActiveStampPresetName) &&
               !string.IsNullOrWhiteSpace(surfaceDefaultName) &&
               string.Equals(board.ActiveStampPresetName, surfaceDefaultName, StringComparison.Ordinal);
    }

    private void ApplyPresetToQuickCreate(StampPreset preset, bool setMessage)
    {
        quickCreateDefinitionId = preset.DefinitionId;
        quickCreateOwnerParticipantId = preset.OwnerParticipantId;
        quickCreateRotation = preset.InitialRotation;

        if (rememberDefinitionPerSurface && !string.IsNullOrWhiteSpace(board.ActiveSurfaceId) && !string.IsNullOrWhiteSpace(quickCreateDefinitionId))
        {
            board.RememberDefinitionForSurface(board.ActiveSurfaceId, quickCreateDefinitionId);
        }

        if (setMessage)
        {
            WorkspaceService.State.LastOperationMessage = $"Applied stamp preset '{preset.Name}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
    }

    private void AddCurrentSetupToStampQueue()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(board.ActiveSurfaceId))
            {
                WorkspaceService.State.LastOperationMessage = "Select an active surface first.";
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
                return;
            }

            var queueItem = board.AddQueueItem(
                board.ActiveSurfaceId,
                board.ActiveStampPresetName,
                quickCreatePieceId,
                stampQueueNote);

            WorkspaceService.State.LastOperationMessage = $"Added queue item for surface '{queueItem.SurfaceId}' with next id '{queueItem.NextPieceId}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void ApplyQueueItem(int index)
    {
        var session = WorkspaceService.State.CurrentTableSession;
        if (!board.TryApplyQueueItemToBoardContext(
            index,
            session,
            clearSelectionWhenSwitchingSurfaces: true,
            out var queueItem,
            out var appliedPreset,
            out var clearedSelection,
            out var statusMessage))
        {
            WorkspaceService.State.LastOperationMessage = statusMessage;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        if (appliedPreset is not null)
        {
            ApplyPresetToQuickCreate(appliedPreset, setMessage: false);
            selectedStampPresetName = appliedPreset.Name;
        }

        quickCreatePieceId = queueItem!.NextPieceId;
        quickCreateBasePieceId = queueItem.NextPieceId;
        selectedPieceId = board.SelectedPieceId;
        previousActiveSurfaceId = board.ActiveSurfaceId;
        addPieceAtBoardClickMode = true;

        WorkspaceService.State.LastOperationMessage = clearedSelection
            ? $"Applied queue item {index + 1} and cleared off-surface selection."
            : $"Applied queue item {index + 1} to board context.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void ApplyNextQueueItem()
    {
        var session = WorkspaceService.State.CurrentTableSession;
        if (!board.TryApplyNextQueueItemToBoardContext(
            session,
            clearSelectionWhenSwitchingSurfaces: true,
            out var queueItem,
            out var appliedPreset,
            out var clearedSelection,
            out var statusMessage))
        {
            WorkspaceService.State.LastOperationMessage = statusMessage;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        if (appliedPreset is not null)
        {
            ApplyPresetToQuickCreate(appliedPreset, setMessage: false);
            selectedStampPresetName = appliedPreset.Name;
        }

        quickCreatePieceId = queueItem!.NextPieceId;
        quickCreateBasePieceId = queueItem.NextPieceId;
        selectedPieceId = board.SelectedPieceId;
        previousActiveSurfaceId = board.ActiveSurfaceId;
        addPieceAtBoardClickMode = true;

        WorkspaceService.State.LastOperationMessage = clearedSelection
            ? "Applied next queue item and cleared off-surface selection."
            : "Applied next queue item to board context.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void RemoveQueueItem(int index)
    {
        if (!board.RemoveQueueItem(index))
        {
            WorkspaceService.State.LastOperationMessage = "Queue item was not found.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        WorkspaceService.State.LastOperationMessage = $"Removed queue item {index + 1}.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void MoveQueueItemUp(int index)
    {
        if (!board.MoveQueueItemUp(index))
        {
            WorkspaceService.State.LastOperationMessage = "Queue item cannot move up.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        WorkspaceService.State.LastOperationMessage = $"Moved queue item {index + 1} up.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void MoveQueueItemDown(int index)
    {
        if (!board.MoveQueueItemDown(index))
        {
            WorkspaceService.State.LastOperationMessage = "Queue item cannot move down.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        WorkspaceService.State.LastOperationMessage = $"Moved queue item {index + 1} down.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void UpdateQueueItemNextId(int index, ChangeEventArgs args)
    {
        var nextId = args.Value?.ToString();
        if (!board.UpdateQueueItemNextId(index, nextId ?? string.Empty))
        {
            WorkspaceService.State.LastOperationMessage = "Next id must be provided for queue item.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        if (index == board.ActiveStampQueueIndex)
        {
            quickCreatePieceId = nextId ?? string.Empty;
            quickCreateBasePieceId = nextId ?? string.Empty;
        }

        WorkspaceService.State.LastOperationMessage = $"Updated queue item {index + 1} next id to '{nextId}'.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void SaveQueueAsTemplate()
    {
        try
        {
            var template = board.SaveQueueAsTemplate(queueTemplateNameInput);
            selectedQueueTemplateName = template.TemplateName;
            queueTemplateRenameBufferByName[template.TemplateName] = template.TemplateName;
            queueTemplateNameInput = string.Empty;
            WorkspaceService.State.LastOperationMessage = $"Saved queue template '{template.TemplateName}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void ApplyQueueTemplate(string templateName)
    {
        try
        {
            var template = board.ApplyTemplateToQueue(templateName);
            selectedQueueTemplateName = template.TemplateName;
            WorkspaceService.State.LastOperationMessage = $"Applied queue template '{template.TemplateName}' to current queue.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void DeleteQueueTemplate(string templateName)
    {
        if (!board.DeleteQueueTemplate(templateName))
        {
            WorkspaceService.State.LastOperationMessage = "Template was not found.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        queueTemplateRenameBufferByName.Remove(templateName);
        if (string.Equals(selectedQueueTemplateName, templateName, StringComparison.Ordinal))
        {
            selectedQueueTemplateName = null;
        }

        WorkspaceService.State.LastOperationMessage = $"Deleted queue template '{templateName}'.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
    }

    private void RenameQueueTemplate(string templateName)
    {
        if (!queueTemplateRenameBufferByName.TryGetValue(templateName, out var targetName))
        {
            targetName = templateName;
        }

        try
        {
            var renamed = board.RenameQueueTemplate(templateName, targetName);
            queueTemplateRenameBufferByName.Remove(templateName);
            queueTemplateRenameBufferByName[renamed.TemplateName] = renamed.TemplateName;
            selectedQueueTemplateName = renamed.TemplateName;
            WorkspaceService.State.LastOperationMessage = $"Renamed queue template to '{renamed.TemplateName}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void DuplicateQueueTemplate(string templateName)
    {
        try
        {
            var duplicated = board.DuplicateQueueTemplate(templateName);
            selectedQueueTemplateName = duplicated.TemplateName;
            queueTemplateRenameBufferByName[duplicated.TemplateName] = duplicated.TemplateName;
            WorkspaceService.State.LastOperationMessage = $"Duplicated queue template as '{duplicated.TemplateName}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private string GetTemplateRenameValue(string templateName)
    {
        if (!queueTemplateRenameBufferByName.TryGetValue(templateName, out var value))
        {
            value = templateName;
            queueTemplateRenameBufferByName[templateName] = value;
        }

        return value;
    }

    private void SetTemplateRenameValue(string templateName, ChangeEventArgs args)
    {
        queueTemplateRenameBufferByName[templateName] = args.Value?.ToString() ?? string.Empty;
    }

    private void OnActiveSurfaceChanged(ChangeEventArgs args)
    {
        ActivateSurfaceWorkflow(args.Value?.ToString(), includeSuccessMessage: true);
    }

    private void ActivateSurfaceFromStrip(string surfaceId)
    {
        ActivateSurfaceWorkflow(surfaceId, includeSuccessMessage: true);
    }

    private void ActivateSurfaceWorkflow(string? surfaceId, bool includeSuccessMessage)
    {
        var session = WorkspaceService.State.CurrentTableSession;
        if (session is null)
        {
            WorkspaceService.State.LastOperationMessage = "Current session is required before switching surfaces.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        if (rememberDefinitionPerSurface &&
            !string.IsNullOrWhiteSpace(previousActiveSurfaceId) &&
            !string.IsNullOrWhiteSpace(quickCreateDefinitionId))
        {
            board.RememberDefinitionForSurface(previousActiveSurfaceId, quickCreateDefinitionId);
        }

        if (!board.TrySwitchActiveSurface(session, surfaceId, clearSelectionWhenSwitchingSurfaces: true, out var clearedSelection, out var switchMessage))
        {
            WorkspaceService.State.LastOperationMessage = switchMessage;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        previousActiveSurfaceId = board.ActiveSurfaceId;
        selectedPieceId = board.SelectedPieceId;

        if (rememberDefinitionPerSurface &&
            !string.IsNullOrWhiteSpace(board.ActiveSurfaceId) &&
            board.TryGetRememberedDefinitionForSurface(board.ActiveSurfaceId) is { } rememberedDefinition)
        {
            quickCreateDefinitionId = rememberedDefinition;
        }

        ApplyBoardVisibilityRules();

        if (board.GetDefaultPresetForSurface(board.ActiveSurfaceId) is { } defaultPreset)
        {
            ApplyPresetToQuickCreate(defaultPreset, setMessage: false);
            selectedStampPresetName = defaultPreset.Name;

            if (includeSuccessMessage)
            {
                WorkspaceService.State.LastOperationMessage = $"Switched to '{board.ActiveSurfaceId}' and applied default preset '{defaultPreset.Name}'.";
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
            }

            return;
        }

        if (includeSuccessMessage)
        {
            WorkspaceService.State.LastOperationMessage = clearedSelection
                ? $"Switched to '{board.ActiveSurfaceId}'. Cleared selected piece because it was on another surface."
                : $"Switched to '{board.ActiveSurfaceId}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
    }

    private string GetSurfaceStripPresetValue(string surfaceId)
    {
        return board.GetDefaultPresetForSurface(surfaceId)?.Name ?? string.Empty;
    }

    private void OnSurfaceStripPresetChanged(string surfaceId, ChangeEventArgs args)
    {
        var presetName = args.Value?.ToString();

        if (string.IsNullOrWhiteSpace(presetName))
        {
            board.ClearDefaultPresetForSurface(surfaceId);
            WorkspaceService.State.LastOperationMessage = $"Cleared default preset for surface '{surfaceId}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
            return;
        }

        try
        {
            board.SetDefaultPresetForSurface(surfaceId, presetName);

            if (string.Equals(surfaceId, board.ActiveSurfaceId, StringComparison.Ordinal))
            {
                var preset = board.ApplyPreset(presetName);
                ApplyPresetToQuickCreate(preset, setMessage: false);
                selectedStampPresetName = preset.Name;
            }

            WorkspaceService.State.LastOperationMessage = $"Set '{presetName}' as default preset for surface '{surfaceId}'.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void ApplyActivePresetToSurface(string surfaceId)
    {
        if (string.IsNullOrWhiteSpace(board.ActiveStampPresetName))
        {
            WorkspaceService.State.LastOperationMessage = "Select or apply a preset first.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            return;
        }

        try
        {
            board.SetDefaultPresetForSurface(surfaceId, board.ActiveStampPresetName);
            WorkspaceService.State.LastOperationMessage = $"Applied active preset '{board.ActiveStampPresetName}' to surface '{surfaceId}' default.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void QuickAddSurfaceFromStrip()
    {
        Execute(() =>
        {
            var surfaceId = quickSurfaceId.Trim();
            var definitionId = string.IsNullOrWhiteSpace(quickSurfaceDefinitionId)
                ? $"def-{surfaceId}"
                : quickSurfaceDefinitionId.Trim();

            WorkspaceService.AddSurface(surfaceId, definitionId, quickSurfaceType, quickSurfaceCoordinateSystem);
            duplicateSurfaceId = $"{surfaceId}-copy";
            ActivateSurfaceWorkflow(surfaceId, includeSuccessMessage: false);
        }, "Added surface from surface workflow strip.");
    }

    private void DuplicateActiveSurfaceFromStrip()
    {
        Execute(() =>
        {
            var session = WorkspaceService.State.CurrentTableSession ?? throw new InvalidOperationException("Current session is required.");
            if (string.IsNullOrWhiteSpace(board.ActiveSurfaceId))
            {
                throw new InvalidOperationException("Select an active surface first.");
            }

            var sourceSurface = session.Surfaces.FirstOrDefault(s => s.Id == board.ActiveSurfaceId)
                ?? throw new InvalidOperationException("Active surface was not found.");

            var newSurfaceId = duplicateSurfaceId.Trim();
            WorkspaceService.AddSurface(newSurfaceId, sourceSurface.DefinitionId, sourceSurface.Type, sourceSurface.CoordinateSystem);
            ActivateSurfaceWorkflow(newSurfaceId, includeSuccessMessage: false);
        }, "Duplicated active surface configuration to new surface id.");
    }

    private void OnQuickCreatePieceIdEdited()
    {
        quickCreateBasePieceId = quickCreatePieceId;
    }

    private void OnQuickCreateDefinitionEdited()
    {
        if (!rememberDefinitionPerSurface || string.IsNullOrWhiteSpace(board.ActiveSurfaceId) || string.IsNullOrWhiteSpace(quickCreateDefinitionId))
        {
            return;
        }

        board.RememberDefinitionForSurface(board.ActiveSurfaceId, quickCreateDefinitionId);
    }

    private void ClearNextIdPreview()
    {
        quickCreatePieceId = string.Empty;
        WorkspaceService.State.LastOperationMessage = "Cleared next id preview.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
    }

    private void ResetNextIdPreviewToBase()
    {
        quickCreatePieceId = board.ResetPieceIdPreview(quickCreateBasePieceId);
        WorkspaceService.State.LastOperationMessage = "Reset next id preview to base id.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
    }

    private void NudgeNextIdUp()
    {
        NudgeNextId(1);
    }

    private void NudgeNextIdDown()
    {
        NudgeNextId(-1);
    }

    private void NudgeNextId(int delta)
    {
        try
        {
            quickCreatePieceId = board.NudgePieceIdPreview(quickCreatePieceId, delta);
            WorkspaceService.State.LastOperationMessage = delta > 0 ? "Advanced next id preview by +1." : "Decremented next id preview by -1.";
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void BoardClicked(MouseEventArgs args)
    {
        if (!WorkspaceService.State.Settings.EnableBoardInteraction)
        {
            WorkspaceService.ReportFeatureNotEnabled();
            return;
        }

        var offsetX = (float)Math.Clamp(args.OffsetX, 0, BoardMaxX);
        var offsetY = (float)Math.Clamp(args.OffsetY, 0, BoardMaxY);
        lastBoardClickX = offsetX;
        lastBoardClickY = offsetY;
        moveX = offsetX;
        moveY = offsetY;

        try
        {
            if (board.TryHandleBoardClick(
                CommandService,
                moveX,
                moveY,
                addPieceAtBoardClickMode,
                stampModeEnabled,
                quickCreatePieceId,
                quickCreateDefinitionId,
                quickCreateOwnerParticipantId,
                quickCreateRotation,
                clampToBoard,
                BoardMaxX,
                BoardMaxY,
                out var statusMessage,
                out var createdPieceId,
                out var keepAddPieceAtBoardClickMode,
                out var nextSuggestedPieceId))
            {
                if (!string.IsNullOrWhiteSpace(createdPieceId))
                {
                    selectedPieceId = createdPieceId;
                    board.SelectedPieceId = createdPieceId;

                    if (!string.IsNullOrWhiteSpace(nextSuggestedPieceId))
                    {
                        quickCreatePieceId = nextSuggestedPieceId;
                        quickCreateBasePieceId = nextSuggestedPieceId;
                        _ = board.TryUpdateActiveQueueItemNextId(nextSuggestedPieceId);
                    }

                    if (rememberDefinitionPerSurface && !string.IsNullOrWhiteSpace(board.ActiveSurfaceId) && !string.IsNullOrWhiteSpace(quickCreateDefinitionId))
                    {
                        board.RememberDefinitionForSurface(board.ActiveSurfaceId, quickCreateDefinitionId);
                    }

                    addPieceAtBoardClickMode = keepAddPieceAtBoardClickMode;
                }

                WorkspaceService.State.LastOperationMessage = statusMessage;
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
            }
            else
            {
                WorkspaceService.State.LastOperationMessage = statusMessage;
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            }
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }

    }

    private void OnBoardFocused(FocusEventArgs _)
    {
        isBoardFocused = true;
    }

    private void OnBoardBlurred(FocusEventArgs _)
    {
        isBoardFocused = false;
    }

    private void OnBoardKeyDown(KeyboardEventArgs args)
    {
        if (!WorkspaceService.State.Settings.EnableBoardInteraction)
        {
            WorkspaceService.ReportFeatureNotEnabled();
            return;
        }

        try
        {
            if (board.TryHandleKeyboardInput(
                CommandService,
                args.Key,
                isBoardFocused,
                selectedNudgeStep,
                QuickRotateIncrement,
                clampToBoard,
                BoardMaxX,
                BoardMaxY,
                out var statusMessage))
            {
                WorkspaceService.State.LastOperationMessage = statusMessage;
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
            }
            else if (!string.IsNullOrWhiteSpace(statusMessage))
            {
                WorkspaceService.State.LastOperationMessage = statusMessage;
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            }
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void MoveFromBoardPanel()
    {
        Execute(() =>
        {
            board.MoveSelectedPiece(CommandService, moveX, moveY, clampToBoard, BoardMaxX, BoardMaxY);
            board.SelectedPieceId = selectedPieceId;
        }, "Moved piece from board panel.");
    }

    private void CenterSelected()
    {
        try
        {
            if (board.TryCenterSelectedPiece(
                CommandService,
                BoardMaxX / 2,
                BoardMaxY / 2,
                clampToBoard,
                BoardMaxX,
                BoardMaxY,
                out var statusMessage))
            {
                WorkspaceService.State.LastOperationMessage = statusMessage;
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
            }
            else
            {
                WorkspaceService.State.LastOperationMessage = statusMessage;
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            }
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
        }
    }

    private void RotateFromBoardPanel()
    {
        Execute(() =>
        {
            board.RotateSelectedPiece(CommandService, rotateDegrees);
            board.SelectedPieceId = selectedPieceId;
        }, "Rotated piece from board panel.");
    }

    private void RotateLeftQuick()
    {
        Execute(() =>
        {
            var selectedPiece = board.GetSelectedPieceOrThrow(CommandService.CurrentTableSession);
            rotateDegrees = selectedPiece.Rotation.Degrees - QuickRotateIncrement;
            board.RotateSelectedPiece(CommandService, rotateDegrees);
            board.SelectedPieceId = selectedPieceId;
        }, "Rotated selected piece left.");
    }

    private void RotateRightQuick()
    {
        Execute(() =>
        {
            var selectedPiece = board.GetSelectedPieceOrThrow(CommandService.CurrentTableSession);
            rotateDegrees = selectedPiece.Rotation.Degrees + QuickRotateIncrement;
            board.RotateSelectedPiece(CommandService, rotateDegrees);
            board.SelectedPieceId = selectedPieceId;
        }, "Rotated selected piece right.");
    }

    private void MovePiece()
    {
        Execute(() =>
        {
            var pieceId = RequireSelectedPiece();
            CommandService.ProcessAction(new ActionRequest
            {
                ActionType = "MovePiece",
                ActorParticipantId = "gm",
                Payload = new MovePiecePayload
                {
                    PieceId = pieceId,
                    NewLocation = new Location
                    {
                        SurfaceId = moveSurfaceId,
                        Coordinate = new Coordinate { X = moveX, Y = moveY }
                    }
                }
            });
        }, "Moved piece.");
    }

    private void RotatePiece()
    {
        Execute(() =>
        {
            var pieceId = RequireSelectedPiece();
            CommandService.ProcessAction(new ActionRequest
            {
                ActionType = "RotatePiece",
                ActorParticipantId = "gm",
                Payload = new RotatePiecePayload
                {
                    PieceId = pieceId,
                    NewRotation = new Rotation { Degrees = rotateDegrees }
                }
            });
        }, "Rotated piece.");
    }

    private void AddMarker()
    {
        Execute(() =>
        {
            var pieceId = RequireSelectedPiece();
            CommandService.ProcessAction(new ActionRequest
            {
                ActionType = "AddMarker",
                ActorParticipantId = "gm",
                Payload = new AddMarkerPayload
                {
                    PieceId = pieceId,
                    MarkerId = markerId
                }
            });
        }, "Added marker.");
    }

    private void RemoveMarker()
    {
        Execute(() =>
        {
            var pieceId = RequireSelectedPiece();
            CommandService.ProcessAction(new ActionRequest
            {
                ActionType = "RemoveMarker",
                ActorParticipantId = "gm",
                Payload = new RemoveMarkerPayload
                {
                    PieceId = pieceId,
                    MarkerId = markerId
                }
            });
        }, "Removed marker.");
    }

    private void ChangePieceState()
    {
        Execute(() =>
        {
            var pieceId = RequireSelectedPiece();
            CommandService.ProcessAction(new ActionRequest
            {
                ActionType = "ChangePieceState",
                ActorParticipantId = "gm",
                Payload = new ChangePieceStatePayload
                {
                    PieceId = pieceId,
                    Key = stateKey,
                    Value = stateValue
                }
            });
        }, "Changed piece state.");
    }

    private void RemovePieceState()
    {
        Execute(() =>
        {
            var pieceId = RequireSelectedPiece();
            CommandService.ProcessAction(new ActionRequest
            {
                ActionType = "ChangePieceState",
                ActorParticipantId = "gm",
                Payload = new ChangePieceStatePayload
                {
                    PieceId = pieceId,
                    Key = removeStateKey,
                    Value = null
                }
            });
        }, "Removed piece state key.");
    }

    private void SetPieceHpState()
    {
        Execute(() =>
        {
            var pieceId = RequireSelectedPiece();
            CommandService.ProcessAction(new ActionRequest
            {
                ActionType = "ChangePieceState",
                ActorParticipantId = "gm",
                Payload = new ChangePieceStatePayload
                {
                    PieceId = pieceId,
                    Key = "HP",
                    Value = quickHpValue
                }
            });
        }, "Updated HP state.");
    }

    private void OnDeveloperSettingsChanged()
    {
        if (!WorkspaceService.State.Settings.EnableStampMode)
        {
            stampModeEnabled = false;
        }

        WorkspaceService.State.LastOperationMessage = "Updated developer settings.";
        WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
    }

    private static string GetPieceLabel(PieceInstance piece)
    {
        if (piece.State.TryGetValue("Label", out var labelValue))
        {
            var formatted = FormatStateValue(labelValue);
            if (!string.IsNullOrWhiteSpace(formatted))
            {
                return formatted;
            }
        }

        if (piece.State.TryGetValue("Name", out var nameValue))
        {
            var formatted = FormatStateValue(nameValue);
            if (!string.IsNullOrWhiteSpace(formatted))
            {
                return formatted;
            }
        }

        return piece.Id;
    }

    private static string GetPieceStateDisplayValue(PieceInstance piece, string key)
    {
        if (!piece.State.TryGetValue(key, out var value))
        {
            return "(none)";
        }

        return FormatStateValue(value);
    }

    private static string GetMarkerSummary(PieceInstance piece)
    {
        if (piece.MarkerIds.Count == 0)
        {
            return "0";
        }

        return $"{piece.MarkerIds.Count} ({string.Join(", ", piece.MarkerIds)})";
    }

    private static string FormatStateValue(object? value)
    {
        return value?.ToString() ?? "(null)";
    }

    private void Execute(Action action, string successMessage)
    {
        try
        {
            var previousMessage = WorkspaceService.State.LastOperationMessage;
            action();

            if (string.Equals(previousMessage, WorkspaceService.State.LastOperationMessage, StringComparison.Ordinal))
            {
                WorkspaceService.State.LastOperationMessage = successMessage;
                WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            WorkspaceService.State.LastOperationMessage = ex.Message;
            WorkspaceService.State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
            StateHasChanged();
        }
    }

    private string GetLastOperationAlertClass()
    {
        return WorkspaceService.State.LastOperationSeverity switch
        {
            WorkspaceMessageSeverity.Success => "alert-success",
            WorkspaceMessageSeverity.Error => "alert-danger",
            _ => "alert-warning"
        };
    }

    private string RequireSelectedPiece()
    {
        if (string.IsNullOrWhiteSpace(selectedPieceId))
        {
            throw new InvalidOperationException("Select a piece first.");
        }

        return selectedPieceId;
    }

    private string GetPieceCss(PieceInstance piece)
    {
        var css = "board-piece";
        if (board.IsPieceSelected(piece.Id))
        {
            css += " selected";
        }

        if (board.IsPrimarySelectedPiece(piece.Id))
        {
            css += " primary";
        }

        return css;
    }

    private string GetPieceStyle(PieceInstance piece)
    {
        var left = Math.Clamp(piece.Location.Coordinate.X, 0, 620);
        var top = Math.Clamp(piece.Location.Coordinate.Y, 0, 360);
        return $"left: {left}px; top: {top}px; transform: rotate({piece.Rotation.Degrees}deg);";
    }

    private bool TryGetSelectedPiece(out PieceInstance selectedPiece)
    {
        selectedPiece = null!;
        var session = WorkspaceService.State.CurrentTableSession;
        if (session is null || string.IsNullOrWhiteSpace(selectedPieceId))
        {
            return false;
        }

        var piece = session.Pieces.FirstOrDefault(p => p.Id == selectedPieceId);
        if (piece is null)
        {
            return false;
        }

        selectedPiece = piece;
        return true;
    }

    private Task OnSessionPathChangedFromPanel(string? value)
    {
        sessionPath = value;
        return Task.CompletedTask;
    }

    private Task OnScenarioPathChangedFromPanel(string? value)
    {
        scenarioPath = value;
        return Task.CompletedTask;
    }

    private Task OnScenarioTitleChangedFromPanel(string value)
    {
        scenarioTitle = value;
        return Task.CompletedTask;
    }

    private Task OnSessionTitleInputChangedFromPanel(string value)
    {
        sessionTitleInput = value;
        return Task.CompletedTask;
    }

    private void ToggleWorkspacePanel()
    {
        isWorkspacePanelOpen = !isWorkspacePanelOpen;
    }

    private void CloseWorkspacePanel()
    {
        isWorkspacePanelOpen = false;
    }
}
