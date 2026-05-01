using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;
using MIMLESvtt.src;
using MIMLESvtt.Services.Actions;

namespace MIMLESvtt.Services.Workspace;

public class WorkspaceBoardState
{
    private readonly Dictionary<string, string> _definitionBySurface = new(StringComparer.Ordinal);
    private readonly List<StampPreset> _stampPresets = [];
    private readonly Dictionary<string, string> _defaultPresetBySurface = new(StringComparer.Ordinal);
    private readonly List<StampQueueItem> _stampQueue = [];
    private readonly List<StampQueueTemplate> _stampQueueTemplates = [];
    private readonly List<PieceGroup> _pieceGroups = [];

    public string? ActiveSurfaceId { get; set; }

    public string? SelectedPieceId { get; set; }

    public List<string> SelectedPieceIds { get; } = [];

    public IReadOnlyList<StampPreset> StampPresets => _stampPresets;

    public IReadOnlyList<StampQueueItem> StampQueue => _stampQueue;

    public IReadOnlyList<StampQueueTemplate> StampQueueTemplates => _stampQueueTemplates;

    public IReadOnlyList<PieceGroup> PieceGroups => _pieceGroups;

    public int ActiveStampQueueIndex { get; private set; } = -1;

    public string? ActiveStampQueueTemplateName { get; private set; }

    public string? ActiveStampPresetName { get; private set; }

    public void EnsureActiveSurface(VttSession? session)
    {
        if (session is null || session.Surfaces.Count == 0)
        {
            ActiveSurfaceId = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(ActiveSurfaceId) || !session.Surfaces.Any(s => s.Id == ActiveSurfaceId))
        {
            ActiveSurfaceId = session.Surfaces[0].Id;
        }
    }

    public void SetActiveSurface(string? surfaceId)
    {
        ActiveSurfaceId = surfaceId;
    }

    public bool TrySwitchActiveSurface(
        VttSession? session,
        string? surfaceId,
        bool clearSelectionWhenSwitchingSurfaces,
        out bool clearedSelection,
        out string? statusMessage)
    {
        clearedSelection = false;
        statusMessage = null;

        if (session is null)
        {
            statusMessage = "Current session is required to switch surfaces.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(surfaceId) || session.Surfaces.All(s => s.Id != surfaceId))
        {
            statusMessage = "Selected surface was not found in current session.";
            return false;
        }

        ActiveSurfaceId = surfaceId;

        if (!clearSelectionWhenSwitchingSurfaces || string.IsNullOrWhiteSpace(SelectedPieceId))
        {
            return true;
        }

        var selectedPiece = session.Pieces.FirstOrDefault(p => p.Id == SelectedPieceId);
        if (selectedPiece is null)
        {
            SelectedPieceId = null;
            SelectedPieceIds.Clear();
            clearedSelection = true;
            return true;
        }

        if (!string.Equals(selectedPiece.Location.SurfaceId, surfaceId, StringComparison.Ordinal))
        {
            SelectedPieceId = null;
            SelectedPieceIds.Clear();
            clearedSelection = true;
        }

        return true;
    }

    public void SelectPiece(string pieceId)
    {
        SelectSinglePiece(pieceId);
    }

    public void SelectSinglePiece(string pieceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pieceId);

        SelectedPieceId = pieceId;
        SelectedPieceIds.Clear();
        SelectedPieceIds.Add(pieceId);
    }

    public void TogglePieceSelection(string pieceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pieceId);

        if (SelectedPieceIds.Contains(pieceId))
        {
            SelectedPieceIds.Remove(pieceId);
            if (string.Equals(SelectedPieceId, pieceId, StringComparison.Ordinal))
            {
                SelectedPieceId = SelectedPieceIds.Count > 0 ? SelectedPieceIds[^1] : null;
            }

            return;
        }

        SelectedPieceIds.Add(pieceId);
        SelectedPieceId = pieceId;
    }

    public void ClearSelection()
    {
        SelectedPieceId = null;
        SelectedPieceIds.Clear();
    }

    public int SelectAllPiecesOnActiveSurface(VttSession? session)
    {
        if (session is null || string.IsNullOrWhiteSpace(ActiveSurfaceId))
        {
            ClearSelection();
            return 0;
        }

        SelectedPieceIds.Clear();
        var pieceIds = session.Pieces
            .Where(p => string.Equals(p.Location.SurfaceId, ActiveSurfaceId, StringComparison.Ordinal))
            .Select(p => p.Id)
            .ToList();

        SelectedPieceIds.AddRange(pieceIds);
        SelectedPieceId = SelectedPieceIds.Count > 0 ? SelectedPieceIds[^1] : null;
        return SelectedPieceIds.Count;
    }

    public bool IsPieceSelected(string pieceId)
    {
        return SelectedPieceIds.Contains(pieceId);
    }

    public bool IsPrimarySelectedPiece(string pieceId)
    {
        return string.Equals(SelectedPieceId, pieceId, StringComparison.Ordinal);
    }

    public int MoveSelectedPiecesByDelta(
        ISessionCommandService commandService,
        float deltaX,
        float deltaY,
        bool clampToBoard,
        float maxX,
        float maxY)
    {
        if (commandService.CurrentVttSession is null)
        {
            throw new InvalidOperationException("Current session is required.");
        }

        if (SelectedPieceIds.Count == 0)
        {
            throw new InvalidOperationException("Select at least one piece first.");
        }

        var movedCount = 0;
        var selectedIds = SelectedPieceIds.ToList();
        foreach (var pieceId in selectedIds)
        {
            var piece = commandService.CurrentVttSession.Pieces.FirstOrDefault(p => p.Id == pieceId);
            if (piece is null)
            {
                continue;
            }

            var targetX = piece.Location.Coordinate.X + deltaX;
            var targetY = piece.Location.Coordinate.Y + deltaY;
            if (clampToBoard)
            {
                targetX = Math.Clamp(targetX, 0, maxX);
                targetY = Math.Clamp(targetY, 0, maxY);
            }

            commandService.ProcessAction(new ActionRequest
            {
                ActionType = "MovePiece",
                ActorParticipantId = "gm",
                Payload = new MovePiecePayload
                {
                    PieceId = piece.Id,
                    NewLocation = new Location
                    {
                        SurfaceId = piece.Location.SurfaceId,
                        Coordinate = new Coordinate { X = targetX, Y = targetY }
                    }
                }
            });

            movedCount++;
        }

        return movedCount;
    }

    public PieceGroup CreateGroupFromSelection(string? groupLabel)
    {
        if (SelectedPieceIds.Count == 0)
        {
            throw new InvalidOperationException("Select at least one piece to create a group.");
        }

        var groupId = $"group-{Guid.NewGuid():N}";
        var normalizedLabel = string.IsNullOrWhiteSpace(groupLabel)
            ? $"Group {_pieceGroups.Count + 1}"
            : groupLabel.Trim();

        var group = new PieceGroup(
            groupId,
            normalizedLabel,
            SelectedPieceIds.Distinct(StringComparer.Ordinal).ToList());

        _pieceGroups.Add(group);
        return group;
    }

    public bool DisbandGroup(string? groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            return false;
        }

        var group = _pieceGroups.FirstOrDefault(g => string.Equals(g.GroupId, groupId, StringComparison.Ordinal));
        if (group is null)
        {
            return false;
        }

        return _pieceGroups.Remove(group);
    }

    public int SelectGroup(string? groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            ClearSelection();
            return 0;
        }

        var group = _pieceGroups.FirstOrDefault(g => string.Equals(g.GroupId, groupId, StringComparison.Ordinal));
        if (group is null)
        {
            ClearSelection();
            return 0;
        }

        SelectedPieceIds.Clear();
        SelectedPieceIds.AddRange(group.MemberPieceIds);
        SelectedPieceId = SelectedPieceIds.Count > 0 ? SelectedPieceIds[^1] : null;
        return SelectedPieceIds.Count;
    }

    public int MoveGroupByDelta(
        VttSessionWorkspaceService workspaceService,
        string groupId,
        float deltaX,
        float deltaY,
        bool clampToBoard,
        float maxX,
        float maxY)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(groupId);

        var selectedCount = SelectGroup(groupId);
        if (selectedCount == 0)
        {
            throw new InvalidOperationException("Selected group was not found or has no members.");
        }

        return MoveSelectedPiecesByDelta(workspaceService, deltaX, deltaY, clampToBoard, maxX, maxY);
    }

    public string GetGroupMembersSummary(string groupId)
    {
        var group = _pieceGroups.FirstOrDefault(g => string.Equals(g.GroupId, groupId, StringComparison.Ordinal));
        if (group is null || group.MemberPieceIds.Count == 0)
        {
            return "(none)";
        }

        return string.Join(", ", group.MemberPieceIds);
    }

    public string GetSelectionSummary()
    {
        if (SelectedPieceIds.Count == 0)
        {
            return "Selected: 0";
        }

        var preview = string.Join(", ", SelectedPieceIds.Take(3));
        if (SelectedPieceIds.Count > 3)
        {
            preview += ", ...";
        }

        return $"Selected: {SelectedPieceIds.Count} [{preview}]";
    }

    public IReadOnlyList<PieceInstance> GetRenderedPiecesForActiveSurface(VttSession? session)
    {
        return GetRenderedPiecesForBoard(session, new BoardVisibilityOptions
        {
            ShowPieces = true,
            ActiveSurfaceOnly = true
        });
    }

    public IReadOnlyList<PieceInstance> GetRenderedPiecesForBoard(VttSession? session, BoardVisibilityOptions visibility)
    {
        if (session is null || !visibility.ShowPieces)
        {
            return [];
        }

        IEnumerable<PieceInstance> pieces = session.Pieces;
        if (visibility.ActiveSurfaceOnly)
        {
            if (string.IsNullOrWhiteSpace(ActiveSurfaceId))
            {
                return [];
            }

            pieces = pieces.Where(p => string.Equals(p.Location.SurfaceId, ActiveSurfaceId, StringComparison.Ordinal));
        }

        if (!string.IsNullOrWhiteSpace(visibility.DefinitionIdFilter))
        {
            pieces = pieces.Where(p => p.DefinitionId.Contains(visibility.DefinitionIdFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(visibility.OwnerParticipantIdFilter))
        {
            pieces = pieces.Where(p => p.OwnerParticipantId.Contains(visibility.OwnerParticipantIdFilter, StringComparison.OrdinalIgnoreCase));
        }

        return pieces.ToList();
    }

    public bool ShouldRenderSurfaceVisuals(bool showSurfaces)
    {
        return showSurfaces && !string.IsNullOrWhiteSpace(ActiveSurfaceId);
    }

    public bool ClearHiddenSelections(VttSession? session, BoardVisibilityOptions visibility)
    {
        if (SelectedPieceIds.Count == 0)
        {
            return false;
        }

        var visiblePieceIds = GetRenderedPiecesForBoard(session, visibility)
            .Select(p => p.Id)
            .ToHashSet(StringComparer.Ordinal);

        var before = SelectedPieceIds.Count;
        SelectedPieceIds.RemoveAll(pieceId => !visiblePieceIds.Contains(pieceId));

        if (!string.IsNullOrWhiteSpace(SelectedPieceId) && !visiblePieceIds.Contains(SelectedPieceId))
        {
            SelectedPieceId = SelectedPieceIds.Count > 0 ? SelectedPieceIds[^1] : null;
        }

        if (SelectedPieceIds.Count == 0)
        {
            SelectedPieceId = null;
        }

        return before != SelectedPieceIds.Count;
    }

    public ActionRecord MoveSelectedPiece(ISessionCommandService commandService, float x, float y)
    {
        return MoveSelectedPiece(commandService, x, y, clampToBoard: false, maxX: 0, maxY: 0);
    }

    public ActionRecord MoveSelectedPiece(
        ISessionCommandService commandService,
        float x,
        float y,
        bool clampToBoard,
        float maxX,
        float maxY)
    {
        var selectedPiece = RequireSelectedPiece(commandService.CurrentVttSession);
        if (string.IsNullOrWhiteSpace(ActiveSurfaceId))
        {
            throw new InvalidOperationException("Select an active surface first.");
        }

        var targetX = clampToBoard ? Math.Clamp(x, 0, maxX) : x;
        var targetY = clampToBoard ? Math.Clamp(y, 0, maxY) : y;

        return commandService.ProcessAction(new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "gm",
            Payload = new MovePiecePayload
            {
                PieceId = selectedPiece.Id,
                NewLocation = new Location
                {
                    SurfaceId = ActiveSurfaceId,
                    Coordinate = new Coordinate { X = targetX, Y = targetY }
                }
            }
        });
    }

    public bool TryHandleKeyboardInput(
        ISessionCommandService commandService,
        string? key,
        bool isBoardFocused,
        float moveStep,
        float rotateStep,
        bool clampToBoard,
        float maxX,
        float maxY,
        out string? statusMessage)
    {
        statusMessage = null;

        if (!isBoardFocused)
        {
            statusMessage = "Board keyboard input is inactive. Focus the board first.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedPieceId))
        {
            statusMessage = "Select a piece first for keyboard movement and rotation.";
            return false;
        }

        var selectedPiece = RequireSelectedPiece(commandService.CurrentVttSession);
        switch (key)
        {
            case "ArrowUp":
                MoveSelectedPiece(commandService, selectedPiece.Location.Coordinate.X, selectedPiece.Location.Coordinate.Y - moveStep, clampToBoard, maxX, maxY);
                statusMessage = "Moved selected piece up.";
                return true;
            case "ArrowDown":
                MoveSelectedPiece(commandService, selectedPiece.Location.Coordinate.X, selectedPiece.Location.Coordinate.Y + moveStep, clampToBoard, maxX, maxY);
                statusMessage = "Moved selected piece down.";
                return true;
            case "ArrowLeft":
                MoveSelectedPiece(commandService, selectedPiece.Location.Coordinate.X - moveStep, selectedPiece.Location.Coordinate.Y, clampToBoard, maxX, maxY);
                statusMessage = "Moved selected piece left.";
                return true;
            case "ArrowRight":
                MoveSelectedPiece(commandService, selectedPiece.Location.Coordinate.X + moveStep, selectedPiece.Location.Coordinate.Y, clampToBoard, maxX, maxY);
                statusMessage = "Moved selected piece right.";
                return true;
            case "q":
            case "Q":
                RotateSelectedPiece(commandService, selectedPiece.Rotation.Degrees - rotateStep);
                statusMessage = "Rotated selected piece left.";
                return true;
            case "e":
            case "E":
                RotateSelectedPiece(commandService, selectedPiece.Rotation.Degrees + rotateStep);
                statusMessage = "Rotated selected piece right.";
                return true;
            default:
                return false;
        }
    }

    public bool TryCenterSelectedPiece(
        ISessionCommandService commandService,
        float centerX,
        float centerY,
        bool clampToBoard,
        float maxX,
        float maxY,
        out string? statusMessage)
    {
        statusMessage = null;

        if (string.IsNullOrWhiteSpace(SelectedPieceId))
        {
            statusMessage = "Select a piece first before centering.";
            return false;
        }

        MoveSelectedPiece(commandService, centerX, centerY, clampToBoard, maxX, maxY);
        statusMessage = "Centered selected piece on board.";
        return true;
    }

    public bool TryHandleBoardClick(
        ISessionCommandService commandService,
        float clickX,
        float clickY,
        bool addPieceAtBoardClickMode,
        string? newPieceId,
        string? newPieceDefinitionId,
        string? newPieceOwnerParticipantId,
        float newPieceRotation,
        bool clampToBoard,
        float maxX,
        float maxY,
        out string? statusMessage,
        out string? createdPieceId)
    {
        var handled = TryHandleBoardClick(
            commandService,
            clickX,
            clickY,
            addPieceAtBoardClickMode,
            stampModeEnabled: false,
            newPieceId,
            newPieceDefinitionId,
            newPieceOwnerParticipantId,
            newPieceRotation,
            clampToBoard,
            maxX,
            maxY,
            out statusMessage,
            out createdPieceId,
            out _,
            out _);

        return handled;
    }

    public bool TryHandleBoardClick(
        ISessionCommandService commandService,
        float clickX,
        float clickY,
        bool addPieceAtBoardClickMode,
        bool stampModeEnabled,
        string? newPieceId,
        string? newPieceDefinitionId,
        string? newPieceOwnerParticipantId,
        float newPieceRotation,
        bool clampToBoard,
        float maxX,
        float maxY,
        out string? statusMessage,
        out string? createdPieceId,
        out bool keepAddPieceAtBoardClickMode,
        out string? nextSuggestedPieceId)
    {
        statusMessage = null;
        createdPieceId = null;
        keepAddPieceAtBoardClickMode = addPieceAtBoardClickMode;
        nextSuggestedPieceId = null;

        var targetX = clampToBoard ? Math.Clamp(clickX, 0, maxX) : clickX;
        var targetY = clampToBoard ? Math.Clamp(clickY, 0, maxY) : clickY;

        if (addPieceAtBoardClickMode)
        {
            if (string.IsNullOrWhiteSpace(ActiveSurfaceId))
            {
                statusMessage = "Cannot add piece from board click without an active surface.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(newPieceId) || string.IsNullOrWhiteSpace(newPieceDefinitionId))
            {
                statusMessage = "Piece Id and Definition Id are required for add-at-click mode.";
                return false;
            }

            if (commandService is not VttSessionWorkspaceService workspaceService)
            {
                throw new InvalidOperationException("Add-at-click requires session workspace setup boundary.");
            }

            try
            {
                workspaceService.AddPiece(
                    newPieceId,
                    newPieceDefinitionId,
                    ActiveSurfaceId,
                    targetX,
                    targetY,
                    newPieceOwnerParticipantId ?? string.Empty,
                    newPieceRotation);
            }
            catch (InvalidOperationException ex)
            {
                statusMessage = ex.Message;
                return false;
            }

            SelectPiece(newPieceId);
            createdPieceId = newPieceId;
            keepAddPieceAtBoardClickMode = stampModeEnabled;
            nextSuggestedPieceId = SuggestNextPieceId(newPieceId);
            statusMessage = "Added piece from board click.";
            return true;
        }

        if (string.IsNullOrWhiteSpace(SelectedPieceId))
        {
            statusMessage = "Select a piece first before clicking board to move.";
            return false;
        }

        MoveSelectedPiece(commandService, targetX, targetY, clampToBoard, maxX, maxY);
        statusMessage = "Moved selected piece from board click.";
        return true;
    }

    public string SuggestNextPieceId(string currentPieceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentPieceId);

        var trimmed = currentPieceId.Trim();
        var lastNonDigitIndex = trimmed.Length - 1;

        while (lastNonDigitIndex >= 0 && char.IsDigit(trimmed[lastNonDigitIndex]))
        {
            lastNonDigitIndex--;
        }

        if (lastNonDigitIndex == trimmed.Length - 1)
        {
            return $"{trimmed}2";
        }

        var prefix = trimmed[..(lastNonDigitIndex + 1)];
        var numericSuffix = trimmed[(lastNonDigitIndex + 1)..];

        if (!int.TryParse(numericSuffix, out var suffixNumber))
        {
            return $"{trimmed}2";
        }

        return $"{prefix}{suffixNumber + 1}";
    }

    public string ResetPieceIdPreview(string? basePieceId)
    {
        return basePieceId?.Trim() ?? string.Empty;
    }

    public string NudgePieceIdPreview(string currentPieceId, int delta)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentPieceId);

        var (prefix, suffix, hasNumericSuffix) = ParsePieceId(currentPieceId.Trim());
        var start = hasNumericSuffix ? suffix : 0;
        var nudged = Math.Max(0, start + delta);
        return $"{prefix}{nudged}";
    }

    public void RememberDefinitionForSurface(string surfaceId, string definitionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(surfaceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(definitionId);

        _definitionBySurface[surfaceId] = definitionId;
    }

    public string? TryGetRememberedDefinitionForSurface(string? surfaceId)
    {
        if (string.IsNullOrWhiteSpace(surfaceId))
        {
            return null;
        }

        return _definitionBySurface.TryGetValue(surfaceId, out var definitionId)
            ? definitionId
            : null;
    }

    public StampPreset AddPreset(
        string? presetName,
        string? definitionId,
        string? ownerParticipantId,
        float initialRotation)
    {
        if (string.IsNullOrWhiteSpace(definitionId))
        {
            throw new InvalidOperationException("Definition Id is required to save a stamp preset.");
        }

        var normalizedDefinitionId = definitionId.Trim();
        var normalizedName = string.IsNullOrWhiteSpace(presetName)
            ? BuildGeneratedPresetName(normalizedDefinitionId)
            : presetName.Trim();

        if (_stampPresets.Any(p => string.Equals(p.Name, normalizedName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Preset name already exists.");
        }

        var preset = new StampPreset(
            normalizedName,
            normalizedDefinitionId,
            ownerParticipantId?.Trim() ?? string.Empty,
            initialRotation);

        _stampPresets.Add(preset);
        return preset;
    }

    public IReadOnlyList<StampPreset> GetPiecePaletteEntries()
    {
        return _stampPresets;
    }

    public StampPreset SelectPiecePaletteEntry(string presetName)
    {
        return ApplyPreset(presetName);
    }

    public StampPreset ApplyPreset(string presetName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(presetName);

        var preset = _stampPresets.FirstOrDefault(p => string.Equals(p.Name, presetName, StringComparison.Ordinal));
        if (preset is null)
        {
            throw new InvalidOperationException("Selected preset was not found.");
        }

        ActiveStampPresetName = preset.Name;
        return preset;
    }

    public void ClearActivePresetSelection()
    {
        ActiveStampPresetName = null;
    }

    public StampPreset RenamePreset(string presetName, string newPresetName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(presetName);
        if (string.IsNullOrWhiteSpace(newPresetName))
        {
            throw new InvalidOperationException("Preset name cannot be empty.");
        }

        var sourcePreset = _stampPresets.FirstOrDefault(p => string.Equals(p.Name, presetName, StringComparison.Ordinal));
        if (sourcePreset is null)
        {
            throw new InvalidOperationException("Selected preset was not found.");
        }

        var normalizedName = newPresetName.Trim();
        if (!string.Equals(sourcePreset.Name, normalizedName, StringComparison.Ordinal) &&
            _stampPresets.Any(p => string.Equals(p.Name, normalizedName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Preset name already exists.");
        }

        var renamedPreset = sourcePreset with { Name = normalizedName };
        var sourceIndex = _stampPresets.FindIndex(p => string.Equals(p.Name, sourcePreset.Name, StringComparison.Ordinal));
        _stampPresets[sourceIndex] = renamedPreset;

        if (string.Equals(ActiveStampPresetName, sourcePreset.Name, StringComparison.Ordinal))
        {
            ActiveStampPresetName = renamedPreset.Name;
        }

        foreach (var mapping in _defaultPresetBySurface.Where(kvp => string.Equals(kvp.Value, sourcePreset.Name, StringComparison.Ordinal)).ToList())
        {
            _defaultPresetBySurface[mapping.Key] = renamedPreset.Name;
        }

        return renamedPreset;
    }

    public StampPreset DuplicatePreset(string presetName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(presetName);

        var sourcePreset = _stampPresets.FirstOrDefault(p => string.Equals(p.Name, presetName, StringComparison.Ordinal));
        if (sourcePreset is null)
        {
            throw new InvalidOperationException("Selected preset was not found.");
        }

        var duplicatedName = BuildDuplicatedPresetName(sourcePreset.Name);
        var duplicatedPreset = sourcePreset with { Name = duplicatedName };
        _stampPresets.Add(duplicatedPreset);
        ActiveStampPresetName = duplicatedPreset.Name;
        return duplicatedPreset;
    }

    public bool RemovePreset(string? presetName)
    {
        if (string.IsNullOrWhiteSpace(presetName))
        {
            return false;
        }

        var preset = _stampPresets.FirstOrDefault(p => string.Equals(p.Name, presetName, StringComparison.Ordinal));
        if (preset is null)
        {
            return false;
        }

        var removed = _stampPresets.Remove(preset);
        if (!removed)
        {
            return false;
        }

        if (string.Equals(ActiveStampPresetName, preset.Name, StringComparison.Ordinal))
        {
            ActiveStampPresetName = null;
        }

        foreach (var mapping in _defaultPresetBySurface.Where(kvp => string.Equals(kvp.Value, preset.Name, StringComparison.Ordinal)).ToList())
        {
            _defaultPresetBySurface.Remove(mapping.Key);
        }

        return true;
    }

    public void SetDefaultPresetForSurface(string surfaceId, string presetName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(surfaceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(presetName);

        if (_stampPresets.All(p => !string.Equals(p.Name, presetName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Selected preset was not found.");
        }

        _defaultPresetBySurface[surfaceId] = presetName;
    }

    public void ClearDefaultPresetForSurface(string surfaceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(surfaceId);
        _defaultPresetBySurface.Remove(surfaceId);
    }

    public StampPreset? GetDefaultPresetForSurface(string? surfaceId)
    {
        if (string.IsNullOrWhiteSpace(surfaceId))
        {
            return null;
        }

        if (!_defaultPresetBySurface.TryGetValue(surfaceId, out var presetName))
        {
            return null;
        }

        return _stampPresets.FirstOrDefault(p => string.Equals(p.Name, presetName, StringComparison.Ordinal));
    }

    public StampQueueItem AddQueueItem(string surfaceId, string? presetName, string nextPieceId, string? label = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(surfaceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(nextPieceId);

        if (!string.IsNullOrWhiteSpace(presetName) && _stampPresets.All(p => !string.Equals(p.Name, presetName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Selected preset was not found.");
        }

        var item = new StampQueueItem(
            SurfaceId: surfaceId.Trim(),
            PresetName: string.IsNullOrWhiteSpace(presetName) ? null : presetName.Trim(),
            NextPieceId: nextPieceId.Trim(),
            Label: label?.Trim() ?? string.Empty);

        _stampQueue.Add(item);
        ActiveStampQueueIndex = _stampQueue.Count - 1;
        return item;
    }

    public bool RemoveQueueItem(int index)
    {
        if (index < 0 || index >= _stampQueue.Count)
        {
            return false;
        }

        _stampQueue.RemoveAt(index);
        if (_stampQueue.Count == 0)
        {
            ActiveStampQueueIndex = -1;
            return true;
        }

        if (ActiveStampQueueIndex == index)
        {
            ActiveStampQueueIndex = Math.Min(index, _stampQueue.Count - 1);
        }
        else if (ActiveStampQueueIndex > index)
        {
            ActiveStampQueueIndex--;
        }

        return true;
    }

    public bool MoveQueueItemUp(int index)
    {
        if (index <= 0 || index >= _stampQueue.Count)
        {
            return false;
        }

        (_stampQueue[index - 1], _stampQueue[index]) = (_stampQueue[index], _stampQueue[index - 1]);

        if (ActiveStampQueueIndex == index)
        {
            ActiveStampQueueIndex--;
        }
        else if (ActiveStampQueueIndex == index - 1)
        {
            ActiveStampQueueIndex++;
        }

        return true;
    }

    public bool MoveQueueItemDown(int index)
    {
        if (index < 0 || index >= _stampQueue.Count - 1)
        {
            return false;
        }

        (_stampQueue[index], _stampQueue[index + 1]) = (_stampQueue[index + 1], _stampQueue[index]);

        if (ActiveStampQueueIndex == index)
        {
            ActiveStampQueueIndex++;
        }
        else if (ActiveStampQueueIndex == index + 1)
        {
            ActiveStampQueueIndex--;
        }

        return true;
    }

    public bool UpdateQueueItemNextId(int index, string nextPieceId)
    {
        if (index < 0 || index >= _stampQueue.Count || string.IsNullOrWhiteSpace(nextPieceId))
        {
            return false;
        }

        var current = _stampQueue[index];
        _stampQueue[index] = current with { NextPieceId = nextPieceId.Trim() };
        return true;
    }

    public bool TryApplyQueueItemToBoardContext(
        int index,
        VttSession? session,
        bool clearSelectionWhenSwitchingSurfaces,
        out StampQueueItem? queueItem,
        out StampPreset? appliedPreset,
        out bool clearedSelection,
        out string? statusMessage)
    {
        queueItem = null;
        appliedPreset = null;
        clearedSelection = false;
        statusMessage = null;

        if (index < 0 || index >= _stampQueue.Count)
        {
            statusMessage = "Queue item index was out of range.";
            return false;
        }

        var candidate = _stampQueue[index];
        if (!TrySwitchActiveSurface(session, candidate.SurfaceId, clearSelectionWhenSwitchingSurfaces, out clearedSelection, out statusMessage))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(candidate.PresetName))
        {
            var preset = _stampPresets.FirstOrDefault(p => string.Equals(p.Name, candidate.PresetName, StringComparison.Ordinal));
            if (preset is null)
            {
                statusMessage = "Queue item preset was not found.";
                return false;
            }

            appliedPreset = ApplyPreset(preset.Name);
        }

        ActiveStampQueueIndex = index;
        queueItem = candidate;
        return true;
    }

    public bool TryApplyNextQueueItemToBoardContext(
        VttSession? session,
        bool clearSelectionWhenSwitchingSurfaces,
        out StampQueueItem? queueItem,
        out StampPreset? appliedPreset,
        out bool clearedSelection,
        out string? statusMessage)
    {
        var nextIndex = ActiveStampQueueIndex + 1;
        if (nextIndex < 0 || nextIndex >= _stampQueue.Count)
        {
            queueItem = null;
            appliedPreset = null;
            clearedSelection = false;
            statusMessage = "No next queue item is available.";
            return false;
        }

        return TryApplyQueueItemToBoardContext(
            nextIndex,
            session,
            clearSelectionWhenSwitchingSurfaces,
            out queueItem,
            out appliedPreset,
            out clearedSelection,
            out statusMessage);
    }

    public bool TryUpdateActiveQueueItemNextId(string nextPieceId)
    {
        if (ActiveStampQueueIndex < 0)
        {
            return false;
        }

        return UpdateQueueItemNextId(ActiveStampQueueIndex, nextPieceId);
    }

    public StampQueueTemplate SaveQueueAsTemplate(string templateName)
    {
        if (string.IsNullOrWhiteSpace(templateName))
        {
            throw new InvalidOperationException("Template name is required.");
        }

        if (_stampQueue.Count == 0)
        {
            throw new InvalidOperationException("Queue must contain at least one item before saving a template.");
        }

        var normalizedName = templateName.Trim();
        if (_stampQueueTemplates.Any(t => string.Equals(t.TemplateName, normalizedName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Template name already exists.");
        }

        var template = new StampQueueTemplate(normalizedName, CloneQueueItems(_stampQueue));
        _stampQueueTemplates.Add(template);
        ActiveStampQueueTemplateName = template.TemplateName;
        return template;
    }

    public StampQueueTemplate ApplyTemplateToQueue(string templateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

        var template = _stampQueueTemplates.FirstOrDefault(t => string.Equals(t.TemplateName, templateName, StringComparison.Ordinal));
        if (template is null)
        {
            throw new InvalidOperationException("Selected template was not found.");
        }

        _stampQueue.Clear();
        _stampQueue.AddRange(CloneQueueItems(template.Items));
        ActiveStampQueueIndex = _stampQueue.Count > 0 ? 0 : -1;
        ActiveStampQueueTemplateName = template.TemplateName;
        return template;
    }

    public bool DeleteQueueTemplate(string? templateName)
    {
        if (string.IsNullOrWhiteSpace(templateName))
        {
            return false;
        }

        var template = _stampQueueTemplates.FirstOrDefault(t => string.Equals(t.TemplateName, templateName, StringComparison.Ordinal));
        if (template is null)
        {
            return false;
        }

        var removed = _stampQueueTemplates.Remove(template);
        if (!removed)
        {
            return false;
        }

        if (string.Equals(ActiveStampQueueTemplateName, template.TemplateName, StringComparison.Ordinal))
        {
            ActiveStampQueueTemplateName = null;
        }

        return true;
    }

    public StampQueueTemplate RenameQueueTemplate(string templateName, string newTemplateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);
        if (string.IsNullOrWhiteSpace(newTemplateName))
        {
            throw new InvalidOperationException("Template name cannot be empty.");
        }

        var template = _stampQueueTemplates.FirstOrDefault(t => string.Equals(t.TemplateName, templateName, StringComparison.Ordinal));
        if (template is null)
        {
            throw new InvalidOperationException("Selected template was not found.");
        }

        var normalizedName = newTemplateName.Trim();
        if (!string.Equals(template.TemplateName, normalizedName, StringComparison.Ordinal) &&
            _stampQueueTemplates.Any(t => string.Equals(t.TemplateName, normalizedName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Template name already exists.");
        }

        var renamed = template with { TemplateName = normalizedName };
        var index = _stampQueueTemplates.FindIndex(t => string.Equals(t.TemplateName, template.TemplateName, StringComparison.Ordinal));
        _stampQueueTemplates[index] = renamed;

        if (string.Equals(ActiveStampQueueTemplateName, template.TemplateName, StringComparison.Ordinal))
        {
            ActiveStampQueueTemplateName = renamed.TemplateName;
        }

        return renamed;
    }

    public StampQueueTemplate DuplicateQueueTemplate(string templateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

        var template = _stampQueueTemplates.FirstOrDefault(t => string.Equals(t.TemplateName, templateName, StringComparison.Ordinal));
        if (template is null)
        {
            throw new InvalidOperationException("Selected template was not found.");
        }

        var duplicateName = BuildDuplicatedTemplateName(template.TemplateName);
        var duplicated = new StampQueueTemplate(duplicateName, CloneQueueItems(template.Items));
        _stampQueueTemplates.Add(duplicated);
        ActiveStampQueueTemplateName = duplicated.TemplateName;
        return duplicated;
    }
    private static (string Prefix, int Suffix, bool HasNumericSuffix) ParsePieceId(string pieceId)
    {
        var lastNonDigitIndex = pieceId.Length - 1;

        while (lastNonDigitIndex >= 0 && char.IsDigit(pieceId[lastNonDigitIndex]))
        {
            lastNonDigitIndex--;
        }

        if (lastNonDigitIndex == pieceId.Length - 1)
        {
            return (pieceId, 0, false);
        }

        var prefix = pieceId[..(lastNonDigitIndex + 1)];
        var numericSuffix = pieceId[(lastNonDigitIndex + 1)..];
        if (!int.TryParse(numericSuffix, out var suffix))
        {
            return (pieceId, 0, false);
        }

        return (prefix, suffix, true);
    }

    private string BuildGeneratedPresetName(string definitionId)
    {
        if (_stampPresets.All(p => !string.Equals(p.Name, definitionId, StringComparison.Ordinal)))
        {
            return definitionId;
        }

        var index = 2;
        while (_stampPresets.Any(p => string.Equals(p.Name, $"{definitionId}-{index}", StringComparison.Ordinal)))
        {
            index++;
        }

        return $"{definitionId}-{index}";
    }

    private string BuildDuplicatedPresetName(string baseName)
    {
        var candidate = $"{baseName} Copy";
        if (_stampPresets.All(p => !string.Equals(p.Name, candidate, StringComparison.Ordinal)))
        {
            return candidate;
        }

        var index = 2;
        while (_stampPresets.Any(p => string.Equals(p.Name, $"{candidate} {index}", StringComparison.Ordinal)))
        {
            index++;
        }

        return $"{candidate} {index}";
    }

    private string BuildDuplicatedTemplateName(string baseName)
    {
        var candidate = $"{baseName} Copy";
        if (_stampQueueTemplates.All(t => !string.Equals(t.TemplateName, candidate, StringComparison.Ordinal)))
        {
            return candidate;
        }

        var index = 2;
        while (_stampQueueTemplates.Any(t => string.Equals(t.TemplateName, $"{candidate} {index}", StringComparison.Ordinal)))
        {
            index++;
        }

        return $"{candidate} {index}";
    }

    private static IReadOnlyList<StampQueueItem> CloneQueueItems(IEnumerable<StampQueueItem> queueItems)
    {
        return queueItems.Select(item => item with { }).ToList();
    }

    public PieceInstance GetSelectedPieceOrThrow(VttSession? session)
    {
        return RequireSelectedPiece(session);
    }

    public ActionRecord RotateSelectedPiece(ISessionCommandService commandService, float newDegrees)
    {
        var selectedPiece = RequireSelectedPiece(commandService.CurrentVttSession);

        return commandService.ProcessAction(new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "gm",
            Payload = new RotatePiecePayload
            {
                PieceId = selectedPiece.Id,
                NewRotation = new Rotation { Degrees = newDegrees }
            }
        });
    }

    private PieceInstance RequireSelectedPiece(VttSession? session)
    {
        if (session is null)
        {
            throw new InvalidOperationException("Current session is required.");
        }

        if (string.IsNullOrWhiteSpace(SelectedPieceId))
        {
            throw new InvalidOperationException("Select a piece first.");
        }

        var piece = session.Pieces.FirstOrDefault(p => p.Id == SelectedPieceId);
        if (piece is null)
        {
            throw new InvalidOperationException("Selected piece was not found in current session.");
        }

        return piece;
    }
}

public sealed record PieceGroup(
    string GroupId,
    string Label,
    IReadOnlyList<string> MemberPieceIds);

public sealed record StampPreset(
    string Name,
    string DefinitionId,
    string OwnerParticipantId,
    float InitialRotation);

public sealed record StampQueueItem(
    string SurfaceId,
    string? PresetName,
    string NextPieceId,
    string Label);

public sealed record StampQueueTemplate(
    string TemplateName,
    IReadOnlyList<StampQueueItem> Items);

public sealed class BoardVisibilityOptions
{
    public bool ShowSurfaces { get; set; } = true;

    public bool ShowPieces { get; set; } = true;

    public bool ShowMarkers { get; set; } = true;

    public bool ActiveSurfaceOnly { get; set; } = true;

    public string DefinitionIdFilter { get; set; } = string.Empty;

    public string OwnerParticipantIdFilter { get; set; } = string.Empty;
}
