using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class SnapshotImportApplyResult
    {
        public SnapshotImportApplyResult(
            SnapshotImportApplyOperationKind operationKind,
            bool isSuccess,
            bool isApplied,
            bool isRuntimeStateMutated,
            VttSession? resultingTableSession,
            PendingScenarioApplicationPlan? pendingScenarioPlan,
            string message)
        {
            OperationKind = operationKind;
            IsSuccess = isSuccess;
            IsApplied = isApplied;
            IsRuntimeStateMutated = isRuntimeStateMutated;
            ResultingTableSession = resultingTableSession;
            PendingScenarioPlan = pendingScenarioPlan;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public SnapshotImportApplyOperationKind OperationKind { get; }

        public bool IsSuccess { get; }

        public bool IsApplied { get; }

        public bool IsRuntimeStateMutated { get; }

        public VttSession? ResultingTableSession { get; }

        public PendingScenarioApplicationPlan? PendingScenarioPlan { get; }

        public string Message { get; }
    }
}
