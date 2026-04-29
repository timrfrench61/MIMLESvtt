namespace MIMLESvtt.src.Domain.Models.Placement
{
    public class Location
    {
        public string SurfaceId { get; set; } = string.Empty;

        public Coordinate Coordinate { get; set; } = new();

        public string ZoneId { get; set; } = string.Empty;

        public string LayerId { get; set; } = string.Empty;
    }
}
