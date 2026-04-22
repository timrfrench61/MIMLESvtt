using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Snapshot
{
    public class SnapshotFileImportApplyResponse
    {
        public string RequestId { get; init; } = string.Empty;

        public string FilePath { get; init; } = string.Empty;

        public SnapshotFormatKind? DetectedFormat { get; init; }

        public bool IsSuccess { get; init; }

        public bool IsSupported { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public VttSession? ResultingCurrentVttSession { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }

        public SnapshotImportErrorCode ErrorCode { get; init; }

        public SnapshotImportFailureStage FailureStage { get; init; }

        public SnapshotImportApplyResponse? VttSessionApplyResponse { get; init; }

        public VttScenarioActivationWorkflowResponse? ScenarioActivationResponse { get; init; }

        public SnapshotImportApplicationOutcome? ImportOutcome { get; init; }
    }
}
