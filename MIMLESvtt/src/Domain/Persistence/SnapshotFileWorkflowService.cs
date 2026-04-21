namespace MIMLESvtt.src
{
    public class SnapshotFileWorkflowService
    {
        private readonly TableSessionFilePersistenceService _tableSessionFilePersistenceService;
        private readonly ScenarioFilePersistenceService _scenarioFilePersistenceService;
        private readonly ContentPackFilePersistenceService _contentPackFilePersistenceService;
        private readonly ActionLogFilePersistenceService _actionLogFilePersistenceService;

        public SnapshotFileWorkflowService()
            : this(
                new TableSessionFilePersistenceService(),
                new ScenarioFilePersistenceService(),
                new ContentPackFilePersistenceService(),
                new ActionLogFilePersistenceService())
        {
        }

        public SnapshotFileWorkflowService(
            TableSessionFilePersistenceService tableSessionFilePersistenceService,
            ScenarioFilePersistenceService scenarioFilePersistenceService,
            ContentPackFilePersistenceService contentPackFilePersistenceService,
            ActionLogFilePersistenceService actionLogFilePersistenceService)
        {
            _tableSessionFilePersistenceService = tableSessionFilePersistenceService ?? throw new ArgumentNullException(nameof(tableSessionFilePersistenceService));
            _scenarioFilePersistenceService = scenarioFilePersistenceService ?? throw new ArgumentNullException(nameof(scenarioFilePersistenceService));
            _contentPackFilePersistenceService = contentPackFilePersistenceService ?? throw new ArgumentNullException(nameof(contentPackFilePersistenceService));
            _actionLogFilePersistenceService = actionLogFilePersistenceService ?? throw new ArgumentNullException(nameof(actionLogFilePersistenceService));
        }

        public void SaveTableSession(TableSession session, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.TableSession, "TableSession");
            _tableSessionFilePersistenceService.SaveToFile(session, path);
        }

        public TableSession LoadTableSession(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.TableSession, "TableSession");
            return _tableSessionFilePersistenceService.LoadFromFile(path);
        }

        public void SaveScenario(ScenarioExport scenario, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.Scenario, "Scenario");
            _scenarioFilePersistenceService.SaveToFile(scenario, path);
        }

        public ScenarioExport LoadScenario(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.Scenario, "Scenario");
            return _scenarioFilePersistenceService.LoadFromFile(path);
        }

        public void SaveContentPack(ContentPackSnapshot contentPack, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.ContentPack, "ContentPack");
            _contentPackFilePersistenceService.SaveToFile(contentPack, path);
        }

        public ContentPackSnapshot LoadContentPack(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.ContentPack, "ContentPack");
            return _contentPackFilePersistenceService.LoadFromFile(path);
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
