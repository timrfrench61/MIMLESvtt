using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Scenario;
using MIMLESvtt.src.Domain.Persistence.Snapshot;

namespace MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC
{
    public class VttScenarioActivationWorkflowResponse
    {
        public string RequestId { get; init; } = string.Empty;

        public bool IsSuccess { get; init; }

        public bool IsSupported { get; init; }

        public bool PendingVttScenarioPlanCreated { get; init; }

        public bool CandidateCreated { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public VttScenarioCandidateActivationMode ActivationMode { get; init; }

        public VttSession? ResultingCurrentVttSession { get; init; }

        public VttScenarioPendingApplicationPlan? PendingVttScenarioPlan { get; init; }

        public VttSession? VttSessionCandidate { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }

        public SnapshotImportErrorCode ErrorCode { get; init; }

        public SnapshotImportFailureStage FailureStage { get; init; }
    }
}
