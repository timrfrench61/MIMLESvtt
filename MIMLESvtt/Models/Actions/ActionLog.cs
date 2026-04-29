namespace VttMvuModel.Actions;

public sealed record ActionLog(IReadOnlyList<PlayerAction> Actions, IReadOnlyList<ActionResult> Results)
{
    public static ActionLog Empty => new(Array.Empty<PlayerAction>(), Array.Empty<ActionResult>());
}
