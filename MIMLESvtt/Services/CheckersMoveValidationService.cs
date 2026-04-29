using MIMLESvtt.src.Domain.Models.Pieces;

namespace MIMLESvtt.Services;

public class CheckersMoveValidationService
{
    public bool IsLegalBasicMove(PieceInstance piece, float targetX, float targetY, out string error)
    {
        error = string.Empty;

        var deltaX = targetX - piece.Location.Coordinate.X;
        var deltaY = targetY - piece.Location.Coordinate.Y;

        if (Math.Abs(deltaX) != 1 || Math.Abs(deltaY) != 1)
        {
            error = "Checkers move must be a one-step diagonal move for basic movement.";
            return false;
        }

        var isKing = piece.State.TryGetValue("Kinged", out var kingedValue)
            && bool.TryParse(kingedValue?.ToString(), out var parsedKinged)
            && parsedKinged;

        if (isKing)
        {
            return true;
        }

        var side = piece.State.TryGetValue("Side", out var sideValue)
            ? sideValue?.ToString() ?? string.Empty
            : string.Empty;

        if (string.Equals(side, "Red", StringComparison.OrdinalIgnoreCase))
        {
            if (deltaY <= 0)
            {
                error = "Red checkers men move forward only (positive Y).";
                return false;
            }

            return true;
        }

        if (string.Equals(side, "Black", StringComparison.OrdinalIgnoreCase))
        {
            if (deltaY >= 0)
            {
                error = "Black checkers men move forward only (negative Y).";
                return false;
            }

            return true;
        }

        error = "Checkers piece is missing side metadata (Red/Black).";
        return false;
    }
}
