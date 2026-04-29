namespace VttMvuModel.Rules;

public sealed record ActionDefinition(string ActionType, string DisplayName, string Description, bool RequiresActiveTurn);
