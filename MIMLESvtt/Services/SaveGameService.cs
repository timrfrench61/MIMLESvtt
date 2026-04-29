using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Services;

public class SaveGameService
{
    private readonly VttSessionWorkspaceService _workspaceService;

    public SaveGameService(VttSessionWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
    }

    public void CreateNewSession(string campaignName, string gameboxId)
    {
        _workspaceService.CreateNewCampaignSession(campaignName, gameboxId);
    }

    public void SaveCurrentSession()
    {
        _workspaceService.SaveCurrentSession();
    }
}
