namespace MIMLESvtt.src.Domain.Models
{
    public class TurnState
    {
        public string CurrentSeatId { get; set; } = string.Empty;

        public string CurrentPhase { get; set; } = string.Empty;

        public int CurrentPhaseIndex { get; set; }

        public int RoundNumber { get; set; } = 1;

        public int TurnNumber { get; set; } = 1;

        public List<string> PhaseSequence { get; set; } = [];

        public Dictionary<string, object> Metadata { get; set; } = [];
    }
}
