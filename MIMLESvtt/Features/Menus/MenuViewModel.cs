namespace VttMvuView.Menus;
public sealed record MenuViewModel(string MenuId, string Label, IReadOnlyList<MenuItemViewModel> Items);
