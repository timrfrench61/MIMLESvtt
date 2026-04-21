namespace MIMLESvtt.src
{
    public enum WorkspaceOperationKind
    {
        CreateNewSession,
        AddSurface,
        AddPiece,
        OpenTableSessionFromFile,
        SaveCurrentSession,
        SaveCurrentSessionAs,
        SaveCurrentLayoutAsScenario,
        ImportScenarioToPendingPlanFromFile,
        ActivatePendingScenario,
        SaveWorkspaceState,
        RestoreWorkspaceState,
        InitializeTurnOrder,
        AdvanceTurn,
        SetPhase,
        AddParticipant,
        RemoveParticipant,
        SetWorkspaceMode,
        ProcessAction,
        UndoLastOperation,
        RedoLastOperation
    }
}
