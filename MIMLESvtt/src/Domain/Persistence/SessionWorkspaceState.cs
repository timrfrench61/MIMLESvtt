using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class SessionWorkspaceState
    {
        public VttSession? CurrentTableSession { get; internal set; }

        public string? CurrentFilePath { get; internal set; }

        public SnapshotFormatKind? CurrentSnapshotFormat { get; internal set; }

        public bool IsDirty { get; internal set; }

        public string? LastOperationMessage { get; internal set; }

        public WorkspaceMessageSeverity LastOperationSeverity { get; internal set; } = WorkspaceMessageSeverity.Info;

        public PendingScenarioApplicationPlan? CurrentPendingScenarioPlan { get; internal set; }

        public string? PendingScenarioSourcePath { get; internal set; }

        public WorkspaceMode Mode { get; internal set; } = WorkspaceMode.Edit;

        public WorkspaceSettings Settings { get; } = new();

        public List<WorkspaceOperationEntry> OperationHistory { get; } = [];

        public List<WorkspaceUndoEntry> UndoStack { get; } = [];

        public List<WorkspaceUndoEntry> RedoStack { get; } = [];

        public bool CanUndo => UndoStack.Count > 0;

        public bool CanRedo => RedoStack.Count > 0;
    }
}
