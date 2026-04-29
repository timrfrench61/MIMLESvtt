namespace VttMvuView.Tabletop;
public sealed record TableStatusViewState(string GameSystemName, string ScenarioName, string TurnText, string ConnectionText, IReadOnlyList<StatusBadgeViewModel> Badges);
