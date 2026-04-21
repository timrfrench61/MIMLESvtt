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
        ImportScenarioToPendingPlanFromFile,
        ActivatePendingScenario,
        SaveWorkspaceState,
        RestoreWorkspaceState,
        ProcessAction
    }
}
