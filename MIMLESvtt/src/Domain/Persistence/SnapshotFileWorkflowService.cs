using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class SnapshotFileWorkflowService
    {
        private readonly TableSessionFilePersistenceService _tableSessionFilePersistenceService;
        private readonly ScenarioFilePersistenceService _scenarioFilePersistenceService;
        private readonly VttContentPackFilePersistenceService _vttContentPackFilePersistenceService;
        private readonly ActionLogFilePersistenceService _actionLogFilePersistenceService;

        public SnapshotFileWorkflowService()
            : this(
                new TableSessionFilePersistenceService(),
                new ScenarioFilePersistenceService(),
                new VttContentPackFilePersistenceService(),
                new ActionLogFilePersistenceService())
        {
        }

        public SnapshotFileWorkflowService(
            TableSessionFilePersistenceService tableSessionFilePersistenceService,
            ScenarioFilePersistenceService scenarioFilePersistenceService,
            VttContentPackFilePersistenceService vttContentPackFilePersistenceService,
            ActionLogFilePersistenceService actionLogFilePersistenceService)
        {
            _tableSessionFilePersistenceService = tableSessionFilePersistenceService ?? throw new ArgumentNullException(nameof(tableSessionFilePersistenceService));
            _scenarioFilePersistenceService = scenarioFilePersistenceService ?? throw new ArgumentNullException(nameof(scenarioFilePersistenceService));
            _vttContentPackFilePersistenceService = vttContentPackFilePersistenceService ?? throw new ArgumentNullException(nameof(vttContentPackFilePersistenceService));
            _actionLogFilePersistenceService = actionLogFilePersistenceService ?? throw new ArgumentNullException(nameof(actionLogFilePersistenceService));
        }

        public void SaveTableSession(VttSession session, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.TableSession, "TableSession");
            _tableSessionFilePersistenceService.SaveToFile(session, path);
        }

        public VttSession LoadTableSession(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.TableSession, "TableSession");
            return _tableSessionFilePersistenceService.LoadFromFile(path);
        }

        public void SaveScenario(Scenario scenario, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.Scenario, "Scenario");
            _scenarioFilePersistenceService.SaveToFile(scenario, path);
        }

        public Scenario LoadScenario(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.Scenario, "Scenario");
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
