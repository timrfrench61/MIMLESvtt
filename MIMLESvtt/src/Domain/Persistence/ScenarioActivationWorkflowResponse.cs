namespace MIMLESvtt.src
{
    public class ScenarioActivationWorkflowResponse
    {
        public string RequestId { get; init; } = string.Empty;

        public bool IsSuccess { get; init; }

        public bool IsSupported { get; init; }

        public bool PendingScenarioPlanCreated { get; init; }

        public bool CandidateCreated { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public ScenarioCandidateActivationMode ActivationMode { get; init; }

        public TableSession? ResultingCurrentTableSession { get; init; }

        public PendingScenarioApplicationPlan? PendingScenarioPlan { get; init; }

        public TableSession? TableSessionCandidate { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }

        public SnapshotImportErrorCode ErrorCode { get; init; }

        public SnapshotImportFailureStage FailureStage { get; init; }
    }
}
