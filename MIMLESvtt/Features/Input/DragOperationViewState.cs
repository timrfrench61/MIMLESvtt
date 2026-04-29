namespace VttMvuView.Input;
public sealed record DragOperationViewState(string DraggedItemId, double StartX, double StartY, double CurrentX, double CurrentY, bool IsValidDropTarget);
