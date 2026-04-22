using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class SnapshotImportApplyExecutor
    {
        public SnapshotImportApplyResult Execute(SnapshotImportApplyIntent intent, SnapshotImportApplyContext context)
        {
            ArgumentNullException.ThrowIfNull(intent);
            ArgumentNullException.ThrowIfNull(context);

            return intent.OperationKind switch
            {
                SnapshotImportApplyOperationKind.ReplaceTableSession => ExecuteReplaceTableSession(intent, context),
                SnapshotImportApplyOperationKind.CreateScenarioFromImport => ExecuteCreateScenarioFromImport(intent, context),
                SnapshotImportApplyOperationKind.Unsupported => new SnapshotImportApplyResult(
                    SnapshotImportApplyOperationKind.Unsupported,
                    false,
                    false,
                    false,
                    context.CurrentTableSession,
                    null,
                    "Unsupported import intent cannot be executed."),
                _ => throw new InvalidOperationException("Unsupported apply operation kind.")
            };
        }

        private static SnapshotImportApplyResult ExecuteReplaceTableSession(SnapshotImportApplyIntent intent, SnapshotImportApplyContext context)
        {
            if (!intent.IsSupported)
            {
                throw new InvalidOperationException("ReplaceTableSession intent must be marked supported.");
            }

            if (intent.Payload is not VttSession tableSession)
            {
                throw new InvalidOperationException("ReplaceTableSession intent must include a TableSession payload.");
            }

            context.CurrentTableSession = tableSession;

            return new SnapshotImportApplyResult(
                SnapshotImportApplyOperationKind.ReplaceTableSession,
                true,
                true,
                true,
                tableSession,
                null,
                "ReplaceTableSession intent applied successfully.");
        }

        private static SnapshotImportApplyResult ExecuteCreateScenarioFromImport(SnapshotImportApplyIntent intent, SnapshotImportApplyContext context)
        {
            if (!intent.IsSupported)
            {
                throw new InvalidOperationException("CreateScenarioFromImport intent must be marked supported.");
            }

            if (intent.Payload is not Scenario scenario)
            {
                throw new InvalidOperationException("CreateScenarioFromImport intent must include a Scenario payload.");
            }

            var pendingScenarioPlan = new PendingScenarioApplicationPlan
            {
                ScenarioTitle = scenario.Title,
                Scenario = scenario,
                IntendedOperationKind = SnapshotImportApplyOperationKind.CreateScenarioFromImport,
                IsReadyForApply = true,
                SurfaceCount = scenario.Surfaces.Count,
                PieceCount = scenario.Pieces.Count
            };

            return new SnapshotImportApplyResult(
                SnapshotImportApplyOperationKind.CreateScenarioFromImport,
                true,
                false,
                false,
                context.CurrentTableSession,
                pendingScenarioPlan,
                "CreateScenarioFromImport intent produced a pending scenario application plan in this pass.");
        }
    }
}
