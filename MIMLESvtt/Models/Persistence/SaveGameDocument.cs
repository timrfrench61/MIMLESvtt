namespace VttMvuModel.Persistence;

using VttMvuModel.Table;

public sealed record SaveGameDocument(SaveGameHeader Header, TableSession Session);
