namespace MIMLESvtt.src.Domain.Models.Content;

public abstract class ContentEntry
{
    public ContentMetadata Metadata { get; set; } = new();

    public abstract ContentCategory Category { get; }
}

public class MonsterContentEntry : ContentEntry
{
    public override ContentCategory Category => ContentCategory.Monster;

    public string MonsterCategory { get; set; } = string.Empty;

    public int LevelOrThreat { get; set; }

    public int HitPoints { get; set; }

    public decimal Movement { get; set; }

    public int ArmorOrDefense { get; set; }

    public string AttackProfile { get; set; } = string.Empty;
}

public class TreasureContentEntry : ContentEntry
{
    public override ContentCategory Category => ContentCategory.Treasure;

    public string TreasureType { get; set; } = string.Empty;

    public decimal BaseValue { get; set; }

    public string CurrencyOrValueUnit { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string Description { get; set; } = string.Empty;

    public List<TreasureComponentEntry> Components { get; set; } = [];

    public string EncounterReferenceId { get; set; } = string.Empty;

    public string ScenarioReferenceId { get; set; } = string.Empty;

    public string PlacementContainerReferenceId { get; set; } = string.Empty;
}

public class EquipmentContentEntry : ContentEntry
{
    public override ContentCategory Category => ContentCategory.Equipment;

    public string EquipmentType { get; set; } = string.Empty;

    public string EquipmentCategory { get; set; } = string.Empty;

    public decimal BaseCost { get; set; }

    public decimal Weight { get; set; }

    public string WeightUnit { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class MagicItemContentEntry : ContentEntry
{
    public override ContentCategory Category => ContentCategory.MagicItem;

    public string ItemType { get; set; } = string.Empty;

    public string Rarity { get; set; } = string.Empty;

    public bool AttunementRequired { get; set; }

    public int? ChargesOrUses { get; set; }

    public decimal Value { get; set; }

    public string Description { get; set; } = string.Empty;

    public MagicItemEffectMetadata EffectMetadata { get; set; } = new();
}

public class UnitCounterContentEntry : ContentEntry
{
    public override ContentCategory Category => ContentCategory.UnitCounter;

    public string UnitType { get; set; } = string.Empty;

    public string Side { get; set; } = string.Empty;

    public string Faction { get; set; } = string.Empty;

    public decimal StrengthOrValue { get; set; }

    public decimal Movement { get; set; }

    public decimal DefenseOrArmor { get; set; }

    public decimal? RangeOrReach { get; set; }

    public string Description { get; set; } = string.Empty;
}
