namespace MIMLESvtt.src
{
    public class PendingScenarioApplicationPlan
    {
        public string ScenarioTitle { get; init; } = string.Empty;

        public ScenarioExport Scenario { get; init; } = new();

        public SnapshotImportApplyOperationKind IntendedOperationKind { get; init; }

        public bool IsReadyForApply { get; init; }

        public int SurfaceCount { get; init; }

        public int PieceCount { get; init; }
    }
}
