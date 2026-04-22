namespace MIMLESvtt.src.Domain.Models.Surfaces
{
    public class SurfaceInstance
    {
        public string Id { get; set; } = string.Empty;

        public string DefinitionId { get; set; } = string.Empty;

        public SurfaceType Type { get; set; }

        public CoordinateSystem CoordinateSystem { get; set; }

        public List<Layer> Layers { get; set; } = [];

        public List<Zone> Zones { get; set; } = [];

        public SurfaceTransform Transform { get; set; } = new();
    }
}
