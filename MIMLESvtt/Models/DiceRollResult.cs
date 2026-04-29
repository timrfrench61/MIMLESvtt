namespace MIMLESvtt.src.Domain.Models
{
    public class DiceRollResult
    {
        public int Sides { get; set; }

        public int Count { get; set; } = 1;

        public List<int> Rolls { get; set; } = [];

        public int Modifier { get; set; }

        public int Total { get; set; }

    public DateTime TimestampUtc { get; set; }

    public string ContextTag { get; set; } = string.Empty;

    public string RandomProvider { get; set; } = string.Empty;
    }
}
