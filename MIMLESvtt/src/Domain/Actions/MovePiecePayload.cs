namespace MIMLESvtt.src
{
    public class MovePiecePayload
    {
        public string PieceId { get; set; } = string.Empty;

        public Location? NewLocation { get; set; }
    }
}
