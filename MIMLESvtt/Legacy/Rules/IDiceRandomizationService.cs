using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules;

public interface IDiceRandomizationService
{
    DiceRollResult RollD4(int count = 1, int modifier = 0, string? contextTag = null);

    DiceRollResult RollD6(int count = 1, int modifier = 0, string? contextTag = null);

    DiceRollResult RollD8(int count = 1, int modifier = 0, string? contextTag = null);

    DiceRollResult RollD10(int count = 1, int modifier = 0, string? contextTag = null);

    DiceRollResult RollD12(int count = 1, int modifier = 0, string? contextTag = null);

    DiceRollResult RollD20(int count = 1, int modifier = 0, string? contextTag = null);

    DiceRollResult RollD100(int count = 1, int modifier = 0, string? contextTag = null);

    RollExpressionResult RollExpression(string expression, string? contextTag = null);

    DiceRollResult RollDie(int sides, int count = 1, int modifier = 0, string? contextTag = null);
}
