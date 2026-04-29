namespace MIMLESvtt.src.Domain.Models.Content;

public class UnitCounterEditDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string UnitType { get; set; } = string.Empty;

    public string Side { get; set; } = string.Empty;

    public string Faction { get; set; } = string.Empty;

    public decimal StrengthOrValue { get; set; }

    public decimal Movement { get; set; }

    public decimal DefenseOrArmor { get; set; }

    public decimal? RangeOrReach { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    public Dictionary<string, object> Extensions { get; set; } = [];
}
