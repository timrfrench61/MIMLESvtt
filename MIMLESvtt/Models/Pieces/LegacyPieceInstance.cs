using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.Models.Visibility;

namespace MIMLESvtt.src.Domain.Models.Pieces
{
    public class PieceInstance
    {
        public string Id { get; set; } = string.Empty;

        public string DefinitionId { get; set; } = string.Empty;

        public Location Location { get; set; } = new();

        public Rotation Rotation { get; set; } = new();

        public string OwnerParticipantId { get; set; } = string.Empty;

        public VisibilityState Visibility { get; set; } = new();

        public Dictionary<string, object> State { get; set; } = [];

        public List<string> MarkerIds { get; set; } = [];

        public string StackId { get; set; } = string.Empty;

        public string ContainerId { get; set; } = string.Empty;
    }
}
