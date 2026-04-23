using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentManagementUiBaselineTests
{
    [TestMethod]
    public void ContentRoutes_AreDeclaredForRouteGroupPages()
    {
        AssertRoute<ContentHome>("/content");
        AssertRoute<ContentMonsters>("/content/monsters");
        AssertRoute<ContentTreasure>("/content/treasure");
        AssertRoute<ContentEquipment>("/content/equipment");
        AssertRoute<ContentMagicItems>("/content/magic-items");
    }

    [TestMethod]
    public void ContentHome_RendersExpectedSectionShellReferences()
    {
        var root = FindRepoRoot();
        var path = Path.Combine(root, "MIMLESvtt", "Components", "Pages", "ContentHome.razor");
        var source = File.ReadAllText(path);

        StringAssert.Contains(source, "<ContentImportWorkflowShell />");
        StringAssert.Contains(source, "<ContentStatusStateShell");
        StringAssert.Contains(source, "/content/monsters");
        StringAssert.Contains(source, "/content/treasure");
        StringAssert.Contains(source, "/content/equipment");
        StringAssert.Contains(source, "/content/magic-items");
    }

    [TestMethod]
    public void ContentEntityPages_RenderSharedEntityShellComponent()
    {
        var root = FindRepoRoot();
        var pagesDirectory = Path.Combine(root, "MIMLESvtt", "Components", "Pages");

        var monsters = File.ReadAllText(Path.Combine(pagesDirectory, "ContentMonsters.razor"));
        var treasure = File.ReadAllText(Path.Combine(pagesDirectory, "ContentTreasure.razor"));
        var equipment = File.ReadAllText(Path.Combine(pagesDirectory, "ContentEquipment.razor"));
        var magicItems = File.ReadAllText(Path.Combine(pagesDirectory, "ContentMagicItems.razor"));

        StringAssert.Contains(monsters, "<ContentEntityScreenShell");
        StringAssert.Contains(treasure, "<ContentEntityScreenShell");
        StringAssert.Contains(equipment, "<ContentEntityScreenShell");
        StringAssert.Contains(magicItems, "<ContentEntityScreenShell");
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
