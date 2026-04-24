namespace MIMLESvtt.src.Domain.Persistence.Models;

public class ReadonlyScenarioCatalogEntry
{
    public string ScenarioId { get; init; } = string.Empty;

    public string CampaignId { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string FilePath { get; init; } = string.Empty;

    public bool IsReadOnly { get; init; } = true;
}

public class ReadonlyCampaignCatalogEntry
{
    public string CampaignId { get; init; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsReadOnly { get; init; } = true;

    public bool IsHidden { get; set; }
}
