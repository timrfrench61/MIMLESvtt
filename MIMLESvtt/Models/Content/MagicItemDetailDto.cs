namespace MIMLESvtt.src.Domain.Models.Content;

public class MagicItemDetailDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ItemType { get; set; } = string.Empty;

    public string Rarity { get; set; } = string.Empty;

    public bool AttunementRequired { get; set; }

    public int? ChargesOrUses { get; set; }

    public decimal Value { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    public MagicItemEffectMetadata EffectMetadata { get; set; } = new();

    public Dictionary<string, object> Extensions { get; set; } = [];
}
