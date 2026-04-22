using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class ActionValidationService
    {
        private const string MovePieceActionType = "MovePiece";
        private const string RotatePieceActionType = "RotatePiece";
        private const string AddMarkerActionType = "AddMarker";
        private const string RemoveMarkerActionType = "RemoveMarker";
        private const string ChangePieceStateActionType = "ChangePieceState";

        public ActionValidationResult Validate(ActionRequest actionRequest, VttSession vttSesstion)
        {
            ArgumentNullException.ThrowIfNull(actionRequest);
            ArgumentNullException.ThrowIfNull(vttSesstion);

            if (string.IsNullOrWhiteSpace(actionRequest.ActionType))
            {
                return ActionValidationResult.Failure("ActionType is required.");
            }

            switch (actionRequest.ActionType)
            {
                case MovePieceActionType:
                    return ValidateMovePiece(actionRequest, vttSesstion);
                case RotatePieceActionType:
                    return ValidatePieceTarget(actionRequest, vttSesstion, RotatePieceActionType);
                case AddMarkerActionType:
                    return ValidatePieceTarget(actionRequest, vttSesstion, AddMarkerActionType);
                case RemoveMarkerActionType:
                    return ValidatePieceTarget(actionRequest, vttSesstion, RemoveMarkerActionType);
                case ChangePieceStateActionType:
                    return ValidatePieceTarget(actionRequest, vttSesstion, ChangePieceStateActionType);
                default:
                    return ActionValidationResult.Success();
            }
        }

        private static ActionValidationResult ValidateMovePiece(ActionRequest actionRequest, VttSession vttSesstion)
        {
            if (actionRequest.Payload is not MovePiecePayload payload)
            {
                return ActionValidationResult.Failure("MovePiece payload is required.");
            }

            if (string.IsNullOrWhiteSpace(payload.PieceId))
            {
                return ActionValidationResult.Failure("MovePiece PieceId is required.");
            }

            var piece = vttSesstion.Pieces.FirstOrDefault(p => string.Equals(p.Id, payload.PieceId, StringComparison.Ordinal));
            if (piece is null)
            {
                return ActionValidationResult.Failure("MovePiece target piece was not found in vttSesstion.Pieces.");
            }

            if (payload.NewLocation is null || string.IsNullOrWhiteSpace(payload.NewLocation.SurfaceId))
            {
                return ActionValidationResult.Failure("MovePiece target surface is required.");
            }

            var surfaceExists = vttSesstion.Surfaces.Any(s => string.Equals(s.Id, payload.NewLocation.SurfaceId, StringComparison.Ordinal));
            if (!surfaceExists)
            {
                return ActionValidationResult.Failure("MovePiece target surface was not found in vttSesstion.Surfaces.");
            }

            return ActionValidationResult.Success();
        }

        private static ActionValidationResult ValidatePieceTarget(ActionRequest actionRequest, VttSession vttSesstion, string actionType)
        {
            var pieceId = actionRequest.Payload switch
            {
                RotatePiecePayload payload => payload.PieceId,
                AddMarkerPayload payload => payload.PieceId,
                RemoveMarkerPayload payload => payload.PieceId,
                ChangePieceStatePayload payload => payload.PieceId,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(pieceId))
            {
                return ActionValidationResult.Failure($"{actionType} PieceId is required.");
            }

            var pieceExists = vttSesstion.Pieces.Any(p => string.Equals(p.Id, pieceId, StringComparison.Ordinal));
            if (!pieceExists)
            {
                return ActionValidationResult.Failure($"{actionType} target piece was not found in vttSesstion.Pieces.");
            }

            return ActionValidationResult.Success();
        }
    }
}
