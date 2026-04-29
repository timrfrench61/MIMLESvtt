namespace VttMvuModel.Board;

public sealed record BoardZoneDefinition(string ZoneId, string Name, IReadOnlyList<string> CellIds);
