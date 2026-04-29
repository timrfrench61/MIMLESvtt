namespace VttMvuModel.Updates;

public sealed record AppEffect(string EffectType, string Description, IReadOnlyDictionary<string, string> Parameters);
