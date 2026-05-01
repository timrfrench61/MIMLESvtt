using MIMLESvtt.src;
using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.Services.Actions;

public interface ISessionCommandService
{
    VttSession? CurrentVttSession { get; }

    ActionRecord ProcessAction(ActionRequest request);
}
