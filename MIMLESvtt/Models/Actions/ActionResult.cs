namespace VttMvuModel.Actions;

public sealed record ActionResult(bool Succeeded, string? ErrorMessage, IReadOnlyList<StateChange> Changes);
