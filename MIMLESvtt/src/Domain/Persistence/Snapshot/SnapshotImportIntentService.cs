using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Persistence.Snapshot
{
    public class SnapshotImportIntentService
    {
        public SnapshotImportApplyIntent CreateIntent(SnapshotImportApplicationOutcome applicationOutcome)
        {
            ArgumentNullException.ThrowIfNull(applicationOutcome);

            return applicationOutcome.FormatKind switch
            {
                SnapshotFormatKind.VttSessionSnapshot => CreateVttSessionIntent(applicationOutcome),
                SnapshotFormatKind.VttScenarioSnapshot => CreateVttScenarioIntent(applicationOutcome),
                SnapshotFormatKind.VttContentPackSnapshot => CreateUnsupportedIntent(applicationOutcome),
                SnapshotFormatKind.ActionLogSnapshot => CreateUnsupportedIntent(applicationOutcome),
                _ => throw new InvalidOperationException("Unsupported snapshot format kind.")
            };
        }

        private static SnapshotImportApplyIntent CreateVttSessionIntent(SnapshotImportApplicationOutcome applicationOutcome)
        {
            if (!applicationOutcome.IsSupported)
            {
                return CreateUnsupportedIntent(applicationOutcome);
            }

            if (applicationOutcome.Payload is not VttSession VttSession)
            {
                throw new InvalidOperationException("Supported VttSessionSnapshot outcome must include a VttSession payload.");
            }

            return new SnapshotImportApplyIntent(
                SnapshotFormatKind.VttSessionSnapshot,
                SnapshotImportApplyOperationKind.ReplaceVttSession,
                true,
                VttSession,
                "VttSessionSnapshot can be applied as ReplaceVttSession.");
        }

        private static SnapshotImportApplyIntent CreateVttScenarioIntent(SnapshotImportApplicationOutcome applicationOutcome)
        {
            if (!applicationOutcome.IsSupported)
            {
                return CreateUnsupportedIntent(applicationOutcome);
            }

            if (applicationOutcome.Payload is not Scenario scenario)
            {
                throw new InvalidOperationException("Supported ScenarioSnapshot outcome must include a Scenario payload.");
            }

            return new SnapshotImportApplyIntent(
                SnapshotFormatKind.VttScenarioSnapshot,
                SnapshotImportApplyOperationKind.CreateVttScenarioFromImport,
                true,
                scenario,
                "VttScenarioSnapshot can be applied as CreateVttScenarioFromImport.");
        }

        private static SnapshotImportApplyIntent CreateUnsupportedIntent(SnapshotImportApplicationOutcome applicationOutcome)
        {
            return new SnapshotImportApplyIntent(
                applicationOutcome.FormatKind,
                SnapshotImportApplyOperationKind.Unsupported,
                false,
                null,
                applicationOutcome.Message);
        }
    }
}
