namespace MIMLESvtt.src.Domain.Models.Content;

public class UnitCounterListItemDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string UnitType { get; set; } = string.Empty;

    public string Side { get; set; } = string.Empty;

    public string Faction { get; set; } = string.Empty;

    public decimal StrengthOrValue { get; set; }

    public decimal Movement { get; set; }
}
