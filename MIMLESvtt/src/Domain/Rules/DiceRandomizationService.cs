using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules
{
    public class DiceRandomizationService : IDiceRandomizationService
    {
        private readonly IDiceRandomProvider _randomProvider;
    private readonly IRollExpressionParser _rollExpressionParser;

        public DiceRandomizationService()
            : this(new SystemDiceRandomProvider(), new BasicRollExpressionParser())
        {
        }

        public DiceRandomizationService(Random random)
            : this(new SystemDiceRandomProvider(random), new BasicRollExpressionParser())
        {
        }

        public DiceRandomizationService(IDiceRandomProvider randomProvider)
            : this(randomProvider, new BasicRollExpressionParser())
    {
    }

    public DiceRandomizationService(IDiceRandomProvider randomProvider, IRollExpressionParser rollExpressionParser)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
        _rollExpressionParser = rollExpressionParser ?? throw new ArgumentNullException(nameof(rollExpressionParser));
        }

        public DiceRollResult RollD4(int count = 1, int modifier = 0, string? contextTag = null) => RollDie(4, count, modifier, contextTag);

        public DiceRollResult RollD6(int count = 1, int modifier = 0, string? contextTag = null) => RollDie(6, count, modifier, contextTag);

        public DiceRollResult RollD8(int count = 1, int modifier = 0, string? contextTag = null) => RollDie(8, count, modifier, contextTag);

        public DiceRollResult RollD10(int count = 1, int modifier = 0, string? contextTag = null) => RollDie(10, count, modifier, contextTag);

        public DiceRollResult RollD12(int count = 1, int modifier = 0, string? contextTag = null) => RollDie(12, count, modifier, contextTag);

        public DiceRollResult RollD20(int count = 1, int modifier = 0, string? contextTag = null) => RollDie(20, count, modifier, contextTag);

        public DiceRollResult RollD100(int count = 1, int modifier = 0, string? contextTag = null) => RollDie(100, count, modifier, contextTag);

        public RollExpressionResult RollExpression(string expression, string? contextTag = null)
        {
        var parseResult = _rollExpressionParser.Parse(expression);
        if (!parseResult.IsSuccess)
            {
            throw new ArgumentException(parseResult.ErrorMessage, nameof(expression));
            }

        var rollResult = RollDie(parseResult.Sides, parseResult.DiceCount, parseResult.Modifier, contextTag);
        var normalizedExpression = NormalizeExpression(expression, parseResult);

            return new RollExpressionResult
            {
            Expression = normalizedExpression,
            DiceCount = parseResult.DiceCount,
            Sides = parseResult.Sides,
            Modifier = parseResult.Modifier,
            ParseErrorCode = RollExpressionParseErrorCode.None,
            ParseErrorMessage = string.Empty,
                RollResult = rollResult,
                ContextTag = rollResult.ContextTag
            };
        }

        public DiceRollResult RollDie(int sides, int count = 1, int modifier = 0, string? contextTag = null)
        {
            if (sides <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(sides), "Dice sides must be greater than 1.");
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Dice count must be greater than 0.");
            }

            var rolls = new List<int>(count);
            for (var i = 0; i < count; i++)
            {
                rolls.Add(_randomProvider.Next(1, sides + 1));
            }

            return new DiceRollResult
            {
                Sides = sides,
                Count = count,
                Rolls = rolls,
                Modifier = modifier,
                Total = rolls.Sum() + modifier,
                TimestampUtc = DateTime.UtcNow,
                ContextTag = contextTag?.Trim() ?? string.Empty,
                RandomProvider = _randomProvider.ProviderName
            };
        }

    private static string NormalizeExpression(string expression, RollExpressionParseResult parseResult)
        {
        var compact = new string((expression ?? string.Empty).Where(c => !char.IsWhiteSpace(c)).ToArray());
        if (string.IsNullOrWhiteSpace(compact))
            {
            compact = $"{parseResult.DiceCount}d{parseResult.Sides}";
            }

        return compact;
        }
    }
}
