namespace VttMvuView.Tabletop;
public sealed record InspectorViewState(string Title, IReadOnlyList<InspectorSectionViewModel> Sections, bool IsPinned);
