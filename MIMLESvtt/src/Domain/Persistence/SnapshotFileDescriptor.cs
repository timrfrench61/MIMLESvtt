namespace MIMLESvtt.src
{
    public class SnapshotFileDescriptor
    {
        public string FullPath { get; init; } = string.Empty;

        public string FileName { get; init; } = string.Empty;

        public string Extension { get; init; } = string.Empty;

        public SnapshotFormatKind DetectedFormatKind { get; init; }

        public DateTime? LastWriteTimeUtc { get; init; }

        public bool Exists { get; init; }
    }
}
