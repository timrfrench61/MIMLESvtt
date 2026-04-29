namespace VttMvuView.Tabletop;
public sealed record ActionTrayViewState(IReadOnlyList<ActionButtonViewModel> AvailableActions, string? PrimaryActionId);
