using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Scenario;

namespace MIMLESvtt.src.Domain.Persistence.Snapshot
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

        public VttSession? ResultingVttSession { get; init; }

        public string? PendingVttScenarioTitle { get; init; }

        public VttScenarioPendingApplicationPlan? PendingVttScenarioPlan { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }

        public SnapshotImportErrorCode ErrorCode { get; init; }

        public SnapshotImportFailureStage FailureStage { get; init; }
    }
}
