using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Pieces;

namespace MIMLESvtt.src
{
    public class ActionProcessor
    {
        private const string MovePieceActionType = "MovePiece";
        private const string RotatePieceActionType = "RotatePiece";
        private const string AddMarkerActionType = "AddMarker";
        private const string RemoveMarkerActionType = "RemoveMarker";
        private const string ChangePieceStateActionType = "ChangePieceState";

        public ActionRecord Process(VttSession tableSession, ActionRequest actionRequest)
        {
            Validate(tableSession, actionRequest);
            Apply(tableSession, actionRequest);

            var actionRecord = CreateActionRecord(actionRequest);
            Log(tableSession, actionRecord);

            return actionRecord;
        }

        private static void Validate(VttSession tableSession, ActionRequest actionRequest)
        {
            ArgumentNullException.ThrowIfNull(tableSession);
            ArgumentNullException.ThrowIfNull(actionRequest);
            ArgumentException.ThrowIfNullOrWhiteSpace(actionRequest.ActionType);
            ArgumentException.ThrowIfNullOrWhiteSpace(actionRequest.ActorParticipantId);
        }

        private static void Apply(VttSession tableSession, ActionRequest actionRequest)
        {
            switch (actionRequest.ActionType)
            {
                case MovePieceActionType:
                    ApplyMovePiece(tableSession, actionRequest);
                    return;
                case RotatePieceActionType:
                    ApplyRotatePiece(tableSession, actionRequest);
                    return;
                case AddMarkerActionType:
                    ApplyAddMarker(tableSession, actionRequest);
                    return;
                case RemoveMarkerActionType:
                    ApplyRemoveMarker(tableSession, actionRequest);
                    return;
                case ChangePieceStateActionType:
                    ApplyChangePieceState(tableSession, actionRequest);
                    return;
                default:
                    return;
            }
        }

        private static void ApplyMovePiece(VttSession tableSession, ActionRequest actionRequest)
        {
            var movePiecePayload = RequirePayloadType<MovePiecePayload>(actionRequest, MovePieceActionType);

            var pieceId = RequireNonEmptyValue(movePiecePayload.PieceId, nameof(movePiecePayload.PieceId));
            ArgumentNullException.ThrowIfNull(movePiecePayload.NewLocation);
            var surfaceId = RequireNonEmptyValue(movePiecePayload.NewLocation.SurfaceId, nameof(movePiecePayload.NewLocation.SurfaceId));

            var piece = RequirePieceById(tableSession, pieceId, MovePieceActionType);

            RequireSurfaceExists(tableSession, surfaceId, MovePieceActionType);

            piece.Location = movePiecePayload.NewLocation;
        }

        private static void ApplyRotatePiece(VttSession tableSession, ActionRequest actionRequest)
        {
            var rotatePiecePayload = RequirePayloadType<RotatePiecePayload>(actionRequest, RotatePieceActionType);

            var pieceId = RequireNonEmptyValue(rotatePiecePayload.PieceId, nameof(rotatePiecePayload.PieceId));
            ArgumentNullException.ThrowIfNull(rotatePiecePayload.NewRotation);

            var piece = RequirePieceById(tableSession, pieceId, RotatePieceActionType);

            piece.Rotation = rotatePiecePayload.NewRotation;
        }

        private static void ApplyAddMarker(VttSession tableSession, ActionRequest actionRequest)
        {
            var addMarkerPayload = RequirePayloadType<AddMarkerPayload>(actionRequest, AddMarkerActionType);

            var pieceId = RequireNonEmptyValue(addMarkerPayload.PieceId, nameof(addMarkerPayload.PieceId));
            var markerId = RequireNonEmptyValue(addMarkerPayload.MarkerId, nameof(addMarkerPayload.MarkerId));

            var piece = RequirePieceById(tableSession, pieceId, AddMarkerActionType);

            if (piece.MarkerIds.Contains(markerId))
            {
                return;
            }

            piece.MarkerIds.Add(markerId);
        }

        private static void ApplyChangePieceState(VttSession tableSession, ActionRequest actionRequest)
        {
            var changePieceStatePayload = RequirePayloadType<ChangePieceStatePayload>(actionRequest, ChangePieceStateActionType);

            var pieceId = RequireNonEmptyValue(changePieceStatePayload.PieceId, nameof(changePieceStatePayload.PieceId));
            var key = RequireNonEmptyValue(changePieceStatePayload.Key, nameof(changePieceStatePayload.Key));

            var piece = RequirePieceById(tableSession, pieceId, ChangePieceStateActionType);

            if (changePieceStatePayload.Value is null)
            {
                piece.State.Remove(key);
                return;
            }

            piece.State[key] = changePieceStatePayload.Value;
        }

        private static void ApplyRemoveMarker(VttSession tableSession, ActionRequest actionRequest)
        {
            var removeMarkerPayload = RequirePayloadType<RemoveMarkerPayload>(actionRequest, RemoveMarkerActionType);

            var pieceId = RequireNonEmptyValue(removeMarkerPayload.PieceId, nameof(removeMarkerPayload.PieceId));
            var markerId = RequireNonEmptyValue(removeMarkerPayload.MarkerId, nameof(removeMarkerPayload.MarkerId));

            var piece = RequirePieceById(tableSession, pieceId, RemoveMarkerActionType);

            if (!piece.MarkerIds.Contains(markerId))
            {
                return;
            }

            piece.MarkerIds.Remove(markerId);
        }

        private static TPayload RequirePayloadType<TPayload>(ActionRequest actionRequest, string actionType)
        {
            if (actionRequest.Payload is not TPayload payload)
            {
                throw new ArgumentException($"{actionType} payload is required.", nameof(actionRequest));
            }

            return payload;
        }

        private static string RequireNonEmptyValue(string? value, string parameterName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
            return value;
        }

        private static PieceInstance RequirePieceById(VttSession tableSession, string pieceId, string actionType)
        {
            var piece = tableSession.Pieces.FirstOrDefault(p => p.Id == pieceId);
            if (piece is null)
            {
                throw new InvalidOperationException($"{actionType} target piece was not found in TableSession.Pieces.");
            }

            return piece;
        }

        private static void RequireSurfaceExists(VttSession tableSession, string surfaceId, string actionType)
        {
            var surfaceExists = tableSession.Surfaces.Any(s => s.Id == surfaceId);
            if (!surfaceExists)
            {
                throw new InvalidOperationException($"{actionType} target surface was not found in TableSession.Surfaces.");
            }
        }

        private static ActionRecord CreateActionRecord(ActionRequest actionRequest)
        {
            return new ActionRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                ActionType = actionRequest.ActionType,
                ActorParticipantId = actionRequest.ActorParticipantId,
                TimestampUtc = DateTime.UtcNow,
                Payload = actionRequest.Payload
            };
        }

        private static void Log(VttSession tableSession, ActionRecord actionRecord)
        {
            tableSession.ActionLog.Add(actionRecord);
        }
    }
}
