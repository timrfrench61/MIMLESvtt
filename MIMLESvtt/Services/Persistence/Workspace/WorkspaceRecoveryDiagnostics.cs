namespace MIMLESvtt.src.Domain.Persistence.Workspace
{
    public class WorkspaceRecoveryDiagnostics
    {
        public WorkspaceRecoverySource SourceUsed { get; set; } = WorkspaceRecoverySource.None;

        public bool IsSuccess { get; set; }

        public bool MainFileAttempted { get; set; }

        public bool BackupAttempted { get; set; }

        public bool MainFileValid { get; set; }

        public bool BackupValid { get; set; }

        public bool CurrentVttSessionRestored { get; set; }

        public bool PendingVttScenarioRestored { get; set; }

        public List<string> Warnings { get; } = [];

        public List<string> Errors { get; } = [];
    }
}
