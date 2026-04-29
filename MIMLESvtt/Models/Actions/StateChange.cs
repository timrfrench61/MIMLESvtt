namespace VttMvuModel.Actions;

public sealed record StateChange(string ChangeType, string TargetId, string Description);
