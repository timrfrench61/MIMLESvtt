using MIMLESvtt.src.Domain.Models.Placement;

namespace MIMLESvtt.src.Domain.Models.Surfaces
{
    public class Space
    {
        public string Id { get; set; } = string.Empty;

        public Coordinate Coordinate { get; set; } = new();

        public HexCoordinate? HexCoordinate { get; set; }

        public List<string> OccupantPieceIds { get; set; } = [];

        public List<string> AdjacentSpaceIds { get; set; } = [];
    }
}
