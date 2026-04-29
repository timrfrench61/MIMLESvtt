using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Services;

public class ModuleImportService
{
    private readonly VttSessionWorkspaceService _workspaceService;

    public ModuleImportService(VttSessionWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
    }

    public IReadOnlyList<string> ListAvailableGameSystems()
    {
        return _workspaceService.ListAvailableGameboxIds();
    }
}
