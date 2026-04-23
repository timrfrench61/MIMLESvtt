using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MIMLESvtt.Tests;

[TestClass]
public class PieceVisualStatePresentationTests
{
    [TestMethod]
    public void WorkspaceMarkup_ContainsSelectedAndPrimaryPieceVisualCues()
    {
        var root = FindRepoRoot();
        var path = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "Workspace.razor.css");
        var css = File.ReadAllText(path);

        StringAssert.Contains(css, ".board-piece.selected");
        StringAssert.Contains(css, ".board-piece.primary");
    }

    [TestMethod]
    public void WorkspaceMarkup_ContainsMarkerVisibilityWarningAndOverlayBadges()
    {
        var root = FindRepoRoot();
        var markupPath = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "Workspace.razor");
        var cssPath = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "Workspace.razor.css");

        var markup = File.ReadAllText(markupPath);
        var css = File.ReadAllText(cssPath);

        StringAssert.Contains(markup, "Marker cues are currently hidden by the Show Markers toggle.");
        StringAssert.Contains(markup, "piece-overlay-badge");
        StringAssert.Contains(css, ".piece-overlay-badge");
    }

    [TestMethod]
    public void WorkspaceMarkup_ContainsPlannedStateTokensForDisabledEliteLeaderPromoted()
    {
        var root = FindRepoRoot();
        var cssPath = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "Workspace.razor.css");
        var css = File.ReadAllText(cssPath);

        StringAssert.Contains(css, ".board-piece.state-disabled");
        StringAssert.Contains(css, ".board-piece.state-elite");
        StringAssert.Contains(css, ".board-piece.state-leader");
        StringAssert.Contains(css, ".board-piece.state-promoted");
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "MIMLESvtt.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Unable to locate repository root containing MIMLESvtt.sln.");
    }
}
