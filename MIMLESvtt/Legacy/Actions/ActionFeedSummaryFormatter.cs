using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Surfaces;

namespace MIMLESvtt.src
{
    public static class ActionFeedSummaryFormatter
    {
        public static string SummarizeActionPayload(ActionRecord action)
        {
            ArgumentNullException.ThrowIfNull(action);

            return action.ActionType switch
            {
                "MovePiece" when action.Payload is MovePiecePayload payload =>
                    $"{payload.PieceId} -> {payload.NewLocation?.SurfaceId} ({payload.NewLocation?.Coordinate.X}, {payload.NewLocation?.Coordinate.Y})",
                "RotatePiece" when action.Payload is RotatePiecePayload payload =>
                    $"{payload.PieceId} -> {payload.NewRotation?.Degrees}°",
                "AddMarker" when action.Payload is AddMarkerPayload payload =>
                    $"{payload.PieceId} + {payload.MarkerId}",
                "RemoveMarker" when action.Payload is RemoveMarkerPayload payload =>
                    $"{payload.PieceId} - {payload.MarkerId}",
                "ChangePieceState" when action.Payload is ChangePieceStatePayload payload =>
                    payload.Value is null ? $"{payload.PieceId} remove {payload.Key}" : $"{payload.PieceId} {payload.Key}={payload.Value}",
                "AddPiece" when action.Payload is PieceInstance payload =>
                    $"{payload.Id} on {payload.Location.SurfaceId}",
                "AddSurface" when action.Payload is SurfaceInstance payload =>
                    $"{payload.Id} ({payload.Type}/{payload.CoordinateSystem})",
                _ => action.Payload?.ToString() ?? "(no payload)"
            };
        }
    }
}
