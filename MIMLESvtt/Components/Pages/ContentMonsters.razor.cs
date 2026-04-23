namespace MIMLESvtt.Components.Pages;

public partial class ContentMonsters
{
    private enum MonsterEditorMode
    {
        Create,
        Edit
    }

    private enum MonsterSort
    {
        NameAsc,
        NameDesc,
        CategoryAsc,
        CategoryDesc
    }

    private sealed class MonsterListItem
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public int LevelOrThreat { get; init; }
        public int HitPoints { get; init; }
    }

    private string searchQuery = string.Empty;
    private MonsterSort sortOption = MonsterSort.NameAsc;
    private string selectedMonsterId = "GOB001";
    private MonsterEditorMode editorMode = MonsterEditorMode.Create;
    private string editId = string.Empty;
    private string editName = string.Empty;
    private string editCategory = string.Empty;
    private string editorValidationMessage = string.Empty;
    private bool editorValidationPassed;
    private string editorSaveMessage = string.Empty;
    private bool editorSaveSucceeded;

    private readonly List<MonsterListItem> monsters =
    [
        new() { Id = "GOB001", Name = "Goblin", Category = "Humanoid", LevelOrThreat = 1, HitPoints = 5 },
        new() { Id = "ORC001", Name = "Orc", Category = "Humanoid", LevelOrThreat = 2, HitPoints = 8 },
        new() { Id = "TROLL001", Name = "Troll", Category = "Giantkin", LevelOrThreat = 5, HitPoints = 30 },
        new() { Id = "WRAITH001", Name = "Wraith", Category = "Undead", LevelOrThreat = 6, HitPoints = 25 }
    ];

    private List<MonsterListItem> VisibleMonsters
    {
        get
        {
            IEnumerable<MonsterListItem> query = monsters;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var term = searchQuery.Trim();
                query = query.Where(m =>
                    m.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || m.Id.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || m.Category.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            query = sortOption switch
            {
                MonsterSort.NameDesc => query.OrderByDescending(m => m.Name, StringComparer.OrdinalIgnoreCase),
                MonsterSort.CategoryAsc => query.OrderBy(m => m.Category, StringComparer.OrdinalIgnoreCase).ThenBy(m => m.Name, StringComparer.OrdinalIgnoreCase),
                MonsterSort.CategoryDesc => query.OrderByDescending(m => m.Category, StringComparer.OrdinalIgnoreCase).ThenBy(m => m.Name, StringComparer.OrdinalIgnoreCase),
                _ => query.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
            };

            return query.ToList();
        }
    }

    private MonsterListItem? SelectedMonster => monsters.FirstOrDefault(m => string.Equals(m.Id, selectedMonsterId, StringComparison.Ordinal));

    private bool IsEditMode => editorMode == MonsterEditorMode.Edit;

    private void SelectMonster(string id)
    {
        selectedMonsterId = id;

        var selected = monsters.FirstOrDefault(m => string.Equals(m.Id, id, StringComparison.Ordinal));
        if (selected is null)
        {
            return;
        }

        editorMode = MonsterEditorMode.Edit;
        editId = selected.Id;
        editName = selected.Name;
        editCategory = selected.Category;
        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveMessage = string.Empty;
        editorSaveSucceeded = false;
    }

    private void ClearFilters()
    {
        searchQuery = string.Empty;
        sortOption = MonsterSort.NameAsc;
    }

    private void StartCreateMode()
    {
        editorMode = MonsterEditorMode.Create;
        editId = string.Empty;
        editName = string.Empty;
        editCategory = string.Empty;
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

        if (string.IsNullOrWhiteSpace(editCategory))
        {
            editorValidationPassed = false;
            editorValidationMessage = "Category is required.";
            return;
        }

        editorValidationPassed = true;
        editorValidationMessage = "Monster editor form passed required-field validation.";
    }

    private void SaveMonster()
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

        if (editorMode == MonsterEditorMode.Create)
        {
            if (monsters.Any(m => string.Equals(m.Id, normalizedId, StringComparison.Ordinal)))
            {
                editorSaveSucceeded = false;
                editorSaveMessage = "Save failed. Monster id already exists.";
                return;
            }

            monsters.Add(new MonsterListItem
            {
                Id = normalizedId,
                Name = normalizedName,
                Category = normalizedCategory,
                LevelOrThreat = 1,
                HitPoints = 1
            });

            selectedMonsterId = normalizedId;
            editorMode = MonsterEditorMode.Edit;
            editorSaveSucceeded = true;
            editorSaveMessage = "Saved new monster.";
            return;
        }

        var existingIndex = monsters.FindIndex(m => string.Equals(m.Id, selectedMonsterId, StringComparison.Ordinal));
        if (existingIndex < 0)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Selected monster no longer exists.";
            return;
        }

        var duplicateIdExists = monsters.Any(m =>
            !string.Equals(m.Id, selectedMonsterId, StringComparison.Ordinal)
            && string.Equals(m.Id, normalizedId, StringComparison.Ordinal));

        if (duplicateIdExists)
        {
            editorSaveSucceeded = false;
            editorSaveMessage = "Save failed. Monster id already exists.";
            return;
        }

        var existing = monsters[existingIndex];
        monsters[existingIndex] = new MonsterListItem
        {
            Id = normalizedId,
            Name = normalizedName,
            Category = normalizedCategory,
            LevelOrThreat = existing.LevelOrThreat,
            HitPoints = existing.HitPoints
        };

        selectedMonsterId = normalizedId;
        editorSaveSucceeded = true;
        editorSaveMessage = "Saved monster changes.";
    }

    private void CancelEdit()
    {
        if (editorMode == MonsterEditorMode.Edit)
        {
            var selected = monsters.FirstOrDefault(m => string.Equals(m.Id, selectedMonsterId, StringComparison.Ordinal));
            if (selected is not null)
            {
                editId = selected.Id;
                editName = selected.Name;
                editCategory = selected.Category;
            }
        }
        else
        {
            editId = string.Empty;
            editName = string.Empty;
            editCategory = string.Empty;
        }

        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveSucceeded = true;
        editorSaveMessage = "Canceled edit changes.";
    }

    internal void TestSelectMonster(string id) => SelectMonster(id);
    internal void TestSetCreateMode() => StartCreateMode();
    internal void TestSetEditorFields(string id, string name, string category)
    {
        editId = id;
        editName = name;
        editCategory = category;
    }
    internal void TestValidateEditorForm() => ValidateEditorForm();
    internal void TestSaveMonster() => SaveMonster();
    internal void TestCancelEdit() => CancelEdit();
    internal string TestEditorValidationMessage => editorValidationMessage;
    internal bool TestEditorValidationPassed => editorValidationPassed;
    internal string TestEditorSaveMessage => editorSaveMessage;
    internal bool TestEditorSaveSucceeded => editorSaveSucceeded;
    internal int TestVisibleMonsterCount() => VisibleMonsters.Count;
    internal string TestSelectedMonsterName => SelectedMonster?.Name ?? string.Empty;
    internal bool TestMonsterExists(string id, string expectedName)
        => monsters.Any(m => string.Equals(m.Id, id, StringComparison.Ordinal) && string.Equals(m.Name, expectedName, StringComparison.Ordinal));
}
