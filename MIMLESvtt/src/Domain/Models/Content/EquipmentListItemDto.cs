namespace MIMLESvtt.src.Domain.Models.Content;

public class EquipmentListItemDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string EquipmentType { get; set; } = string.Empty;

    public decimal BaseCost { get; set; }

    public decimal Weight { get; set; }

    public string WeightUnit { get; set; } = string.Empty;
}
