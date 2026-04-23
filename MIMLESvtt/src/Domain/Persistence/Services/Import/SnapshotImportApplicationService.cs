using MIMLESvtt.src.Domain.Persistence.Model;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.Snapshot;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class SnapshotImportApplicationService
    {
        public SnapshotImportApplicationOutcome Apply(SnapshotImportResult importResult)
        {
            ArgumentNullException.ThrowIfNull(importResult);

            return importResult.FormatKind switch
            {
                SnapshotFormatKind.VttSessionSnapshot => ApplyVttSession(importResult.Payload),
                SnapshotFormatKind.VttScenarioSnapshot => ApplyScenario(importResult.Payload),
                SnapshotFormatKind.VttGameboxSnapshot => new SnapshotImportApplicationOutcome(
                    SnapshotFormatKind.VttGameboxSnapshot,
                    false,
                    null,
                    "VttGameboxSnapshot is not supported for application in this pass."),
                SnapshotFormatKind.ActionLogSnapshot => new SnapshotImportApplicationOutcome(
                    SnapshotFormatKind.ActionLogSnapshot,
                    false,
                    null,
                    "ActionLogSnapshot is not supported for application in this pass."),
                _ => throw new InvalidOperationException("Unsupported snapshot format kind.")
            };
        }

        private static SnapshotImportApplicationOutcome ApplyVttSession(object payload)
        {
            if (payload is not VttSessionSnapshot VttSessionSnapshot || VttSessionSnapshot.VttSession is null)
            {
                throw new InvalidOperationException("SnapshotImportResult payload does not match VttSessionSnapshot.");
            }

            return new SnapshotImportApplicationOutcome(
                SnapshotFormatKind.VttSessionSnapshot,
                true,
                VttSessionSnapshot.VttSession,
                "VttSessionSnapshot is supported for application.");
        }

        private static SnapshotImportApplicationOutcome ApplyScenario(object payload)
        {
            if (payload is not VttScenarioSnapshot scenarioSnapshot || scenarioSnapshot.VttScenario is null)
            {
                throw new InvalidOperationException("SnapshotImportResult payload does not match ScenarioSnapshot.");
            }

            return new SnapshotImportApplicationOutcome(
                SnapshotFormatKind.VttScenarioSnapshot,
                true,
                scenarioSnapshot.VttScenario,
                "VttScenarioSnapshot is supported for application.");
        }
    }
}
