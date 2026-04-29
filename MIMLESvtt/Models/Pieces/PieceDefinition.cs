namespace VttMvuModel.Pieces;

public sealed record PieceDefinition(string PieceDefinitionId, string Name, PieceKind Kind, string? DefaultImageAssetId, IReadOnlyDictionary<string, string> DefaultProperties);
