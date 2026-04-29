namespace VttMvuModel.Table;

using VttMvuModel.GameSystems;
using VttMvuModel.Players;
using VttMvuModel.Turns;
using VttMvuModel.Actions;

public sealed record TableSession(string SessionId, GameSystemDefinition GameSystem, string ScenarioId, string ScenarioName, IReadOnlyList<PlayerSeat> Seats, TableState State, TurnState Turn, ActionLog ActionLog, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
