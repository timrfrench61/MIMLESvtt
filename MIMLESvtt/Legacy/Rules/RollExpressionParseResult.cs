namespace MIMLESvtt.src.Domain.Rules;

public class RollExpressionParseResult
{
    public bool IsSuccess { get; init; }

    public RollExpressionParseErrorCode ErrorCode { get; init; } = RollExpressionParseErrorCode.None;

    public string ErrorMessage { get; init; } = string.Empty;

    public int DiceCount { get; init; }

    public int Sides { get; init; }

    public int Modifier { get; init; }

    public static RollExpressionParseResult Success(int diceCount, int sides, int modifier)
    {
        return new RollExpressionParseResult
        {
            IsSuccess = true,
            DiceCount = diceCount,
            Sides = sides,
            Modifier = modifier,
            ErrorCode = RollExpressionParseErrorCode.None,
            ErrorMessage = string.Empty
        };
    }

    public static RollExpressionParseResult Failure(RollExpressionParseErrorCode errorCode, string message)
    {
        return new RollExpressionParseResult
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = message
        };
    }
}
