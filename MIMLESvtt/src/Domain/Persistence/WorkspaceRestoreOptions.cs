namespace MIMLESvtt.src
{
    public class WorkspaceRestoreOptions
    {
        public MissingPendingScenarioMode MissingPendingScenarioBehavior { get; init; } = MissingPendingScenarioMode.FailRestore;

        public static WorkspaceRestoreOptions Default { get; } = new();
    }
}
