namespace MIMLESvtt.src
{
    public class SessionWorkspaceRecoveryState
    {
        public string? CurrentFilePath { get; init; }

        public SnapshotFormatKind? CurrentSnapshotFormat { get; init; }

        public string? PendingScenarioSourcePath { get; init; }

        public List<WorkspaceOperationEntry> RecentOperationHistory { get; init; } = [];
    }
}
