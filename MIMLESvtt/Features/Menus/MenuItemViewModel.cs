namespace VttMvuView.Menus;
public sealed record MenuItemViewModel(string ItemId, string Label, string? IconName, string? CommandId, bool IsEnabled, IReadOnlyList<MenuItemViewModel> Children);
