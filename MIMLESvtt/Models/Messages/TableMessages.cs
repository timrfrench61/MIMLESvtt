namespace VttMvuModel.Messages;

using VttMvuModel.Actions;

public sealed record DispatchPlayerActionMessage(PlayerAction Action) : AppMessage;
public sealed record SelectPieceMessage(string PieceId) : AppMessage;
public sealed record ClearSelectionMessage : AppMessage;
