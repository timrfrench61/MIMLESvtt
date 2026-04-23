using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.VttGamebox;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.Services.VttFile;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttGameboxNSPC;
namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class SnapshotFileWorkflowService
    {
        private readonly VttSessionFilePersistenceService _vttSessionFilePersistenceService;
        private readonly VttScenarioFilePersistenceService _scenarioFilePersistenceService;
        private readonly VttGameboxFilePersistenceService _vttGameboxFilePersistenceService;
        private readonly ActionLogFilePersistenceService _actionLogFilePersistenceService;

        public SnapshotFileWorkflowService()
            : this(
                new VttSessionFilePersistenceService(),
                new VttScenarioFilePersistenceService(),
                new VttGameboxFilePersistenceService(),
                new ActionLogFilePersistenceService())
        {
        }

        public SnapshotFileWorkflowService(
            VttSessionFilePersistenceService vttSessionFilePersistenceService,
            VttScenarioFilePersistenceService vttScenarioFilePersistenceService,
            VttGameboxFilePersistenceService vttGameboxFilePersistenceService,
            ActionLogFilePersistenceService actionLogFilePersistenceService)
        {
            _vttSessionFilePersistenceService = vttSessionFilePersistenceService ?? throw new ArgumentNullException(nameof(vttSessionFilePersistenceService));
            _scenarioFilePersistenceService = vttScenarioFilePersistenceService ?? throw new ArgumentNullException(nameof(vttScenarioFilePersistenceService));
            _vttGameboxFilePersistenceService = vttGameboxFilePersistenceService ?? throw new ArgumentNullException(nameof(vttGameboxFilePersistenceService));
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

        public void SaveVttGamebox(VttGamebox gamebox, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttGamebox, "VttGamebox");
            _vttGameboxFilePersistenceService.SaveToFile(gamebox, path);
        }

        public void SaveVttGameboxSnapshot(VttGameboxSnapshot gamebox, string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttGamebox, "VttGamebox");
            _vttGameboxFilePersistenceService.SaveToFile(gamebox, path);
        }

        public VttGamebox LoadVttGamebox(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttGamebox, "VttGamebox");
            return _vttGameboxFilePersistenceService.LoadModelFromFile(path);
        }

        public VttGameboxSnapshot LoadVttGameboxSnapshot(string path)
        {
            ValidatePathExtension(path, SnapshotFileExtensions.VttGamebox, "VttGamebox");
            return _vttGameboxFilePersistenceService.LoadFromFile(path);
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
