using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules
{
    public class DiceRandomizationService
    {
        private readonly Random _random;

        public DiceRandomizationService()
            : this(new Random())
        {
        }

        public DiceRandomizationService(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public DiceRollResult RollD4(int count = 1, int modifier = 0) => RollDie(4, count, modifier);

        public DiceRollResult RollD6(int count = 1, int modifier = 0) => RollDie(6, count, modifier);

        public DiceRollResult RollD8(int count = 1, int modifier = 0) => RollDie(8, count, modifier);

        public DiceRollResult RollD10(int count = 1, int modifier = 0) => RollDie(10, count, modifier);

        public DiceRollResult RollD12(int count = 1, int modifier = 0) => RollDie(12, count, modifier);

        public DiceRollResult RollD20(int count = 1, int modifier = 0) => RollDie(20, count, modifier);

        public DiceRollResult RollD100(int count = 1, int modifier = 0) => RollDie(100, count, modifier);

        public RollExpressionResult RollExpression(string expression)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(expression);

            var trimmed = expression.Trim();
            var dIndex = trimmed.IndexOf('d');
            if (dIndex <= 0 || dIndex == trimmed.Length - 1)
            {
                throw new ArgumentException("Roll expression must be in NdS(+/-M) format.", nameof(expression));
            }

            var countPart = trimmed[..dIndex];
            if (!int.TryParse(countPart, out var count) || count <= 0)
            {
                throw new ArgumentException("Roll expression dice count is invalid.", nameof(expression));
            }

            var sidesAndModifier = trimmed[(dIndex + 1)..];
            var modifier = 0;
            var modifierIndex = FindModifierIndex(sidesAndModifier);

            string sidesPart;
            if (modifierIndex >= 0)
            {
                sidesPart = sidesAndModifier[..modifierIndex];
                var modifierPart = sidesAndModifier[modifierIndex..];
                if (!int.TryParse(modifierPart, out modifier))
                {
                    throw new ArgumentException("Roll expression modifier is invalid.", nameof(expression));
                }
            }
            else
            {
                sidesPart = sidesAndModifier;
            }

            if (!int.TryParse(sidesPart, out var sides) || sides <= 1)
            {
                throw new ArgumentException("Roll expression die sides are invalid.", nameof(expression));
            }

            var rollResult = RollDie(sides, count, modifier);

            return new RollExpressionResult
            {
                Expression = trimmed,
                DiceCount = count,
                Sides = sides,
                Modifier = modifier,
                RollResult = rollResult
            };
        }

        public DiceRollResult RollDie(int sides, int count = 1, int modifier = 0)
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
                rolls.Add(_random.Next(1, sides + 1));
            }

            return new DiceRollResult
            {
                Sides = sides,
                Count = count,
                Rolls = rolls,
                Modifier = modifier,
                Total = rolls.Sum() + modifier
            };
        }

        private static int FindModifierIndex(string value)
        {
            for (var i = 1; i < value.Length; i++)
            {
                var current = value[i];
                if (current == '+' || current == '-')
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
