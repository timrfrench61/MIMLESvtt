namespace VttMvuView.Tabletop;
public sealed record TabletopViewState(BoardViewportViewState BoardViewport, TableToolbarViewState Toolbar, SelectionViewState Selection, InspectorViewState Inspector, ActionTrayViewState ActionTray, TableStatusViewState Status);
