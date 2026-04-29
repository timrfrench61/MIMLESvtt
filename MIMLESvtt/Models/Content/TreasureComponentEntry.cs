namespace MIMLESvtt.src.Domain.Models.Content;

public class TreasureComponentEntry
{
    public string ComponentId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ComponentType { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal ValueContribution { get; set; }
}
