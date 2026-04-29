namespace VttMvuView.Tabletop;
public sealed record SelectionViewState(IReadOnlyList<SelectedItemViewModel> SelectedItems, SelectionMode Mode);
