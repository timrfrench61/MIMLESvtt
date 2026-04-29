using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Services;

public sealed record LoadGameSessionOption(string FilePath, string FileName, string JoinCode);

public class LoadGameService
{
    private readonly VttSessionWorkspaceService _workspaceService;

    public LoadGameService(VttSessionWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
    }

    public IReadOnlyList<LoadGameSessionOption> ListKnownSessions()
    {
        return _workspaceService.ListKnownGameSessions()
            .Where(s => s.Exists && !string.IsNullOrWhiteSpace(s.FilePath))
            .Select(s => new LoadGameSessionOption(s.FilePath, s.FileName, s.JoinCode))
            .OrderBy(s => s.FileName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void OpenSession(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _workspaceService.OpenVttSessionFromFile(filePath);
    }
}
