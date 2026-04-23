using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;

namespace MIMLESvtt.Tests;

[TestClass]
public class CheckersReferenceUiFlowTests
{
    [TestMethod]
    public void CheckersReferenceRoute_IsDeclared()
    {
        var route = typeof(CheckersReference)
            .GetCustomAttributes<RouteAttribute>(inherit: true)
            .Select(a => a.Template)
            .FirstOrDefault();

        Assert.AreEqual("/workspace/checkers-reference", route);
    }

    [TestMethod]
    public void CheckersReferenceMarkup_ContainsWorkspaceBootstrapLink()
    {
        var root = FindRepoRoot();
        var path = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "CheckersReference.razor");
        var source = File.ReadAllText(path);

        StringAssert.Contains(source, "/workspace?mode=checkers");
    }

    [TestMethod]
    public void WorkspaceMarkup_ContainsCheckersTurnIndicatorAndTerminalMessageSlot()
    {
        var root = FindRepoRoot();
        var path = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "Workspace.razor");
        var source = File.ReadAllText(path);

        StringAssert.Contains(source, "Checkers Reference Mode");
        StringAssert.Contains(source, "Turn indicator:");
        StringAssert.Contains(source, "Checkers terminal-state slot");
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
