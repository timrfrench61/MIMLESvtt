namespace MIMLESvtt.src
{
    public class ChangePieceStatePayload
    {
        public string PieceId { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public object? Value { get; set; }
    }
}
