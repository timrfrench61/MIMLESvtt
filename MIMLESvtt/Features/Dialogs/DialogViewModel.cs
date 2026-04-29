namespace VttMvuView.Dialogs;
public sealed record DialogViewModel(string DialogId, DialogKind Kind, string Title, string Message, IReadOnlyList<DialogButtonViewModel> Buttons);
