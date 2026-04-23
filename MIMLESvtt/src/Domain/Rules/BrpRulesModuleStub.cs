namespace MIMLESvtt.src.Domain.Rules;

public class BrpRulesModuleStub
{
    private readonly IDiceRandomizationService _dice;

    public BrpRulesModuleStub(IDiceRandomizationService dice)
    {
        _dice = dice;
    }

    public int RollPercentileSkillCheck()
    {
        var result = _dice.RollD100(contextTag: "brp:skill");
        return result.Total;
    }

    public RollExpressionResult ResolveExpression(string expression)
    {
        return _dice.RollExpression(expression, contextTag: "brp:expression");
    }
}
