namespace MIMLESvtt.src.Domain.Models.Surfaces
{
    public class StrategicMapDefinition
    {
        public List<StrategicRegion> Regions { get; set; } = [];

        public Dictionary<string, object> Metadata { get; set; } = [];
    }
}
