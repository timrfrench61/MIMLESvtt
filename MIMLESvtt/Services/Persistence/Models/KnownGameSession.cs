namespace MIMLESvtt.src.Domain.Persistence.Models
{
    public class KnownGameSession
    {
        public string FilePath { get; init; } = string.Empty;

        public string FileName { get; init; } = string.Empty;

        public string JoinCode { get; init; } = string.Empty;

        public bool Exists { get; init; }

        public DateTime? LastWriteTimeUtc { get; init; }

        public DateTime? LastJoinCodeUpdatedUtc { get; init; }

        public DateTime? LastJoinedUtc { get; init; }
    }
}
