using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.Workspace;

namespace MIMLESvtt.src.Domain.Persistence.VttSessionNSPC
{
    public class VttSessionWorkspaceRecoveryState
    {
        public string? CurrentFilePath { get; init; }

        public SnapshotFormatKind? CurrentSnapshotFormat { get; init; }

        public string? PendingVttScenarioSourcePath { get; init; }

        public List<WorkspaceOperationEntry> RecentOperationHistory { get; init; } = [];
    }
}
