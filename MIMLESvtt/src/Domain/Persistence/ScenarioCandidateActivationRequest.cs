namespace MIMLESvtt.src
{
    public class ScenarioCandidateActivationRequest
    {
        public TableSession? TableSessionCandidate { get; init; }

        public SnapshotImportApplyContext? TargetContext { get; init; }

        public ScenarioCandidateActivationMode Mode { get; init; }
    }
}
