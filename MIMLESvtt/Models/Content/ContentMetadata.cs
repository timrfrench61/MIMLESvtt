namespace MIMLESvtt.src.Domain.Models.Content;

public class ContentMetadata
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string CategoryType { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    public string VersionRevision { get; set; } = string.Empty;

    public Dictionary<string, object> Extensions { get; set; } = [];
}
