using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Snapshot;

namespace MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC
{
    public class VttScenarioCandidateActivationRequest
    {
        public VttSession? VttSessionCandidate { get; init; }

        public SnapshotImportApplyContext? TargetContext { get; init; }

        public VttScenarioCandidateActivationMode Mode { get; init; }
    }
}
