namespace MIMLESvtt.src
{
    public class SnapshotImportApplyPolicy
    {
        public bool AllowReplaceTableSession { get; init; } = true;

        public bool AllowCreateScenarioFromImport { get; init; } = true;

        public bool AllowActivateScenarioCandidate { get; init; } = true;

        public static SnapshotImportApplyPolicy Default { get; } = new();
    }
}
