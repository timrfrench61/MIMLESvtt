namespace MIMLESvtt.src.Domain.Models.Surfaces
{
    public class StrategicRegion
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public List<string> AdjacentRegionIds { get; set; } = [];

        public Dictionary<string, object> Metadata { get; set; } = [];
    }
}
