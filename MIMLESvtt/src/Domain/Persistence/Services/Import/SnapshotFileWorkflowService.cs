using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.VttContentPack;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.Services.VttFile;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttContentPackNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class SnapshotFileWorkflowService
    {
        private readonly VttSessionFilePersistenceService _vttSessionFilePersistenceService;
        private readonly VttScenarioFilePersistenceService _scenarioFilePersistenceService;
        private readonly VttContentPackFilePersistenceService _vttContentPackFilePersistenceService;
        private readonly ActionLogFilePersistenceService _actionLogFilePersistenceService;

        public SnapshotFileWorkflowService()
            : this(
                new VttSessionFilePersistenceService(),
                new VttScenarioFilePersistenceService(),
                new VttContentPackFilePersistenceService(),
                new ActionLogFilePersistenceService())
        {
        }

        public SnapshotFileWorkflowService(
            VttSessionFilePersistenceService vttSessionFilePersistenceService,
            VttScenarioFilePersistenceService scenarioFilePersistenceService,
            VttContentPackFilePersistenceService vttContentPackFilePersistenceService,
            ActionLogFilePersistenceService actionLogFilePersistenceService)
        {
            _vttSessionFilePersistenceService = vttSessionFilePersistenceService ?? throw new ArgumentNullException(nameof(vttSessionFilePersistenceService));
            _scenarioFilePersistenceService = scenarioFilePersistenceService ?? throw new ArgumentNullException(nameof(scenarioFilePersistenceService));
            _vttContentPackFilePersistenceService = vttContentPackFilePersistenceService ?? throw new ArgumentNullException(nameof(vttContentPackFilePersistenceService));
            _actionLogFilePersistenceService = actionLogFilePersistenceService ?? throw new ArgumentNullException(nameof(actionLogFilePersistenceService));
        }

        public void SaveVttSession(VttSession session, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttSession, "vttSession");
            _vttSessionFilePersistenceService.SaveToFile(session, path);
        }

        public VttSession LoadVttSession(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttSession, "vttSession");
            return _vttSessionFilePersistenceService.LoadFromFile(path);
        }

        public void SaveScenario(VttScenario scenario, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttScenario, "Scenario");
            _scenarioFilePersistenceService.SaveToFile(scenario, path);
        }

        public VttScenario LoadScenario(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttScenario, "Scenario");
            return _scenarioFilePersistenceService.LoadFromFile(path);
        }

        public void SaveVttContentPack(VttContentPack contentPack, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttContentPack, "VttContentPack");
            _vttContentPackFilePersistenceService.SaveToFile(contentPack, path);
        }

        public void SaveVttContentPackSnapshot(VttContentPackSnapshot contentPack, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttContentPack, "VttContentPack");
            _vttContentPackFilePersistenceService.SaveToFile(contentPack, path);
        }

        public VttContentPack LoadVttContentPack(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttContentPack, "VttContentPack");
            return _vttContentPackFilePersistenceService.LoadModelFromFile(path);
        }

        public VttContentPackSnapshot LoadVttContentPackSnapshot(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttContentPack, "VttContentPack");
            return _vttContentPackFilePersistenceService.LoadFromFile(path);
        }

        public void SaveActionLog(ActionLogSnapshot actionLog, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.ActionLog, "ActionLog");
            _actionLogFilePersistenceService.SaveToFile(actionLog, path);
        }

        public ActionLogSnapshot LoadActionLog(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.ActionLog, "ActionLog");
            return _actionLogFilePersistenceService.LoadFromFile(path);
        }

        private static void ValidatePathExtension(string path, string expectedExtension, string snapshotType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!HasExpectedExtension(path, expectedExtension))
            {
                throw new ArgumentException($"{snapshotType} file path must end with '{expectedExtension}'.", nameof(path));
            }
        }

        private static bool HasExpectedExtension(string path, string expectedExtension)
        {
            return path.EndsWith(expectedExtension, StringComparison.OrdinalIgnoreCase);
        }
    }
}
