namespace VttMvuView.Application;
public sealed record NavigationViewState(string CurrentRoute, IReadOnlyList<NavigationItemViewModel> Items, IReadOnlyList<BreadcrumbViewModel> Breadcrumbs);
