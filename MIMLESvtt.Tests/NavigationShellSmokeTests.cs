using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;

namespace MIMLESvtt.Tests;

[TestClass]
public class NavigationShellSmokeTests
{
    [TestMethod]
    public void PrimaryNavigationRoutes_AreDeclared()
    {
        AssertRoute<Home>("/");
        AssertRoute<Workspace>("/workspace");
        AssertRoute<TacticalScenarioSetup>("/workspace/tactical-setup");
        AssertRoute<ContentHome>("/content");
        AssertRoute<ContentImport>("/content/import");
        AssertRoute<Login>("/login");
        AssertRoute<Logout>("/logout");
    }

    [TestMethod]
    public void NavMenu_Hrefs_MatchRouteDeclarations()
    {
        var root = FindRepoRoot();
        var navPath = Path.Combine(root, "MIMLESvtt", "Components", "Layout", "NavMenu.razor");
        var navMarkup = File.ReadAllText(navPath);

        StringAssert.Contains(navMarkup, "href=\"\"");
        StringAssert.Contains(navMarkup, "href=\"workspace\"");
        StringAssert.Contains(navMarkup, "href=\"workspace/tactical-setup\"");
        StringAssert.Contains(navMarkup, "href=\"content\"");
        StringAssert.Contains(navMarkup, "href=\"content/import\"");

        AssertRoute<ContentImport>("/content/import");
        AssertRoute<ContentHome>("/content");
        AssertRoute<Workspace>("/workspace");
    }

    private static void AssertRoute<TComponent>(string expectedRoute)
    {
        var route = typeof(TComponent)
            .GetCustomAttributes<RouteAttribute>(inherit: true)
            .Select(a => a.Template)
            .FirstOrDefault();

        Assert.AreEqual(expectedRoute, route, $"Route mismatch for {typeof(TComponent).Name}.");
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
