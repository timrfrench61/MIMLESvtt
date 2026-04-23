namespace MIMLESvtt.src.Domain.Models.Surfaces
{
    public class Zone
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public List<string> SpaceIds { get; set; } = [];

        public Dictionary<string, object> Metadata { get; set; } = [];

        public Dictionary<string, object> TerrainMetadata { get; set; } = [];
    }
}
