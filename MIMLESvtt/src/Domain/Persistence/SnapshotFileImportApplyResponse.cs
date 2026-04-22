using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class SnapshotFileImportApplyResponse
    {
        public string RequestId { get; init; } = string.Empty;

        public string FilePath { get; init; } = string.Empty;

        public SnapshotFormatKind? DetectedFormat { get; init; }

        public bool IsSuccess { get; init; }

        public bool IsSupported { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public VttSession? ResultingCurrentTableSession { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }

        public SnapshotImportErrorCode ErrorCode { get; init; }

        public SnapshotImportFailureStage FailureStage { get; init; }

        public SnapshotImportApplyResponse? TableSessionApplyResponse { get; init; }

        public ScenarioActivationWorkflowResponse? ScenarioActivationResponse { get; init; }

        public SnapshotImportApplicationOutcome? ImportOutcome { get; init; }
    }
}
