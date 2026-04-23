using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MIMLESvtt.Tests;

[TestClass]
public class WorkspaceBoardPresentationTests
{
    [TestMethod]
    public void WorkspaceMarkup_IncludesBoardOverlayLayersAndHud()
    {
        var root = FindRepoRoot();
        var path = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "Workspace.razor");
        var source = File.ReadAllText(path);

        StringAssert.Contains(source, "board-overlay-stack");
        StringAssert.Contains(source, "overlay-grid");
        StringAssert.Contains(source, "overlay-selection");
        StringAssert.Contains(source, "overlay-markers");
        StringAssert.Contains(source, "board-hud floating");
    }

    [TestMethod]
    public void WorkspaceMarkup_IncludesLegalMoveHintHookAndAccessibilityCues()
    {
        var root = FindRepoRoot();
        var path = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "Workspace.razor");
        var source = File.ReadAllText(path);

        StringAssert.Contains(source, "Show Legal-Move Hints (Hook)");
        StringAssert.Contains(source, "board-legal-move-hints");
        StringAssert.Contains(source, "role=\"application\"");
        StringAssert.Contains(source, "aria-label=\"Interactive board panel\"");
    }

    [TestMethod]
    public void WorkspaceMarkup_IncludesBoardVisibilityFilterControls()
    {
        var root = FindRepoRoot();
        var path = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "Workspace.razor");
        var source = File.ReadAllText(path);

        StringAssert.Contains(source, "show-surfaces-toggle");
        StringAssert.Contains(source, "show-pieces-toggle");
        StringAssert.Contains(source, "show-markers-toggle");
        StringAssert.Contains(source, "active-surface-only-toggle");
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
