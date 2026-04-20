namespace MIMLESvtt.src
{
    public class SnapshotImportWorkflowService
    {
        private readonly SnapshotImportService _importService;
        private readonly SnapshotImportApplicationService _applicationService;

        public SnapshotImportWorkflowService()
            : this(new SnapshotImportService(), new SnapshotImportApplicationService())
        {
        }

        public SnapshotImportWorkflowService(
            SnapshotImportService importService,
            SnapshotImportApplicationService applicationService)
        {
            _importService = importService ?? throw new ArgumentNullException(nameof(importService));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }

        public SnapshotImportApplicationOutcome Import(string json)
        {
            var importResult = _importService.Import(json);
            return _applicationService.Apply(importResult);
        }
    }
}
