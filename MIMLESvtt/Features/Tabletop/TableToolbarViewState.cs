namespace VttMvuView.Tabletop;
public sealed record TableToolbarViewState(IReadOnlyList<ToolbarButtonViewModel> Buttons, string? ActiveToolId);
