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
        public decimal Movement { get; init; }
        public int ArmorOrDefense { get; init; }
        public string AttackProfile { get; init; } = string.Empty;
        public string Source { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public Dictionary<string, string> Extensions { get; init; } = new(StringComparer.Ordinal);
    }

    private sealed class ExtensionEditorRow
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    private string searchQuery = string.Empty;
    private MonsterSort sortOption = MonsterSort.NameAsc;
    private string selectedMonsterId = "GOB001";
    private MonsterEditorMode editorMode = MonsterEditorMode.Create;
    private string editId = string.Empty;
    private string editName = string.Empty;
    private string editCategory = string.Empty;
    private int? editLevelOrThreat;
    private int? editHitPoints;
    private decimal? editMovement;
    private int? editArmorOrDefense;
    private string editAttackProfile = string.Empty;
    private string editSource = string.Empty;
    private string editTags = string.Empty;
    private string editDescription = string.Empty;
    private List<ExtensionEditorRow> extensionRows = [];
    private string editorValidationMessage = string.Empty;
    private bool editorValidationPassed;
    private string editorSaveMessage = string.Empty;
    private bool editorSaveSucceeded;

    private readonly List<MonsterListItem> monsters =
    [
        new() { Id = "GOB001", Name = "Goblin", Category = "Humanoid", LevelOrThreat = 1, HitPoints = 5, Movement = 9, ArmorOrDefense = 6, AttackProfile = "Scimitar +0", Source = "Manual", Tags = "starter", Description = "Common cave raider", Extensions = new Dictionary<string, string>(StringComparer.Ordinal) { ["AD&D1.THAC0"] = "19" } },
        new() { Id = "ORC001", Name = "Orc", Category = "Humanoid", LevelOrThreat = 2, HitPoints = 8, Movement = 9, ArmorOrDefense = 5, AttackProfile = "Axe +1", Source = "Manual", Tags = "brute", Description = "Aggressive front-line raider", Extensions = new Dictionary<string, string>(StringComparer.Ordinal) },
        new() { Id = "TROLL001", Name = "Troll", Category = "Giantkin", LevelOrThreat = 5, HitPoints = 30, Movement = 12, ArmorOrDefense = 4, AttackProfile = "Claw/Claw/Bite", Source = "Import", Tags = "regeneration", Description = "Regenerating giantkin predator", Extensions = new Dictionary<string, string>(StringComparer.Ordinal) { ["BRP.Special"] = "Regenerate" } },
        new() { Id = "WRAITH001", Name = "Wraith", Category = "Undead", LevelOrThreat = 6, HitPoints = 25, Movement = 12, ArmorOrDefense = 3, AttackProfile = "Life drain", Source = "Pack", Tags = "undead,incorporeal", Description = "Incorporeal undead hunter", Extensions = new Dictionary<string, string>(StringComparer.Ordinal) }
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
        editLevelOrThreat = selected.LevelOrThreat;
        editHitPoints = selected.HitPoints;
        editMovement = selected.Movement;
        editArmorOrDefense = selected.ArmorOrDefense;
        editAttackProfile = selected.AttackProfile;
        editSource = selected.Source;
        editTags = selected.Tags;
        editDescription = selected.Description;
        extensionRows = selected.Extensions
            .Select(e => new ExtensionEditorRow { Key = e.Key, Value = e.Value })
            .ToList();
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
        editLevelOrThreat = null;
        editHitPoints = null;
        editMovement = null;
        editArmorOrDefense = null;
        editAttackProfile = string.Empty;
        editSource = "Manual";
        editTags = string.Empty;
        editDescription = string.Empty;
        extensionRows = [];
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

        if (editLevelOrThreat.HasValue && editLevelOrThreat.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Level/Threat must be 0 or greater when provided.";
            return;
        }

        if (editHitPoints.HasValue && editHitPoints.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Hit Points must be 0 or greater when provided.";
            return;
        }

        if (editMovement.HasValue && editMovement.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Movement must be 0 or greater when provided.";
            return;
        }

        if (editArmorOrDefense.HasValue && editArmorOrDefense.Value < 0)
        {
            editorValidationPassed = false;
            editorValidationMessage = "Armor/Defense must be 0 or greater when provided.";
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
        editorValidationMessage = "Monster editor form passed required-field and extension validation.";
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
        var normalizedExtensions = extensionRows
            .Where(r => !string.IsNullOrWhiteSpace(r.Key))
            .GroupBy(r => r.Key.Trim(), StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Last().Value?.Trim() ?? string.Empty, StringComparer.Ordinal);

        var normalizedSource = string.IsNullOrWhiteSpace(editSource) ? "Manual" : editSource.Trim();
        var normalizedTags = editTags?.Trim() ?? string.Empty;
        var normalizedDescription = editDescription?.Trim() ?? string.Empty;

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
                LevelOrThreat = editLevelOrThreat ?? 0,
                HitPoints = editHitPoints ?? 0,
                Movement = editMovement ?? 0,
                ArmorOrDefense = editArmorOrDefense ?? 0,
                AttackProfile = string.IsNullOrWhiteSpace(editAttackProfile) ? "(none)" : editAttackProfile.Trim(),
                Source = normalizedSource,
                Tags = normalizedTags,
                Description = normalizedDescription,
                Extensions = normalizedExtensions
            });

            selectedMonsterId = normalizedId;
            editorMode = MonsterEditorMode.Edit;
            editorSaveSucceeded = true;
            editorSaveMessage = "Saved new monster and returned to detail/list flow.";
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
            LevelOrThreat = editLevelOrThreat ?? existing.LevelOrThreat,
            HitPoints = editHitPoints ?? existing.HitPoints,
            Movement = editMovement ?? existing.Movement,
            ArmorOrDefense = editArmorOrDefense ?? existing.ArmorOrDefense,
            AttackProfile = string.IsNullOrWhiteSpace(editAttackProfile) ? existing.AttackProfile : editAttackProfile.Trim(),
            Source = normalizedSource,
            Tags = normalizedTags,
            Description = normalizedDescription,
            Extensions = normalizedExtensions
        };

        selectedMonsterId = normalizedId;
        editorSaveSucceeded = true;
        editorSaveMessage = "Saved monster changes and returned to detail/list flow.";
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
        if (editorMode == MonsterEditorMode.Edit)
        {
            var selected = monsters.FirstOrDefault(m => string.Equals(m.Id, selectedMonsterId, StringComparison.Ordinal));
            if (selected is not null)
            {
                editId = selected.Id;
                editName = selected.Name;
                editCategory = selected.Category;
                editLevelOrThreat = selected.LevelOrThreat;
                editHitPoints = selected.HitPoints;
                editMovement = selected.Movement;
                editArmorOrDefense = selected.ArmorOrDefense;
                editAttackProfile = selected.AttackProfile;
                editSource = selected.Source;
                editTags = selected.Tags;
                editDescription = selected.Description;
                extensionRows = selected.Extensions
                    .Select(e => new ExtensionEditorRow { Key = e.Key, Value = e.Value })
                    .ToList();
            }
        }
        else
        {
            editId = string.Empty;
            editName = string.Empty;
            editCategory = string.Empty;
            editLevelOrThreat = null;
            editHitPoints = null;
            editMovement = null;
            editArmorOrDefense = null;
            editAttackProfile = string.Empty;
            editSource = "Manual";
            editTags = string.Empty;
            editDescription = string.Empty;
            extensionRows = [];
        }

        editorValidationMessage = string.Empty;
        editorValidationPassed = false;
        editorSaveSucceeded = true;
        editorSaveMessage = "Canceled edit changes. No mutation persisted; returned to detail/list flow.";
    }

    internal void TestSelectMonster(string id) => SelectMonster(id);
    internal void TestSetCreateMode() => StartCreateMode();
    internal void TestSetEditorFields(string id, string name, string category)
    {
        editId = id;
        editName = name;
        editCategory = category;
    }
    internal void TestSetOptionalFields(int? levelOrThreat, int? hitPoints, decimal? movement, int? armorOrDefense, string attackProfile, string source, string tags, string description)
    {
        editLevelOrThreat = levelOrThreat;
        editHitPoints = hitPoints;
        editMovement = movement;
        editArmorOrDefense = armorOrDefense;
        editAttackProfile = attackProfile;
        editSource = source;
        editTags = tags;
        editDescription = description;
    }
    internal void TestSetExtensions(params (string Key, string Value)[] entries)
    {
        extensionRows = entries
            .Select(e => new ExtensionEditorRow { Key = e.Key, Value = e.Value })
            .ToList();
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
    internal string TestSelectedMonsterId => SelectedMonster?.Id ?? string.Empty;
    internal string TestSelectedMonsterExtensionValue(string key)
        => SelectedMonster is null
            ? string.Empty
            : (SelectedMonster.Extensions.TryGetValue(key, out var value) ? value : string.Empty);
    internal bool TestMonsterExists(string id, string expectedName)
        => monsters.Any(m => string.Equals(m.Id, id, StringComparison.Ordinal) && string.Equals(m.Name, expectedName, StringComparison.Ordinal));
}
