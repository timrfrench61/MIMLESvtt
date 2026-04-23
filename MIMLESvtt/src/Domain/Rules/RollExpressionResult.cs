using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules
{
    public class RollExpressionResult
    {
        public string Expression { get; set; } = string.Empty;

        public int DiceCount { get; set; }

        public int Sides { get; set; }

        public int Modifier { get; set; }

        public DiceRollResult RollResult { get; set; } = new();
    }
}
