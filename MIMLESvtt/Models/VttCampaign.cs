namespace MIMLESvtt.src.Domain.Models;

public class VttCampaign
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string SessionId { get; set; } = string.Empty;

    public string GameboxId { get; set; } = string.Empty;

    public bool IsReadOnly { get; set; }

    public bool IsHidden { get; set; }

    public List<string> ScenarioIds { get; set; } = [];

    public VttScenario? CurrentScenarioSnapshot { get; set; }
}

public class Campaign : VttCampaign
{
}
