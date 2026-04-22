using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public interface ITableSessionCommandService
    {
        VttSession? CurrentTableSession { get; }

        ActionRecord ProcessAction(ActionRequest request);
    }
}
