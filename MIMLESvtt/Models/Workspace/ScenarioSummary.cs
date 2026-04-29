namespace VttMvuModel.Workspace;

public sealed record ScenarioSummary(string ScenarioId, string GameSystemId, string Name, string Description, DateTimeOffset? LastPlayedAt);
