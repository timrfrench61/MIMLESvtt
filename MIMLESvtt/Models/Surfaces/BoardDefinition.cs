namespace MIMLESvtt.src.Domain.Models.Surfaces
{
    public class BoardDefinition
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public BoardLayoutType LayoutType { get; set; } = BoardLayoutType.SquareGrid;

        public StrategicMapDefinition StrategicMap { get; set; } = new();

        public Dictionary<string, object> Metadata { get; set; } = [];
    }
}
