namespace MIMLESvtt.Components.Pages;

public partial class ContentMagicItems
{
    private enum MagicItemEditorMode
    {
        Create,
        Edit
    }

    private sealed class MagicItemListItem
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string ItemType { get; init; } = string.Empty;
        public string Rarity { get; init; } = string.Empty;
        public bool AttunementRequired { get; init; }
        public int? Charges { get; init; }
        public string EffectMetadata { get; init; } = string.Empty;
    }

    private string searchQuery = string.Empty;
    private string rarityFilter = string.Empty;
    private string typeFilter = string.Empty;
    private string selectedMagicItemId = "MI001";

    private MagicItemEditorMode editorMode = MagicItemEditorMode.Create;
    private string editId = string.Empty;
    private string editName = string.Empty;
    private string editItemType = string.Empty;
    private string editRarity = string.Empty;
    private bool editAttunementRequired;
    private int? editCharges;
    private string editEffectMetadata = string.Empty;

    private string editorValidationMessage = string.Empty;
    private bool editorValidationPassed;
    private string editorSaveMessage = string.Empty;
    private bool editorSaveSucceeded;

    private readonly List<MagicItemListItem> magicItems =
    [
        new() { Id = "MI001", Name = "Ring of Protection", ItemType = "Ring", Rarity = "Rare", AttunementRequired = true, Charges = null, EffectMetadata = "Passive defense bonus" },
        new() { Id = "MI002", Name = "Wand of Fireballs", ItemType = "Wand", Rarity = "Very Rare", AttunementRequired = false, Charges = 7, EffectMetadata = "Activated fireball effect" },
        new() { Id = "MI003", Name = "Potion of Healing", ItemType = "Potion", Rarity = "Common", AttunementRequired = false, Charges = 1, EffectMetadata = "Consumable healing effect" },
        new() { Id = "MI004", Name = "Cloak of Shadows", ItemType = "Wondrous", Rarity = "Uncommon", AttunementRequired = true, Charges = null, EffectMetadata = "Stealth advantage while worn" }
    ];

    private List<string> AvailableRarities => magicItems
        .Select(i => i.Rarity)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
        .ToList();

    private List<string> AvailableTypes => magicItems
        .Select(i => i.ItemType)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
        .ToList();

    private List<MagicItemListItem> VisibleMagicItems
    {
        get
        {
            IEnumerable<MagicItemListItem> query = magicItems;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var term = searchQuery.Trim();
                query = query.Where(i =>
                    i.Id.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || i.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || i.ItemType.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || i.Rarity.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(rarityFilter))
            {
                query = query.Where(i => string.Equals(i.Rarity, rarityFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                query = query.Where(i => string.Equals(i.ItemType, typeFilter, StringComparison.OrdinalIgnoreCase));
            }

            return query.OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }

    private MagicItemListItem? SelectedMagicItem => magicItems.FirstOrDefault(i => string.Equals(i.Id, selectedMagicItemId, StringComparison.Ordinal));

    private bool IsEditMode => editorMode == MagicItemEditorMode.Edit;

    private void SelectMagicItem(string id)
    {
        selectedMagicItemId = id;

        var selected = magicItems.FirstOrDefault(i => string.Equals(i.Id, id, StringComparison.Ordinal));
        if (selected is null)
        {
            return;
        }

        editorMode = MagicItemEditorMode.Edit;
        editId = selected.Id;
        editName = selected.Name;
        editItemType = selected.ItemType;
        editRarity = selected.Rarity;
        editAttunementRequired = selected.AttunementRequired;
        editCharges = selected.Charges;
        editEffectMetadata = selected.EffectMetadata;
        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveMessage = string.Empty;
        editorSaveSucceeded = false;
    }

    private void StartCreateMode()
    {
        editorMode = MagicItemEditorMode.Create;
        editId = string.Empty;
        editName = string.Empty;
        editItemType = string.Empty;
        editRarity = string.Empty;
        editAttunementRequired = false;
        editCharges = null;
        editEffectMetadata = string.Empty;
        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveMessage = string.Empty;
        editorSaveSucceeded = false;
    }

    private void ClearFilters()
    {
        searchQuery = string.Empty;
        rarityFilter = string.Empty;
        typeFilter = string.Empty;
    }

    private void ValidateEditorForm()
    {
        if (string.IsNullOrWhiteSpace(editId))
        {
            editorValidationPassed = false;
            editorValidationMessage = "Id is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(editName))
        {
            editorValidationPassed = false;
            editorValidationMessage = "Name is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(editItemType))
        {
            editorValidationPassed = false;
            editorValidationMessage = "Type is required.";
            return;
        }

        if (editCharges.HasValue && editCharges.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Charges must be 0 or greater when provided.";
            return;
        }

        editorValidationPassed = true;
        editorValidationMessage = "Magic item editor form passed validation.";
    }

    private void SaveMagicItem()
    {
        ValidateEditorForm();
        if (!editorValidationPassed)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Fix validation errors first.";
            return;
        }

        var normalizedId = editId.Trim();
        var normalizedName = editName.Trim();
        var normalizedType = editItemType.Trim();
        var normalizedRarity = editRarity?.Trim() ?? string.Empty;
        var normalizedEffectMetadata = editEffectMetadata?.Trim() ?? string.Empty;

        if (editorMode == MagicItemEditorMode.Create)
        {
            if (magicItems.Any(i => string.Equals(i.Id, normalizedId, StringComparison.Ordinal)))
            {
                editorSaveSucceeded = false;
                editorSaveMessage = "Save failed. Magic item id already exists.";
                return;
            }

            magicItems.Add(new MagicItemListItem
            {
                Id = normalizedId,
                Name = normalizedName,
                ItemType = normalizedType,
                Rarity = normalizedRarity,
                AttunementRequired = editAttunementRequired,
                Charges = editCharges,
                EffectMetadata = normalizedEffectMetadata
            });

            selectedMagicItemId = normalizedId;
            editorMode = MagicItemEditorMode.Edit;
            editorSaveSucceeded = true;
            editorSaveMessage = "Saved new magic item.";
            return;
        }

        var existingIndex = magicItems.FindIndex(i => string.Equals(i.Id, selectedMagicItemId, StringComparison.Ordinal));
        if (existingIndex < 0)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Selected magic item no longer exists.";
            return;
        }

        var duplicateIdExists = magicItems.Any(i =>
            !string.Equals(i.Id, selectedMagicItemId, StringComparison.Ordinal)
            && string.Equals(i.Id, normalizedId, StringComparison.Ordinal));

        if (duplicateIdExists)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Magic item id already exists.";
            return;
        }

        magicItems[existingIndex] = new MagicItemListItem
        {
            Id = normalizedId,
            Name = normalizedName,
            ItemType = normalizedType,
            Rarity = normalizedRarity,
            AttunementRequired = editAttunementRequired,
            Charges = editCharges,
            EffectMetadata = normalizedEffectMetadata
        };

        selectedMagicItemId = normalizedId;
        editorSaveSucceeded = true;
        editorSaveMessage = "Saved magic item changes.";
    }

    private void CancelEdit()
    {
        if (editorMode == MagicItemEditorMode.Edit)
        {
            var selected = magicItems.FirstOrDefault(i => string.Equals(i.Id, selectedMagicItemId, StringComparison.Ordinal));
            if (selected is not null)
            {
                editId = selected.Id;
                editName = selected.Name;
                editItemType = selected.ItemType;
                editRarity = selected.Rarity;
                editAttunementRequired = selected.AttunementRequired;
                editCharges = selected.Charges;
                editEffectMetadata = selected.EffectMetadata;
            }
        }
        else
        {
            editId = string.Empty;
            editName = string.Empty;
            editItemType = string.Empty;
            editRarity = string.Empty;
            editAttunementRequired = false;
            editCharges = null;
            editEffectMetadata = string.Empty;
        }

        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveSucceeded = true;
        editorSaveMessage = "Canceled edit changes.";
    }

    internal void TestSelectMagicItem(string id) => SelectMagicItem(id);
    internal void TestSetCreateMode() => StartCreateMode();
    internal void TestSetEditorFields(string id, string name, string type, string rarity, bool attunementRequired, int? charges, string effectMetadata)
    {
        editId = id;
        editName = name;
        editItemType = type;
        editRarity = rarity;
        editAttunementRequired = attunementRequired;
        editCharges = charges;
        editEffectMetadata = effectMetadata;
    }

    internal void TestValidateEditorForm() => ValidateEditorForm();
    internal void TestSaveMagicItem() => SaveMagicItem();
    internal void TestCancelEdit() => CancelEdit();
    internal string TestEditorValidationMessage => editorValidationMessage;
    internal bool TestEditorValidationPassed => editorValidationPassed;
    internal string TestEditorSaveMessage => editorSaveMessage;
    internal bool TestEditorSaveSucceeded => editorSaveSucceeded;
    internal string TestSelectedMagicItemName => SelectedMagicItem?.Name ?? string.Empty;
    internal bool TestMagicItemExists(string id, string expectedName)
        => magicItems.Any(i => string.Equals(i.Id, id, StringComparison.Ordinal) && string.Equals(i.Name, expectedName, StringComparison.Ordinal));
}
