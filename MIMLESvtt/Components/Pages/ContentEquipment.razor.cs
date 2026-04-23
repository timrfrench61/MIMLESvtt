namespace MIMLESvtt.Components.Pages;

public partial class ContentEquipment
{
    private enum EquipmentEditorMode
    {
        Create,
        Edit
    }

    private enum EquipmentSort
    {
        NameAsc,
        NameDesc,
        CategoryAsc,
        CategoryDesc
    }

    private sealed class EquipmentListItem
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public decimal Cost { get; init; }
        public decimal Weight { get; init; }
        public string WeightUnit { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Source { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public Dictionary<string, string> Extensions { get; init; } = new(StringComparer.Ordinal);
    }

    private sealed class ExtensionEditorRow
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    private string searchQuery = string.Empty;
    private string categoryFilter = string.Empty;
    private EquipmentSort sortOption = EquipmentSort.NameAsc;
    private string selectedEquipmentId = "EQ001";
    private EquipmentEditorMode editorMode = EquipmentEditorMode.Create;
    private string editId = string.Empty;
    private string editName = string.Empty;
    private string editCategory = string.Empty;
    private string editType = string.Empty;
    private decimal? editCost;
    private decimal? editWeight;
    private string editWeightUnit = string.Empty;
    private string editDescription = string.Empty;
    private string editSource = string.Empty;
    private string editTags = string.Empty;
    private List<ExtensionEditorRow> extensionRows = [];
    private string editorValidationMessage = string.Empty;
    private bool editorValidationPassed;
    private string editorSaveMessage = string.Empty;
    private bool editorSaveSucceeded;

    private readonly List<EquipmentListItem> equipmentItems =
    [
        new() { Id = "EQ001", Name = "Longsword", Category = "Weapon", Type = "Melee", Cost = 15, Weight = 3, WeightUnit = "lb", Description = "Standard martial blade", Source = "Manual", Tags = "weapon,starter", Extensions = new Dictionary<string, string>(StringComparer.Ordinal) { ["AD&D1.WeaponSpeed"] = "5" } },
        new() { Id = "EQ002", Name = "Shield", Category = "Armor", Type = "Defense", Cost = 10, Weight = 6, WeightUnit = "lb", Description = "Basic defensive shield", Source = "Manual", Tags = "armor", Extensions = new Dictionary<string, string>(StringComparer.Ordinal) },
        new() { Id = "EQ003", Name = "Rope", Category = "Tool", Type = "Utility", Cost = 1, Weight = 5, WeightUnit = "lb", Description = "General utility rope", Source = "Import", Tags = "tool", Extensions = new Dictionary<string, string>(StringComparer.Ordinal) },
        new() { Id = "EQ004", Name = "Ration Pack", Category = "Consumable", Type = "Supply", Cost = 2, Weight = 1, WeightUnit = "lb", Description = "Travel food pack", Source = "Pack", Tags = "consumable", Extensions = new Dictionary<string, string>(StringComparer.Ordinal) }
    ];

    private List<string> AvailableCategories => equipmentItems
        .Select(i => i.Category)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
        .ToList();

    private List<string> AvailableTypes => equipmentItems
        .Select(i => i.Type)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
        .ToList();

    private List<EquipmentListItem> VisibleEquipment
    {
        get
        {
            IEnumerable<EquipmentListItem> query = equipmentItems;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var term = searchQuery.Trim();
                query = query.Where(i =>
                    i.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || i.Id.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || i.Category.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(categoryFilter))
            {
                query = query.Where(i => string.Equals(i.Category, categoryFilter, StringComparison.OrdinalIgnoreCase));
            }

            query = sortOption switch
            {
                EquipmentSort.NameDesc => query.OrderByDescending(i => i.Name, StringComparer.OrdinalIgnoreCase),
                EquipmentSort.CategoryAsc => query.OrderBy(i => i.Category, StringComparer.OrdinalIgnoreCase).ThenBy(i => i.Name, StringComparer.OrdinalIgnoreCase),
                EquipmentSort.CategoryDesc => query.OrderByDescending(i => i.Category, StringComparer.OrdinalIgnoreCase).ThenBy(i => i.Name, StringComparer.OrdinalIgnoreCase),
                _ => query.OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            };

            return query.ToList();
        }
    }

    private EquipmentListItem? SelectedEquipment => equipmentItems.FirstOrDefault(i => string.Equals(i.Id, selectedEquipmentId, StringComparison.Ordinal));

    private bool IsEditMode => editorMode == EquipmentEditorMode.Edit;

    private void SelectEquipment(string id)
    {
        selectedEquipmentId = id;

        var selected = equipmentItems.FirstOrDefault(i => string.Equals(i.Id, id, StringComparison.Ordinal));
        if (selected is null)
        {
            return;
        }

        editorMode = EquipmentEditorMode.Edit;
        editId = selected.Id;
        editName = selected.Name;
        editCategory = selected.Category;
        editType = selected.Type;
        editCost = selected.Cost;
        editWeight = selected.Weight;
        editWeightUnit = selected.WeightUnit;
        editDescription = selected.Description;
        editSource = selected.Source;
        editTags = selected.Tags;
        extensionRows = selected.Extensions.Select(e => new ExtensionEditorRow { Key = e.Key, Value = e.Value }).ToList();
        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveMessage = string.Empty;
        editorSaveSucceeded = false;
    }

    private void ClearFilters()
    {
        searchQuery = string.Empty;
        categoryFilter = string.Empty;
        sortOption = EquipmentSort.NameAsc;
    }

    private void StartCreateMode()
    {
        editorMode = EquipmentEditorMode.Create;
        editId = string.Empty;
        editName = string.Empty;
        editCategory = string.Empty;
        editType = string.Empty;
        editCost = null;
        editWeight = null;
        editWeightUnit = "lb";
        editDescription = string.Empty;
        editSource = "Manual";
        editTags = string.Empty;
        extensionRows = [];
        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveMessage = string.Empty;
        editorSaveSucceeded = false;
    }

    private void OnEditorCategoryChanged(string value)
    {
        editCategory = value ?? string.Empty;
    }

    private void OnEditorTypeChanged(string value)
    {
        editType = value ?? string.Empty;
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

        if (string.IsNullOrWhiteSpace(editType))
        {
            editorValidationPassed = false;
            editorValidationMessage = "Type is required.";
            return;
        }

        if (!editCost.HasValue)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Cost is required.";
            return;
        }

        if (editCost.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Cost must be 0 or greater.";
            return;
        }

        if (!editWeight.HasValue)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Weight is required.";
            return;
        }

        if (editWeight.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Weight must be 0 or greater.";
            return;
        }

        for (var i = 0; i < extensionRows.Count; i++)
        {
            var row = extensionRows[i];
            if (string.IsNullOrWhiteSpace(row.Key) && !string.IsNullOrWhiteSpace(row.Value))
            {
                editorValidationPassed = false;
                editorValidationMessage = $"Extension row {i + 1} requires a non-empty key.";
                return;
            }
        }

        editorValidationPassed = true;
        editorValidationMessage = "Equipment editor form passed required-field, numeric, and extension validation.";
    }

    private void SaveEquipment()
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
        var normalizedCategory = editCategory.Trim();
        var normalizedType = editType.Trim();
        var normalizedWeightUnit = string.IsNullOrWhiteSpace(editWeightUnit) ? "lb" : editWeightUnit.Trim();
        var normalizedDescription = editDescription?.Trim() ?? string.Empty;
        var normalizedSource = string.IsNullOrWhiteSpace(editSource) ? "Manual" : editSource.Trim();
        var normalizedTags = editTags?.Trim() ?? string.Empty;
        var normalizedExtensions = extensionRows
            .Where(r => !string.IsNullOrWhiteSpace(r.Key))
            .GroupBy(r => r.Key.Trim(), StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Last().Value?.Trim() ?? string.Empty, StringComparer.Ordinal);

        if (editorMode == EquipmentEditorMode.Create)
        {
            if (equipmentItems.Any(i => string.Equals(i.Id, normalizedId, StringComparison.Ordinal)))
            {
                editorSaveSucceeded = false;
                editorSaveMessage = "Save failed. Equipment id already exists.";
                return;
            }

            equipmentItems.Add(new EquipmentListItem
            {
                Id = normalizedId,
                Name = normalizedName,
                Category = normalizedCategory,
                Type = normalizedType,
                Cost = editCost!.Value,
                Weight = editWeight!.Value,
                WeightUnit = normalizedWeightUnit,
                Description = normalizedDescription,
                Source = normalizedSource,
                Tags = normalizedTags,
                Extensions = normalizedExtensions
            });

            selectedEquipmentId = normalizedId;
            editorMode = EquipmentEditorMode.Edit;
            editorSaveSucceeded = true;
            editorSaveMessage = "Saved new equipment and returned to detail/list flow.";
            return;
        }

        var existingIndex = equipmentItems.FindIndex(i => string.Equals(i.Id, selectedEquipmentId, StringComparison.Ordinal));
        if (existingIndex < 0)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Selected equipment no longer exists.";
            return;
        }

        var duplicateIdExists = equipmentItems.Any(i =>
            !string.Equals(i.Id, selectedEquipmentId, StringComparison.Ordinal)
            && string.Equals(i.Id, normalizedId, StringComparison.Ordinal));

        if (duplicateIdExists)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Equipment id already exists.";
            return;
        }

        equipmentItems[existingIndex] = new EquipmentListItem
        {
            Id = normalizedId,
            Name = normalizedName,
            Category = normalizedCategory,
            Type = normalizedType,
            Cost = editCost!.Value,
            Weight = editWeight!.Value,
            WeightUnit = normalizedWeightUnit,
            Description = normalizedDescription,
            Source = normalizedSource,
            Tags = normalizedTags,
            Extensions = normalizedExtensions
        };

        selectedEquipmentId = normalizedId;
        editorSaveSucceeded = true;
        editorSaveMessage = "Saved equipment changes and returned to detail/list flow.";
    }

    private void AddExtensionRow()
    {
        extensionRows.Add(new ExtensionEditorRow());
    }

    private void RemoveExtensionRow(int index)
    {
        if (index < 0 || index >= extensionRows.Count)
        {
            return;
        }

        extensionRows.RemoveAt(index);
    }

    private void CancelEdit()
    {
        if (editorMode == EquipmentEditorMode.Edit)
        {
            var selected = equipmentItems.FirstOrDefault(i => string.Equals(i.Id, selectedEquipmentId, StringComparison.Ordinal));
            if (selected is not null)
            {
                editId = selected.Id;
                editName = selected.Name;
                editCategory = selected.Category;
                editType = selected.Type;
                editCost = selected.Cost;
                editWeight = selected.Weight;
                editWeightUnit = selected.WeightUnit;
                editDescription = selected.Description;
                editSource = selected.Source;
                editTags = selected.Tags;
                extensionRows = selected.Extensions.Select(e => new ExtensionEditorRow { Key = e.Key, Value = e.Value }).ToList();
            }
        }
        else
        {
            editId = string.Empty;
            editName = string.Empty;
            editCategory = string.Empty;
            editType = string.Empty;
            editCost = null;
            editWeight = null;
            editWeightUnit = "lb";
            editDescription = string.Empty;
            editSource = "Manual";
            editTags = string.Empty;
            extensionRows = [];
        }

        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveSucceeded = true;
        editorSaveMessage = "Canceled edit changes. No mutation persisted; returned to detail/list flow.";
    }

    internal void TestSelectEquipment(string id) => SelectEquipment(id);
    internal void TestSetCreateMode() => StartCreateMode();
    internal void TestSetEditorFields(string id, string name, string category, string type, decimal? cost, decimal? weight)
    {
        editId = id;
        editName = name;
        editCategory = category;
        editType = type;
        editCost = cost;
        editWeight = weight;
    }
    internal void TestSetOptionalFields(string weightUnit, string description, string source, string tags)
    {
        editWeightUnit = weightUnit;
        editDescription = description;
        editSource = source;
        editTags = tags;
    }
    internal void TestSetExtensions(params (string Key, string Value)[] entries)
    {
        extensionRows = entries.Select(e => new ExtensionEditorRow { Key = e.Key, Value = e.Value }).ToList();
    }

    internal void TestValidateEditorForm() => ValidateEditorForm();
    internal void TestSaveEquipment() => SaveEquipment();
    internal void TestCancelEdit() => CancelEdit();
    internal string TestEditorValidationMessage => editorValidationMessage;
    internal bool TestEditorValidationPassed => editorValidationPassed;
    internal string TestEditorSaveMessage => editorSaveMessage;
    internal bool TestEditorSaveSucceeded => editorSaveSucceeded;
    internal string TestSelectedEquipmentName => SelectedEquipment?.Name ?? string.Empty;
    internal string TestSelectedEquipmentId => SelectedEquipment?.Id ?? string.Empty;
    internal string TestSelectedEquipmentExtensionValue(string key)
        => SelectedEquipment is null
            ? string.Empty
            : (SelectedEquipment.Extensions.TryGetValue(key, out var value) ? value : string.Empty);
    internal bool TestEquipmentExists(string id, string expectedName)
        => equipmentItems.Any(i => string.Equals(i.Id, id, StringComparison.Ordinal) && string.Equals(i.Name, expectedName, StringComparison.Ordinal));
}
