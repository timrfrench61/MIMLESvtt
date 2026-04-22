using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class SnapshotImportApplyResponse
    {
        public string RequestId { get; init; } = string.Empty;

        public bool IsSuccess { get; init; }

        public bool IsSupported { get; init; }

        public bool IsApplied { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public SnapshotFormatKind? FormatKind { get; init; }

        public SnapshotImportApplyOperationKind? OperationKind { get; init; }

        public VttSession? ResultingTableSession { get; init; }

        public string? PendingScenarioTitle { get; init; }

        public PendingScenarioApplicationPlan? PendingScenarioPlan { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }

        public SnapshotImportErrorCode ErrorCode { get; init; }

        public SnapshotImportFailureStage FailureStage { get; init; }
    }
}
