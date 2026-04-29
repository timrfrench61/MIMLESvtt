using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Workspace
{
    public class WorkspaceRestoreOptions
    {
        public VttScenarioModeMissingPending MissingPendingVttScenarioBehavior { get; init; } = VttScenarioModeMissingPending.FailRestore;

        public static WorkspaceRestoreOptions Default { get; } = new();
    }
}
