namespace MIMLESvtt.src.Domain.Rules;

public interface IRollExpressionParser
{
    RollExpressionParseResult Parse(string expression);
}
