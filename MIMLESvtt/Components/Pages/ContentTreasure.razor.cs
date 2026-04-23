namespace MIMLESvtt.Components.Pages;

public partial class ContentTreasure
{
    private enum TreasureEditorMode
    {
        Create,
        Edit
    }

    private enum TreasureSort
    {
        NameAsc,
        NameDesc,
        TypeAsc,
        TypeDesc
    }

    private sealed class TreasureListItem
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string TreasureType { get; init; } = string.Empty;
        public decimal Value { get; init; }
        public int Quantity { get; init; }
    }

    private string searchQuery = string.Empty;
    private TreasureSort sortOption = TreasureSort.NameAsc;
    private string selectedTreasureId = "TRS001";
    private TreasureEditorMode editorMode = TreasureEditorMode.Create;
    private string editId = string.Empty;
    private string editName = string.Empty;
    private string editTreasureType = string.Empty;
    private decimal? editValue;
    private int? editQuantity;
    private string editorValidationMessage = string.Empty;
    private bool editorValidationPassed;
    private string editorSaveMessage = string.Empty;
    private bool editorSaveSucceeded;

    private readonly List<TreasureListItem> treasureItems =
    [
        new() { Id = "TRS001", Name = "Goblin Coin Pouch", TreasureType = "Coin", Value = 27, Quantity = 1 },
        new() { Id = "TRS002", Name = "Small Gem Cache", TreasureType = "Bundle", Value = 150, Quantity = 3 },
        new() { Id = "TRS003", Name = "Ancient Relic", TreasureType = "Artifact", Value = 900, Quantity = 1 },
        new() { Id = "TRS004", Name = "Bandit Stash", TreasureType = "Hoard", Value = 320, Quantity = 2 }
    ];

    private List<TreasureListItem> VisibleTreasure
    {
        get
        {
            IEnumerable<TreasureListItem> query = treasureItems;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var term = searchQuery.Trim();
                query = query.Where(t =>
                    t.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || t.Id.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || t.TreasureType.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            query = sortOption switch
            {
                TreasureSort.NameDesc => query.OrderByDescending(t => t.Name, StringComparer.OrdinalIgnoreCase),
                TreasureSort.TypeAsc => query.OrderBy(t => t.TreasureType, StringComparer.OrdinalIgnoreCase).ThenBy(t => t.Name, StringComparer.OrdinalIgnoreCase),
                TreasureSort.TypeDesc => query.OrderByDescending(t => t.TreasureType, StringComparer.OrdinalIgnoreCase).ThenBy(t => t.Name, StringComparer.OrdinalIgnoreCase),
                _ => query.OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            };

            return query.ToList();
        }
    }

    private TreasureListItem? SelectedTreasure => treasureItems.FirstOrDefault(t => string.Equals(t.Id, selectedTreasureId, StringComparison.Ordinal));

    private bool IsEditMode => editorMode == TreasureEditorMode.Edit;

    private void SelectTreasure(string id)
    {
        selectedTreasureId = id;

        var selected = treasureItems.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.Ordinal));
        if (selected is null)
        {
            return;
        }

        editorMode = TreasureEditorMode.Edit;
        editId = selected.Id;
        editName = selected.Name;
        editTreasureType = selected.TreasureType;
        editValue = selected.Value;
        editQuantity = selected.Quantity;
        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveMessage = string.Empty;
        editorSaveSucceeded = false;
    }

    private void ClearFilters()
    {
        searchQuery = string.Empty;
        sortOption = TreasureSort.NameAsc;
    }

    private void StartCreateMode()
    {
        editorMode = TreasureEditorMode.Create;
        editId = string.Empty;
        editName = string.Empty;
        editTreasureType = string.Empty;
        editValue = null;
        editQuantity = null;
        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveMessage = string.Empty;
        editorSaveSucceeded = false;
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

        if (string.IsNullOrWhiteSpace(editTreasureType))
        {
            editorValidationPassed = false;
            editorValidationMessage = "Treasure type is required.";
            return;
        }

        if (!editValue.HasValue)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Value is required.";
            return;
        }

        if (editValue.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Value must be 0 or greater.";
            return;
        }

        if (!editQuantity.HasValue)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Quantity is required.";
            return;
        }

        if (editQuantity.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Quantity must be 0 or greater.";
            return;
        }

        editorValidationPassed = true;
        editorValidationMessage = "Treasure editor form passed required-field and numeric validation.";
    }

    private void SaveTreasure()
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
        var normalizedType = editTreasureType.Trim();

        if (editorMode == TreasureEditorMode.Create)
        {
            if (treasureItems.Any(t => string.Equals(t.Id, normalizedId, StringComparison.Ordinal)))
            {
                editorSaveSucceeded = false;
                editorSaveMessage = "Save failed. Treasure id already exists.";
                return;
            }

            treasureItems.Add(new TreasureListItem
            {
                Id = normalizedId,
                Name = normalizedName,
                TreasureType = normalizedType,
                Value = editValue!.Value,
                Quantity = editQuantity!.Value
            });

            selectedTreasureId = normalizedId;
            editorMode = TreasureEditorMode.Edit;
            editorSaveSucceeded = true;
            editorSaveMessage = "Saved new treasure.";
            return;
        }

        var existingIndex = treasureItems.FindIndex(t => string.Equals(t.Id, selectedTreasureId, StringComparison.Ordinal));
        if (existingIndex < 0)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Selected treasure no longer exists.";
            return;
        }

        var duplicateIdExists = treasureItems.Any(t =>
            !string.Equals(t.Id, selectedTreasureId, StringComparison.Ordinal)
            && string.Equals(t.Id, normalizedId, StringComparison.Ordinal));

        if (duplicateIdExists)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Treasure id already exists.";
            return;
        }

        treasureItems[existingIndex] = new TreasureListItem
        {
            Id = normalizedId,
            Name = normalizedName,
            TreasureType = normalizedType,
            Value = editValue!.Value,
            Quantity = editQuantity!.Value
        };

        selectedTreasureId = normalizedId;
        editorSaveSucceeded = true;
        editorSaveMessage = "Saved treasure changes.";
    }

    private void CancelEdit()
    {
        if (editorMode == TreasureEditorMode.Edit)
        {
            var selected = treasureItems.FirstOrDefault(t => string.Equals(t.Id, selectedTreasureId, StringComparison.Ordinal));
            if (selected is not null)
            {
                editId = selected.Id;
                editName = selected.Name;
                editTreasureType = selected.TreasureType;
                editValue = selected.Value;
                editQuantity = selected.Quantity;
            }
        }
        else
        {
            editId = string.Empty;
            editName = string.Empty;
            editTreasureType = string.Empty;
            editValue = null;
            editQuantity = null;
        }

        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveSucceeded = true;
        editorSaveMessage = "Canceled edit changes.";
    }

    internal void TestSelectTreasure(string id) => SelectTreasure(id);
    internal void TestSetCreateMode() => StartCreateMode();
    internal void TestSetEditorFields(string id, string name, string treasureType, decimal? value, int? quantity)
    {
        editId = id;
        editName = name;
        editTreasureType = treasureType;
        editValue = value;
        editQuantity = quantity;
    }

    internal void TestValidateEditorForm() => ValidateEditorForm();
    internal void TestSaveTreasure() => SaveTreasure();
    internal void TestCancelEdit() => CancelEdit();
    internal string TestEditorValidationMessage => editorValidationMessage;
    internal bool TestEditorValidationPassed => editorValidationPassed;
    internal string TestEditorSaveMessage => editorSaveMessage;
    internal bool TestEditorSaveSucceeded => editorSaveSucceeded;
    internal string TestSelectedTreasureName => SelectedTreasure?.Name ?? string.Empty;
    internal bool TestTreasureExists(string id, string expectedName)
        => treasureItems.Any(t => string.Equals(t.Id, id, StringComparison.Ordinal) && string.Equals(t.Name, expectedName, StringComparison.Ordinal));
}
