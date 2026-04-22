using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.src.Domain.Models.Surfaces;

namespace MIMLESvtt.src
{
    public enum WorkspaceUndoOperationKind
    {
        MovePiece,
        RotatePiece,
        AddMarker,
        RemoveMarker,
        ChangePieceState,
        AddPiece,
        AddSurface
    }

    public sealed class WorkspaceUndoEntry
    {
        public WorkspaceUndoOperationKind OperationKind { get; set; }

        public string PieceId { get; set; } = string.Empty;

        public Location? FromLocation { get; set; }

        public Location? ToLocation { get; set; }

        public Rotation? FromRotation { get; set; }

        public Rotation? ToRotation { get; set; }

        public string MarkerId { get; set; } = string.Empty;

        public string StateKey { get; set; } = string.Empty;

        public object? PreviousStateValue { get; set; }

        public bool PreviousStateKeyExisted { get; set; }

        public object? NewStateValue { get; set; }

        public PieceInstance? PieceSnapshot { get; set; }

        public SurfaceInstance? SurfaceSnapshot { get; set; }

        public bool AddMarkerChangedState { get; set; }

        public bool RemoveMarkerChangedState { get; set; }
    }
}
