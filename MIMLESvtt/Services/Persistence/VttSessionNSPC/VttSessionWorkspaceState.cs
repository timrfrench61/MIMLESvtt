using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Scenario;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.Workspace;

namespace MIMLESvtt.src.Domain.Persistence.VttSessionNSPC
{
    public class VttSessionWorkspaceState
    {
        public VttSession? CurrentVttSession { get; internal set; }

        public string? CurrentFilePath { get; internal set; }

        public SnapshotFormatKind? CurrentSnapshotFormat { get; internal set; }

        public bool IsDirty { get; internal set; }

        public string? LastOperationMessage { get; internal set; }

        public WorkspaceMessageSeverity LastOperationSeverity { get; internal set; } = WorkspaceMessageSeverity.Info;

        public VttScenarioPendingApplicationPlan? CurrentPendingVttScenarioPlan { get; internal set; }

        public string? PendingVttScenarioSourcePath { get; internal set; }

        public WorkspaceMode Mode { get; internal set; } = WorkspaceMode.Edit;

        public bool CanCreateSession { get; internal set; }

        public WorkspaceSettings Settings { get; } = new();

        public List<WorkspaceOperationEntry> OperationHistory { get; } = [];

        public List<WorkspaceUndoEntry> UndoStack { get; } = [];

        public List<WorkspaceUndoEntry> RedoStack { get; } = [];

        public bool CanUndo => UndoStack.Count > 0;

        public bool CanRedo => RedoStack.Count > 0;
    }
}
