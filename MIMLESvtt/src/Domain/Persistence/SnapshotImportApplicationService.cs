namespace MIMLESvtt.src
{
    public class SnapshotImportApplicationService
    {
        public SnapshotImportApplicationOutcome Apply(SnapshotImportResult importResult)
        {
            ArgumentNullException.ThrowIfNull(importResult);

            return importResult.FormatKind switch
            {
                SnapshotFormatKind.TableSessionSnapshot => ApplyTableSession(importResult.Payload),
                SnapshotFormatKind.ScenarioSnapshot => ApplyScenario(importResult.Payload),
                SnapshotFormatKind.VttContentPackSnapshot => new SnapshotImportApplicationOutcome(
                    SnapshotFormatKind.VttContentPackSnapshot,
                    false,
                    null,
                    "VttContentPackSnapshot is not supported for application in this pass."),
                SnapshotFormatKind.ActionLogSnapshot => new SnapshotImportApplicationOutcome(
                    SnapshotFormatKind.ActionLogSnapshot,
                    false,
                    null,
                    "ActionLogSnapshot is not supported for application in this pass."),
                _ => throw new InvalidOperationException("Unsupported snapshot format kind.")
            };
        }

        private static SnapshotImportApplicationOutcome ApplyTableSession(object payload)
        {
            if (payload is not TableSessionSnapshot tableSessionSnapshot || tableSessionSnapshot.TableSession is null)
            {
                throw new InvalidOperationException("SnapshotImportResult payload does not match TableSessionSnapshot.");
            }

            return new SnapshotImportApplicationOutcome(
                SnapshotFormatKind.TableSessionSnapshot,
                true,
                tableSessionSnapshot.TableSession,
                "TableSessionSnapshot is supported for application.");
        }

        private static SnapshotImportApplicationOutcome ApplyScenario(object payload)
        {
            if (payload is not ScenarioSnapshot scenarioSnapshot || scenarioSnapshot.Scenario is null)
            {
                throw new InvalidOperationException("SnapshotImportResult payload does not match ScenarioSnapshot.");
            }

            return new SnapshotImportApplicationOutcome(
                SnapshotFormatKind.ScenarioSnapshot,
                true,
                scenarioSnapshot.Scenario,
                "ScenarioSnapshot is supported for application.");
        }
    }
}
