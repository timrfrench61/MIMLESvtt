namespace VttMvuModel.Persistence;

public sealed record SaveGameHeader(string SaveId, string SessionId, string GameSystemId, string ScenarioName, DateTimeOffset SavedAt, string DisplayName);
