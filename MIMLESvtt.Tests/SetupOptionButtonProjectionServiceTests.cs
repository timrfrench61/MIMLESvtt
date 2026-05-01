using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class SetupOptionButtonProjectionServiceTests
{
    private sealed record SourceItem(string Id, string Name, bool Disabled);

    [TestMethod]
    public void Project_ThrowsWhenSourceIsNull()
    {
        List<SourceItem>? source = null;

        Assert.ThrowsException<ArgumentNullException>(() =>
            SetupOptionButtonProjectionService.Project(
                source!,
                item => item.Id,
                item => item.Name));
    }

    [TestMethod]
    public void Project_ThrowsWhenIdSelectorIsNull()
    {
        var source = new List<SourceItem>
        {
            new("A", "Alpha", false)
        };

        Assert.ThrowsException<ArgumentNullException>(() =>
            SetupOptionButtonProjectionService.Project(
                source,
                null!,
                item => item.Name));
    }

    [TestMethod]
    public void Project_ThrowsWhenNameSelectorIsNull()
    {
        var source = new List<SourceItem>
        {
            new("A", "Alpha", false)
        };

        Assert.ThrowsException<ArgumentNullException>(() =>
            SetupOptionButtonProjectionService.Project(
                source,
                item => item.Id,
                null!));
    }

    [TestMethod]
    public void Project_MapsIdNameAndDisabledFlag()
    {
        var source = new List<SourceItem>
        {
            new("A", "Alpha", false),
            new("B", "Beta", true)
        };

        var result = SetupOptionButtonProjectionService.Project(
            source,
            item => item.Id,
            item => item.Name,
            item => item.Disabled);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("A", result[0].Id);
        Assert.AreEqual("Alpha", result[0].Name);
        Assert.IsFalse(result[0].Disabled);
        Assert.AreEqual("B", result[1].Id);
        Assert.AreEqual("Beta", result[1].Name);
        Assert.IsTrue(result[1].Disabled);
    }

    [TestMethod]
    public void Project_DefaultsDisabledToFalse_WhenSelectorNotProvided()
    {
        var source = new List<SourceItem>
        {
            new("A", "Alpha", true)
        };

        var result = SetupOptionButtonProjectionService.Project(
            source,
            item => item.Id,
            item => item.Name);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("A", result[0].Id);
        Assert.AreEqual("Alpha", result[0].Name);
        Assert.IsFalse(result[0].Disabled);
    }
}
