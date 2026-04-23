using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules
{
    public class RollExpressionResult
    {
        public string Expression { get; set; } = string.Empty;

        public int DiceCount { get; set; }

        public int Sides { get; set; }

        public int Modifier { get; set; }

    public string ContextTag { get; set; } = string.Empty;

    public RollExpressionParseErrorCode ParseErrorCode { get; set; } = RollExpressionParseErrorCode.None;

    public string ParseErrorMessage { get; set; } = string.Empty;

        public DiceRollResult RollResult { get; set; } = new();
    }
}
