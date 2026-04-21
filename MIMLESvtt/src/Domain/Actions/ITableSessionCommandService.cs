namespace MIMLESvtt.src
{
    public interface ITableSessionCommandService
    {
        TableSession? CurrentTableSession { get; }

        ActionRecord ProcessAction(ActionRequest request);
    }
}
