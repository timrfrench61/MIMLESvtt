namespace VttMvuModel.Board;

public sealed record BoardDefinition(string BoardId, string Name, BoardGeometry Geometry, int Width, int Height, IReadOnlyList<BoardZoneDefinition> Zones);
