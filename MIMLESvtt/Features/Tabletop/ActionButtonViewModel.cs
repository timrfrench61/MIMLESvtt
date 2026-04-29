namespace VttMvuView.Tabletop;
public sealed record ActionButtonViewModel(string ActionId, string Label, string? IconName, bool IsEnabled, string? DisabledReason);
