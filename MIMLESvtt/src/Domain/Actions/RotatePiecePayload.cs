namespace MIMLESvtt.src
{
    public class RotatePiecePayload
    {
        public string PieceId { get; set; } = string.Empty;

        public Rotation? NewRotation { get; set; }
    }
}
