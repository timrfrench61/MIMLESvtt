using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Scenario;

namespace MIMLESvtt.src.Domain.Persistence.Snapshot
{
    public class SnapshotImportApplyResult
    {
        public SnapshotImportApplyResult(
            SnapshotImportApplyOperationKind operationKind,
            bool isSuccess,
            bool isApplied,
            bool isRuntimeStateMutated,
            VttSession? resultingVttSession,
            VttScenarioPendingApplicationPlan? pendingVttScenarioPlan,
            string message)
        {
            OperationKind = operationKind;
            IsSuccess = isSuccess;
            IsApplied = isApplied;
            IsRuntimeStateMutated = isRuntimeStateMutated;
            ResultingVttSession = resultingVttSession;
            PendingVttScenarioPlan = pendingVttScenarioPlan;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public SnapshotImportApplyOperationKind OperationKind { get; }

        public bool IsSuccess { get; }

        public bool IsApplied { get; }

        public bool IsRuntimeStateMutated { get; }

        public VttSession? ResultingVttSession { get; }

        public VttScenarioPendingApplicationPlan? PendingVttScenarioPlan { get; }
        public string Message { get; }
    }
}
