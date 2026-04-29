namespace VttMvuModel.GameSystems;

using VttMvuModel.Board;
using VttMvuModel.Pieces;
using VttMvuModel.Rules;

public sealed record GameSystemDefinition(string GameSystemId, string Name, string Version, BoardDefinition DefaultBoard, IReadOnlyList<PieceDefinition> PieceDefinitions, RuleSetDefinition Rules);
