using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Models.Visibility;

namespace MIMLESvtt.src.Domain.Models
{
    public class VttSession
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public List<Participant> Participants { get; set; } = [];

        public List<PlayerSeat> PlayerSeats { get; set; } = [];

        public List<SurfaceInstance> Surfaces { get; set; } = [];

        public List<PieceInstance> Pieces { get; set; } = [];

        public List<string> TurnOrder { get; set; } = [];

        public int CurrentTurnIndex { get; set; }

        public int TurnNumber { get; set; } = 1;

        public string CurrentPhase { get; set; } = string.Empty;

        public TurnState TurnState { get; set; } = new();

        public TabletopOptions Options { get; set; } = new();

        public VisibilityState Visibility { get; set; } = new();

        public List<ActionRecord> ActionLog { get; set; } = [];

        public Dictionary<string, object> ModuleState { get; set; } = [];
    }

}
