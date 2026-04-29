namespace VttMvuModel.Updates;

using VttMvuModel.Application;

public sealed record AppUpdateResult(AppModel Model, IReadOnlyList<AppEffect> Effects);
