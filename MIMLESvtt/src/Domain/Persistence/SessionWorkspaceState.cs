namespace MIMLESvtt.src
{
    public class SessionWorkspaceState
    {
        public TableSession? CurrentTableSession { get; internal set; }

        public string? CurrentFilePath { get; internal set; }

        public SnapshotFormatKind? CurrentSnapshotFormat { get; internal set; }

        public bool IsDirty { get; internal set; }

        public string? LastOperationMessage { get; internal set; }

        public PendingScenarioApplicationPlan? CurrentPendingScenarioPlan { get; internal set; }

        public string? PendingScenarioSourcePath { get; internal set; }

        public List<WorkspaceOperationEntry> OperationHistory { get; } = [];
    }
}
