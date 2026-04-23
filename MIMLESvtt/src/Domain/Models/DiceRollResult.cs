namespace MIMLESvtt.src.Domain.Models
{
    public class DiceRollResult
    {
        public int Sides { get; set; }

        public int Count { get; set; } = 1;

        public List<int> Rolls { get; set; } = [];

        public int Modifier { get; set; }

        public int Total { get; set; }
    }
}
