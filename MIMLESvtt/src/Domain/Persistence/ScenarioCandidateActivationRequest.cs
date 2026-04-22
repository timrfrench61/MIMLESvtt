using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class ScenarioCandidateActivationRequest
    {
        public VttSession? TableSessionCandidate { get; init; }

        public SnapshotImportApplyContext? TargetContext { get; init; }

        public ScenarioCandidateActivationMode Mode { get; init; }
    }
}
