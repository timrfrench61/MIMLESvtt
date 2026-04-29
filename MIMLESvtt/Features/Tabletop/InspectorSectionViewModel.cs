namespace VttMvuView.Tabletop;
public sealed record InspectorSectionViewModel(string SectionId, string Title, IReadOnlyList<PropertyRowViewModel> Properties, bool IsExpanded);
