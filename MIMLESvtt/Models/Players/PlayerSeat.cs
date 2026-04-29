namespace VttMvuModel.Players;

public sealed record PlayerSeat(string SeatId, string DisplayName, PlayerRole Role, string? UserId, bool IsOccupied);
