namespace MIMLESvtt.src
{
    public class WorkspaceOperationEntry
    {
        public WorkspaceOperationKind OperationKind { get; init; }

        public DateTime TimestampUtc { get; init; }

        public bool Success { get; init; }

        public string? FilePath { get; init; }

        public string Message { get; init; } = string.Empty;

        public SnapshotFormatKind? SnapshotFormat { get; init; }
    }
}
