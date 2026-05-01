using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public interface IVttSessionCommandService
    {
        VttSession? CurrentVttSession { get; }

        ActionRecord ProcessAction(ActionRequest request);
    }
}
