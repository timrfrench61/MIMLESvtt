namespace MIMLESvtt.src.Domain.Models.Content;

public class EquipmentEditDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string EquipmentType { get; set; } = string.Empty;

    public string EquipmentCategory { get; set; } = string.Empty;

    public decimal BaseCost { get; set; }

    public decimal Weight { get; set; }

    public string WeightUnit { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    public Dictionary<string, object> Extensions { get; set; } = [];
}
