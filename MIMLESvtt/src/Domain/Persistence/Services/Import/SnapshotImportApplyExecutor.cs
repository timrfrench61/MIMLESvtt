using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Scenario;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class SnapshotImportApplyExecutor
    {
        public SnapshotImportApplyResult Execute(SnapshotImportApplyIntent intent, SnapshotImportApplyContext context)
        {
            ArgumentNullException.ThrowIfNull(intent);
            ArgumentNullException.ThrowIfNull(context);

            return intent.OperationKind switch
            {
                SnapshotImportApplyOperationKind.ReplaceVttSession => ExecuteReplaceVttSession(intent, context),
                SnapshotImportApplyOperationKind.CreateVttScenarioFromImport => ExecuteCreateScenarioFromImport(intent, context),
                SnapshotImportApplyOperationKind.Unsupported => new SnapshotImportApplyResult(
                    SnapshotImportApplyOperationKind.Unsupported,
                    false,
                    false,
                    false,
                    context.CurrentVttSession,
                    null,
                    "Unsupported import intent cannot be executed."),
                _ => throw new InvalidOperationException("Unsupported apply operation kind.")
            };
        }

        private static SnapshotImportApplyResult ExecuteReplaceVttSession(SnapshotImportApplyIntent intent, SnapshotImportApplyContext context)
        {
            if (!intent.IsSupported)
            {
                throw new InvalidOperationException("ReplaceVttSession intent must be marked supported.");
            }

            if (intent.Payload is not VttSession VttSession)
            {
                throw new InvalidOperationException("ReplaceVttSession intent must include a VttSession payload.");
            }

            context.CurrentVttSession = VttSession;

            return new SnapshotImportApplyResult(
                SnapshotImportApplyOperationKind.ReplaceVttSession,
                true,
                true,
                true,
                VttSession,
                null,
                "ReplaceVttSession intent applied successfully.");
        }

        private static SnapshotImportApplyResult ExecuteCreateScenarioFromImport(SnapshotImportApplyIntent intent, SnapshotImportApplyContext context)
        {
            if (!intent.IsSupported)
            {
                throw new InvalidOperationException("CreateScenarioFromImport intent must be marked supported.");
            }

            if (intent.Payload is not VttScenario scenario)
            {
                throw new InvalidOperationException("CreateScenarioFromImport intent must include a Scenario payload.");
            }

            var pendingVttScenarioPlan = new VttScenarioPendingApplicationPlan
            {
                ScenarioTitle = scenario.Title,
                Scenario = scenario,
                IntendedOperationKind = SnapshotImportApplyOperationKind.CreateVttScenarioFromImport,
                IsReadyForApply = true,
                SurfaceCount = scenario.Surfaces.Count,
                PieceCount = scenario.Pieces.Count
            };

            return new SnapshotImportApplyResult(
                SnapshotImportApplyOperationKind.CreateVttScenarioFromImport,
                true,
                false,
                false,
                context.CurrentVttSession,
                pendingVttScenarioPlan,
                "CreateScenarioFromImport intent produced a pending scenario application plan in this pass.");
        }
    }
}
