namespace VttMvuView.Tabletop;

public interface ITabletopAction;

public sealed record SelectTokenAction(string TokenId) : ITabletopAction;

public sealed record MoveSelectedTokenAction(int DeltaX, int DeltaY) : ITabletopAction;

public sealed record RotateSelectedTokenAction(int DeltaDegrees) : ITabletopAction;

public sealed record EndTurnAction : ITabletopAction;

public sealed record SetActiveToolAction(string ToolId) : ITabletopAction;

public sealed record RollDiceAction : ITabletopAction;
