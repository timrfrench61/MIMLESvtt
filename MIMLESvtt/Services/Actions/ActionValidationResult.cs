namespace MIMLESvtt.src
{
    public class ActionValidationResult
    {
        public bool IsValid { get; init; }

        public string Message { get; init; } = string.Empty;

        public static ActionValidationResult Success()
        {
            return new ActionValidationResult
            {
                IsValid = true,
                Message = string.Empty
            };
        }

        public static ActionValidationResult Failure(string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            return new ActionValidationResult
            {
                IsValid = false,
                Message = message
            };
        }
    }
}
