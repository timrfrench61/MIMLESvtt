namespace MIMLESvtt.src.Domain.Rules;

public enum RollExpressionParseErrorCode
{
    None,
    EmptyExpression,
    MissingDiceDelimiter,
    MissingDiceCount,
    InvalidDiceCount,
    MissingDiceSides,
    InvalidDiceSides,
    InvalidModifier
}
