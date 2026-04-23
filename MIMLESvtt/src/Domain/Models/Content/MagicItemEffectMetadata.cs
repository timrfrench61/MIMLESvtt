namespace MIMLESvtt.src.Domain.Models.Content;

public class MagicItemEffectMetadata
{
    public List<string> PassiveBonuses { get; set; } = [];

    public List<string> ActivatedEffects { get; set; } = [];

    public List<string> TriggerConditions { get; set; } = [];

    public string DurationOrExpiryHint { get; set; } = string.Empty;

    public List<string> SaveOrCheckReferences { get; set; } = [];
}
