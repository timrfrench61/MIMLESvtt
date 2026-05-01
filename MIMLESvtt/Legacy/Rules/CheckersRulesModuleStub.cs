namespace MIMLESvtt.src.Domain.Rules;

public class CheckersRulesModuleStub
{
    private readonly IDiceRandomizationService _dice;

    public CheckersRulesModuleStub(IDiceRandomizationService dice)
    {
        _dice = dice;
    }

    public int RollInitiativeLikeValue()
    {
        var result = _dice.RollD6(contextTag: "checkers:init");
        return result.Total;
    }

    public RollExpressionResult ResolveExpression(string expression)
    {
        return _dice.RollExpression(expression, contextTag: "checkers:expression");
    }
}
