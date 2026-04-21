using MIMLESvtt.src;

namespace MIMLESvtt.Components.Pages;

public class WorkspaceBoardState
{
    public string? ActiveSurfaceId { get; set; }

    public string? SelectedPieceId { get; set; }

    public void EnsureActiveSurface(TableSession? session)
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

    public void SelectPiece(string pieceId)
    {
        SelectedPieceId = pieceId;
    }

    public IReadOnlyList<PieceInstance> GetRenderedPiecesForActiveSurface(TableSession? session)
    {
        if (session is null || string.IsNullOrWhiteSpace(ActiveSurfaceId))
        {
            return [];
        }

        return session.Pieces.Where(p => p.Location.SurfaceId == ActiveSurfaceId).ToList();
    }

    public ActionRecord MoveSelectedPiece(SessionWorkspaceService workspaceService, float x, float y)
    {
        var selectedPiece = RequireSelectedPiece(workspaceService.State.CurrentTableSession);
        if (string.IsNullOrWhiteSpace(ActiveSurfaceId))
        {
            throw new InvalidOperationException("Select an active surface first.");
        }

        return workspaceService.ProcessAction(new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "gm",
            Payload = new MovePiecePayload
            {
                PieceId = selectedPiece.Id,
                NewLocation = new Location
                {
                    SurfaceId = ActiveSurfaceId,
                    Coordinate = new Coordinate { X = x, Y = y }
                }
            }
        });
    }

    public ActionRecord RotateSelectedPiece(SessionWorkspaceService workspaceService, float newDegrees)
    {
        var selectedPiece = RequireSelectedPiece(workspaceService.State.CurrentTableSession);

        return workspaceService.ProcessAction(new ActionRequest
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

    private PieceInstance RequireSelectedPiece(TableSession? session)
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
