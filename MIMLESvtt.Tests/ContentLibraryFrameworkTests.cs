using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models.Content;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentLibraryFrameworkTests
{
    [TestMethod]
    public void MetadataValidation_WithRequiredFields_MatchesFrameworkContract()
    {
        var metadata = new ContentMetadata
        {
            Id = "MON001",
            Name = "Goblin",
            CategoryType = "Humanoid",
            Source = "Manual",
            Tags = ["starter", "low-level"],
            VersionRevision = "v1",
            Extensions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["Ruleset"] = "AD&D1"
            }
        };

        var result = ContentValidationUtilities.ValidateMetadata(metadata);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void ContentService_CreateAndDetail_WorksAcrossTypedCategoryModel()
    {
        IContentRepository repository = new InMemoryContentRepository();
        IContentService service = new ContentService(repository);

        var monster = new MonsterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "MON001",
                Name = "Goblin",
                CategoryType = "Humanoid",
                Source = "Manual",
                Tags = ["starter"],
                Extensions = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    ["AD&D1.THAC0"] = 19,
                    ["BRP.STR"] = 9
                }
            },
            MonsterCategory = "Humanoid",
            LevelOrThreat = 1,
            HitPoints = 5,
            Movement = 9,
            ArmorOrDefense = 6,
            AttackProfile = "Scimitar +0"
        };

        var createResult = service.Create(monster);
        var detail = service.Detail(ContentCategory.Monster, "MON001");

        Assert.IsTrue(createResult.IsValid);
        Assert.IsNotNull(detail);
        Assert.AreEqual("Goblin", detail!.Metadata.Name);
        Assert.AreEqual(2, detail.Metadata.Extensions.Count);
    }

    [TestMethod]
    public void MonsterMapper_FromCsvRow_MapsCoreFieldsAndExtensionsPassThrough()
    {
        var row = new MonsterCsvImportRow
        {
            DefinitionId = "GOB001",
            Name = "Goblin",
            Category = "Monster",
            LevelOrThreat = "1",
            HitPoints = "5",
            Movement = "9",
            ArmorClass = "6",
            AttackProfile = "Scimitar +0",
            Source = "Import",
            Tags = "starter,low-level"
        };

        var mapped = MonsterContentMapper.FromCsvRow(row);

        Assert.IsTrue(mapped.validation.IsValid);
        Assert.IsNotNull(mapped.entry);
        Assert.AreEqual("GOB001", mapped.entry!.Metadata.Id);
        Assert.AreEqual("Goblin", mapped.entry.Metadata.Name);
        Assert.AreEqual(1, mapped.entry.LevelOrThreat);
        Assert.AreEqual(5, mapped.entry.HitPoints);
        Assert.AreEqual(9m, mapped.entry.Movement);
        Assert.AreEqual(6, mapped.entry.ArmorOrDefense);
        Assert.AreEqual("Scimitar +0", mapped.entry.AttackProfile);
    }

    [TestMethod]
    public void MonsterValidation_MissingOrNegativeFields_Fails()
    {
        var monster = new MonsterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "MON002",
                Name = "Bad Monster",
                CategoryType = "Monster",
                Source = "Manual"
            },
            MonsterCategory = string.Empty,
            LevelOrThreat = -1,
            HitPoints = -5,
            Movement = -1,
            ArmorOrDefense = -2,
            AttackProfile = string.Empty
        };

        var validation = ContentValidationUtilities.ValidateEntry(monster);

        Assert.IsFalse(validation.IsValid);
        Assert.IsTrue(validation.Errors.Count >= 5);
    }

    [TestMethod]
    public void ContentService_Import_CategoryMismatch_FailsValidation()
    {
        IContentRepository repository = new InMemoryContentRepository();
        IContentService service = new ContentService(repository);

        var equipment = new EquipmentContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "EQ001",
                Name = "Shield",
                CategoryType = "Armor",
                Source = "Import"
            },
            EquipmentType = "Armor",
            EquipmentCategory = "Armor",
            BaseCost = 10,
            Weight = 6,
            WeightUnit = "lb"
        };

        var importResult = service.Import(ContentCategory.MagicItem, [equipment]);

        Assert.IsFalse(importResult.IsValid);
        StringAssert.Contains(importResult.Errors[0], "category mismatch");
    }

    [TestMethod]
    public void MagicItemMapper_FromCsvRow_MapsDetailAndEditContractsWithEffectMetadata()
    {
        var row = new MagicItemCsvImportRow
        {
            DefinitionId = "WAND001",
            Name = "Wand of Fireballs",
            Category = "MagicItem",
            Type = "Wand",
            Rarity = "Rare",
            Value = "5000",
            AttunementRequired = "true",
            Charges = "7",
            Description = "Casts fireball charges",
            Source = "Import",
            Tags = "offense,wand"
        };

        var mapped = MagicItemContentMapper.FromCsvRow(row);
        Assert.IsTrue(mapped.validation.IsValid);
        Assert.IsNotNull(mapped.entry);

        mapped.entry!.EffectMetadata.PassiveBonuses.Add("+1 spell attack");
        mapped.entry.EffectMetadata.ActivatedEffects.Add("Cast Fireball");
        mapped.entry.EffectMetadata.TriggerConditions.Add("Command word");
        mapped.entry.EffectMetadata.DurationOrExpiryHint = "Until charges are depleted";
        mapped.entry.EffectMetadata.SaveOrCheckReferences.Add("DEX Save DC 15");
        mapped.entry.Metadata.Extensions["AD&D1.SpecialAttack"] = "Fireball";
        mapped.entry.Metadata.Extensions["BRP.CheckRef"] = "POWx5";

        var detail = MagicItemContentMapper.ToDetailDto(mapped.entry);
        var edit = new MagicItemEditDto
        {
            Id = detail.Id,
            Name = detail.Name,
            ItemType = detail.ItemType,
            Rarity = detail.Rarity,
            AttunementRequired = detail.AttunementRequired,
            ChargesOrUses = detail.ChargesOrUses,
            Value = detail.Value,
            Description = detail.Description,
            Source = detail.Source,
            Tags = detail.Tags,
            EffectMetadata = detail.EffectMetadata,
            Extensions = detail.Extensions
        };
        var remapped = MagicItemContentMapper.FromEditDto(edit);

        Assert.AreEqual("WAND001", detail.Id);
        Assert.AreEqual("Wand", detail.ItemType);
        Assert.IsTrue(detail.AttunementRequired);
        Assert.AreEqual(7, detail.ChargesOrUses);
        Assert.AreEqual("Cast Fireball", detail.EffectMetadata.ActivatedEffects[0]);
        Assert.AreEqual(2, detail.Extensions.Count);
        Assert.AreEqual("Wand of Fireballs", remapped.Metadata.Name);
        Assert.AreEqual("Rare", remapped.Rarity);
        Assert.AreEqual(5000m, remapped.Value);
    }

    [TestMethod]
    public void MagicItemValidation_InvalidUsageOrMissingType_Fails()
    {
        var magicItem = new MagicItemContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "MI-BAD",
                Name = "Broken Relic",
                CategoryType = "MagicItem",
                Source = "Manual"
            },
            ItemType = string.Empty,
            Rarity = "Rare",
            AttunementRequired = false,
            ChargesOrUses = -1,
            Value = -5,
            Description = "",
            EffectMetadata = new MagicItemEffectMetadata()
        };

        var validation = ContentValidationUtilities.ValidateEntry(magicItem);

        Assert.IsFalse(validation.IsValid);
        Assert.IsTrue(validation.Errors.Count >= 3);
    }

    [TestMethod]
    public void EquipmentMapper_FromCsvRow_MapsListDetailEditDtoFlow()
    {
        var row = new EquipmentCsvImportRow
        {
            DefinitionId = "SWD001",
            Name = "Longsword",
            Category = "Equipment",
            Type = "Weapon",
            Value = "15",
            Weight = "3",
            WeightUnit = "lb",
            Description = "Standard martial blade",
            Source = "Import",
            Tags = "weapon,starter"
        };

        var mapped = EquipmentContentMapper.FromCsvRow(row);
        Assert.IsTrue(mapped.validation.IsValid);
        Assert.IsNotNull(mapped.entry);

        mapped.entry!.Metadata.Extensions["AD&D1.WeaponSpeed"] = 5;
        mapped.entry.Metadata.Extensions["BRP.SkillInteraction"] = "Sword";

        var listDto = EquipmentContentMapper.ToListItemDto(mapped.entry);
        var detailDto = EquipmentContentMapper.ToDetailDto(mapped.entry);
        var editDto = new EquipmentEditDto
        {
            Id = detailDto.Id,
            Name = detailDto.Name,
            EquipmentType = detailDto.EquipmentType,
            EquipmentCategory = detailDto.EquipmentCategory,
            BaseCost = detailDto.BaseCost,
            Weight = detailDto.Weight,
            WeightUnit = detailDto.WeightUnit,
            Description = detailDto.Description,
            Source = detailDto.Source,
            Tags = detailDto.Tags,
            Extensions = detailDto.Extensions
        };
        var remapped = EquipmentContentMapper.FromEditDto(editDto);

        Assert.AreEqual("SWD001", listDto.Id);
        Assert.AreEqual("Longsword", detailDto.Name);
        Assert.AreEqual("Weapon", detailDto.EquipmentType);
        Assert.AreEqual(15m, detailDto.BaseCost);
        Assert.AreEqual("lb", detailDto.WeightUnit);
        Assert.AreEqual(2, detailDto.Extensions.Count);
        Assert.AreEqual("Longsword", remapped.Metadata.Name);
        Assert.AreEqual(15m, remapped.BaseCost);
    }

    [TestMethod]
    public void EquipmentValidation_EdgeCases_FailForRequiredAndNumericConstraints()
    {
        var equipment = new EquipmentContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "EQBAD",
                Name = "Broken Gear",
                CategoryType = "Equipment",
                Source = "Manual"
            },
            EquipmentType = string.Empty,
            EquipmentCategory = string.Empty,
            BaseCost = -1,
            Weight = -2,
            WeightUnit = string.Empty,
            Description = ""
        };

        var validation = ContentValidationUtilities.ValidateEntry(equipment);

        Assert.IsFalse(validation.IsValid);
        Assert.IsTrue(validation.Errors.Count >= 5);
    }

    [TestMethod]
    public void TreasureMapper_FromCsvRow_MapsCoreFieldsAndOptionalLinkingReferences()
    {
        var row = new TreasureCsvImportRow
        {
            DefinitionId = "TRS001",
            Name = "Goblin Coin Pouch",
            Category = "Treasure",
            TreasureType = "Coin",
            Value = "27",
            Currency = "gp",
            Quantity = "1",
            Description = "Small pouch of mixed coins",
            Source = "Import",
            Tags = "starter,loot",
            EncounterReferenceId = "enc-001",
            ScenarioReferenceId = "scn-001",
            PlacementContainerReferenceId = "container-001"
        };

        var mapped = TreasureContentMapper.FromCsvRow(row);

        Assert.IsTrue(mapped.validation.IsValid);
        Assert.IsNotNull(mapped.entry);
        Assert.AreEqual("TRS001", mapped.entry!.Metadata.Id);
        Assert.AreEqual("Goblin Coin Pouch", mapped.entry.Metadata.Name);
        Assert.AreEqual("Coin", mapped.entry.TreasureType);
        Assert.AreEqual(27m, mapped.entry.BaseValue);
        Assert.AreEqual("gp", mapped.entry.CurrencyOrValueUnit);
        Assert.AreEqual(1, mapped.entry.Quantity);
        Assert.AreEqual("enc-001", mapped.entry.EncounterReferenceId);
        Assert.AreEqual("scn-001", mapped.entry.ScenarioReferenceId);
        Assert.AreEqual("container-001", mapped.entry.PlacementContainerReferenceId);
    }

    [TestMethod]
    public void TreasureValidation_CompositionAndNumericRules_FailWhenInvalid()
    {
        var treasure = new TreasureContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "TRS002",
                Name = "Invalid Bundle",
                CategoryType = "Treasure",
                Source = "Manual"
            },
            TreasureType = string.Empty,
            BaseValue = -1,
            CurrencyOrValueUnit = "gp",
            Quantity = -2,
            Components =
            [
                new TreasureComponentEntry
                {
                    ComponentId = string.Empty,
                    Name = "Gem",
                    ComponentType = string.Empty,
                    Quantity = -1,
                    ValueContribution = -10
                }
            ]
        };

        var validation = ContentValidationUtilities.ValidateEntry(treasure);

        Assert.IsFalse(validation.IsValid);
        Assert.IsTrue(validation.Errors.Count >= 5);
    }

    [TestMethod]
    public void Repository_Extensibility_AllowsUnitCounterCategoryWithoutCoreShapeChange()
    {
        IContentRepository repository = new InMemoryContentRepository();

        repository.Save(new UnitCounterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "UNIT001",
                Name = "Infantry Counter",
                CategoryType = "Unit",
                Source = "Pack"
            },
            Strength = 4,
            Movement = 3
        });

        var listed = repository.List(ContentCategory.UnitCounter);

        Assert.AreEqual(1, listed.Count);
        Assert.AreEqual(ContentCategory.UnitCounter, listed[0].Category);
    }
}
