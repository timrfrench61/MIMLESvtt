namespace MIMLESvtt.src
{
    public class SnapshotImportResponse
    {
        public string RequestId { get; init; } = string.Empty;

        public bool IsSuccess { get; init; }

        public SnapshotImportApplicationOutcome? Outcome { get; init; }

        public string? ErrorMessage { get; init; }

        public SnapshotImportErrorCode ErrorCode { get; init; }

        public SnapshotImportFailureStage FailureStage { get; init; }
    }
}
