namespace MIMLESvtt.src
{
    public class SnapshotImportIntentService
    {
        public SnapshotImportApplyIntent CreateIntent(SnapshotImportApplicationOutcome applicationOutcome)
        {
            ArgumentNullException.ThrowIfNull(applicationOutcome);

            return applicationOutcome.FormatKind switch
            {
                SnapshotFormatKind.TableSessionSnapshot => CreateTableSessionIntent(applicationOutcome),
                SnapshotFormatKind.ScenarioSnapshot => CreateScenarioIntent(applicationOutcome),
                SnapshotFormatKind.ContentPackSnapshot => CreateUnsupportedIntent(applicationOutcome),
                SnapshotFormatKind.ActionLogSnapshot => CreateUnsupportedIntent(applicationOutcome),
                _ => throw new InvalidOperationException("Unsupported snapshot format kind.")
            };
        }

        private static SnapshotImportApplyIntent CreateTableSessionIntent(SnapshotImportApplicationOutcome applicationOutcome)
        {
            if (!applicationOutcome.IsSupported)
            {
                return CreateUnsupportedIntent(applicationOutcome);
            }

            if (applicationOutcome.Payload is not TableSession tableSession)
            {
                throw new InvalidOperationException("Supported TableSessionSnapshot outcome must include a TableSession payload.");
            }

            return new SnapshotImportApplyIntent(
                SnapshotFormatKind.TableSessionSnapshot,
                SnapshotImportApplyOperationKind.ReplaceTableSession,
                true,
                tableSession,
                "TableSessionSnapshot can be applied as ReplaceTableSession.");
        }

        private static SnapshotImportApplyIntent CreateScenarioIntent(SnapshotImportApplicationOutcome applicationOutcome)
        {
            if (!applicationOutcome.IsSupported)
            {
                return CreateUnsupportedIntent(applicationOutcome);
            }

            if (applicationOutcome.Payload is not ScenarioExport scenario)
            {
                throw new InvalidOperationException("Supported ScenarioSnapshot outcome must include a ScenarioExport payload.");
            }

            return new SnapshotImportApplyIntent(
                SnapshotFormatKind.ScenarioSnapshot,
                SnapshotImportApplyOperationKind.CreateScenarioFromImport,
                true,
                scenario,
                "ScenarioSnapshot can be applied as CreateScenarioFromImport.");
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
