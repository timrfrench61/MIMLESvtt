namespace MIMLESvtt.src.Domain.Models.Content;

public static class ContentValidationUtilities
{
    public static ContentValidationResult ValidateMetadata(ContentMetadata metadata)
    {
        var result = new ContentValidationResult();

        if (metadata is null)
        {
            result.Errors.Add("Metadata is required.");
            return result;
        }

        if (string.IsNullOrWhiteSpace(metadata.Id))
        {
            result.Errors.Add("Metadata Id is required.");
        }

        if (string.IsNullOrWhiteSpace(metadata.Name))
        {
            result.Errors.Add("Metadata Name is required.");
        }

        if (string.IsNullOrWhiteSpace(metadata.Source))
        {
            result.Errors.Add("Metadata Source is required.");
        }

        return result;
    }

    public static ContentValidationResult ValidateEntry(ContentEntry entry)
    {
        var result = new ContentValidationResult();
        if (entry is null)
        {
            result.Errors.Add("Content entry is required.");
            return result;
        }

        var metadataValidation = ValidateMetadata(entry.Metadata);
        result.Errors.AddRange(metadataValidation.Errors);

        if (entry is MonsterContentEntry monster)
        {
            if (string.IsNullOrWhiteSpace(monster.MonsterCategory))
            {
                result.Errors.Add("Monster Category is required.");
            }

            if (monster.LevelOrThreat < 0)
            {
                result.Errors.Add("Monster LevelOrThreat must be 0 or greater.");
            }

            if (monster.HitPoints < 0)
            {
                result.Errors.Add("Monster HitPoints must be 0 or greater.");
            }

            if (monster.Movement < 0)
            {
                result.Errors.Add("Monster Movement must be 0 or greater.");
            }

            if (monster.ArmorOrDefense < 0)
            {
                result.Errors.Add("Monster ArmorOrDefense must be 0 or greater.");
            }

            if (string.IsNullOrWhiteSpace(monster.AttackProfile))
            {
                result.Errors.Add("Monster AttackProfile is required.");
            }
        }

        if (entry is TreasureContentEntry treasure)
        {
            if (string.IsNullOrWhiteSpace(treasure.TreasureType))
            {
                result.Errors.Add("Treasure TreasureType is required.");
            }

            if (treasure.BaseValue < 0)
            {
                result.Errors.Add("Treasure BaseValue must be 0 or greater.");
            }

            if (treasure.Quantity < 0)
            {
                result.Errors.Add("Treasure Quantity must be 0 or greater.");
            }

            for (var i = 0; i < treasure.Components.Count; i++)
            {
                var component = treasure.Components[i];
                if (string.IsNullOrWhiteSpace(component.ComponentId))
                {
                    result.Errors.Add($"Treasure component[{i}] ComponentId is required.");
                }

                if (string.IsNullOrWhiteSpace(component.ComponentType))
                {
                    result.Errors.Add($"Treasure component[{i}] ComponentType is required.");
                }

                if (component.Quantity < 0)
                {
                    result.Errors.Add($"Treasure component[{i}] Quantity must be 0 or greater.");
                }

                if (component.ValueContribution < 0)
                {
                    result.Errors.Add($"Treasure component[{i}] ValueContribution must be 0 or greater.");
                }
            }
        }

        if (entry is EquipmentContentEntry equipment)
        {
            if (string.IsNullOrWhiteSpace(equipment.EquipmentType))
            {
                result.Errors.Add("Equipment EquipmentType is required.");
            }

            if (string.IsNullOrWhiteSpace(equipment.EquipmentCategory))
            {
                result.Errors.Add("Equipment Category is required.");
            }

            if (equipment.BaseCost < 0)
            {
                result.Errors.Add("Equipment BaseCost must be 0 or greater.");
            }

            if (equipment.Weight < 0)
            {
                result.Errors.Add("Equipment Weight must be 0 or greater.");
            }

            if (string.IsNullOrWhiteSpace(equipment.WeightUnit))
            {
                result.Errors.Add("Equipment WeightUnit is required.");
            }
        }

        if (entry is MagicItemContentEntry magicItem)
        {
            if (string.IsNullOrWhiteSpace(magicItem.ItemType))
            {
                result.Errors.Add("MagicItem ItemType is required.");
            }

            if (magicItem.Value < 0)
            {
                result.Errors.Add("MagicItem Value must be 0 or greater.");
            }

            if (magicItem.ChargesOrUses is < 0)
            {
                result.Errors.Add("MagicItem ChargesOrUses must be 0 or greater when provided.");
            }

            if (magicItem.EffectMetadata is null)
            {
                result.Errors.Add("MagicItem EffectMetadata is required.");
            }
        }

        if (entry is UnitCounterContentEntry unitCounter)
        {
            if (string.IsNullOrWhiteSpace(unitCounter.UnitType))
            {
                result.Errors.Add("UnitCounter UnitType is required.");
            }

            if (string.IsNullOrWhiteSpace(unitCounter.Side))
            {
                result.Errors.Add("UnitCounter Side is required.");
            }

            if (unitCounter.StrengthOrValue < 0)
            {
                result.Errors.Add("UnitCounter StrengthOrValue must be 0 or greater.");
            }

            if (unitCounter.Movement < 0)
            {
                result.Errors.Add("UnitCounter Movement must be 0 or greater.");
            }

            if (unitCounter.DefenseOrArmor < 0)
            {
                result.Errors.Add("UnitCounter DefenseOrArmor must be 0 or greater.");
            }

            if (unitCounter.RangeOrReach is < 0)
            {
                result.Errors.Add("UnitCounter RangeOrReach must be 0 or greater when provided.");
            }

            if (entry.Metadata.Extensions.TryGetValue("ScenarioMode", out var mode)
                && string.Equals(mode?.ToString(), "Scenario", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(unitCounter.Faction))
            {
                result.Errors.Add("UnitCounter Faction is required when ScenarioMode is Scenario.");
            }
        }

        return result;
    }
}
