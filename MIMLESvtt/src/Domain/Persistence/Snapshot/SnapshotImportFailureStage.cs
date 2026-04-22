namespace MIMLESvtt.src.Domain.Persistence.Snapshot
{
    public enum SnapshotImportFailureStage
    {
        None,
        FacadeInput,
        Dispatch,
        FormatValidation,
        ApplicationMapping,
        Unexpected
    }
}
