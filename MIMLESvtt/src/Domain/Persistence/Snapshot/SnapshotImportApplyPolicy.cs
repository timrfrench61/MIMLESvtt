namespace MIMLESvtt.src.Domain.Persistence.Snapshot
{
    public class SnapshotImportApplyPolicy
    {
        public bool AllowReplaceVttSession { get; init; } = true;

        public bool AllowCreateVttScenarioFromImport { get; init; } = true;

        public bool AllowActivateVttScenarioCandidate { get; init; } = true;

        public static SnapshotImportApplyPolicy Default { get; } = new();
    }
}
