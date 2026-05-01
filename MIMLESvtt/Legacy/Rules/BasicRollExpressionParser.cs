namespace MIMLESvtt.src.Domain.Rules;

public class BasicRollExpressionParser : IRollExpressionParser
{
    public RollExpressionParseResult Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.EmptyExpression, "Roll expression is required.");
        }

        var compact = new string(expression.Where(c => !char.IsWhiteSpace(c)).ToArray());
        if (string.IsNullOrWhiteSpace(compact))
        {
            return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.EmptyExpression, "Roll expression is required.");
        }

        var dIndex = compact.IndexOf('d');
        if (dIndex < 0)
        {
            dIndex = compact.IndexOf('D');
        }

        if (dIndex < 0)
        {
            return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.MissingDiceDelimiter, "Roll expression must include 'd' dice delimiter.");
        }

        if (dIndex == 0)
        {
            return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.MissingDiceCount, "Roll expression dice count is required before 'd'.");
        }

        var countPart = compact[..dIndex];
        if (!int.TryParse(countPart, out var count) || count <= 0)
        {
            return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.InvalidDiceCount, "Roll expression dice count is invalid.");
        }

        if (dIndex == compact.Length - 1)
        {
            return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.MissingDiceSides, "Roll expression die sides are required after 'd'.");
        }

        var sidesAndModifier = compact[(dIndex + 1)..];
        var modifier = 0;
        var modifierIndex = FindModifierIndex(sidesAndModifier);

        string sidesPart;
        if (modifierIndex >= 0)
        {
            sidesPart = sidesAndModifier[..modifierIndex];
            var modifierPart = sidesAndModifier[modifierIndex..];
            if (!int.TryParse(modifierPart, out modifier))
            {
                return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.InvalidModifier, "Roll expression modifier is invalid.");
            }
        }
        else
        {
            sidesPart = sidesAndModifier;
        }

        if (string.IsNullOrWhiteSpace(sidesPart))
        {
            return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.MissingDiceSides, "Roll expression die sides are required after 'd'.");
        }

        if (!int.TryParse(sidesPart, out var sides) || sides <= 1)
        {
            return RollExpressionParseResult.Failure(RollExpressionParseErrorCode.InvalidDiceSides, "Roll expression die sides are invalid.");
        }

        return RollExpressionParseResult.Success(count, sides, modifier);
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
