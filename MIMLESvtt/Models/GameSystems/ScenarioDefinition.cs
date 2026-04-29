namespace VttMvuModel.GameSystems;

using VttMvuModel.Table;

public sealed record ScenarioDefinition(string ScenarioId, string GameSystemId, string Name, string Description, TableState InitialTableState);
