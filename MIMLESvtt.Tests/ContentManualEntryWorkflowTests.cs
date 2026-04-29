using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentManualEntryWorkflowTests
{
    [TestMethod]
    public void WorkflowService_ContainsOptionsForMonsterTreasureEquipmentMagicItem()
    {
        var service = new ContentManualEntryWorkflowService();

        var options = service.ListOptions();

        Assert.AreEqual(4, options.Count);
        CollectionAssert.AreEqual(
            new[] { "/content/monsters", "/content/treasure", "/content/equipment", "/content/magic-items" },
            options.Select(a => a.ListRoute).ToArray());
    }

    [TestMethod]
    public void WorkflowOption_ExposesReusableListDetailCreateEditLabels()
    {
        ContentManualEntryWorkflowOption adapter = new(
            "Monsters",
            "/content/monsters",
            "Detail / Review",
            "Create / Edit");

        Assert.AreEqual("Monsters", adapter.ContentTypeName);
        Assert.AreEqual("/content/monsters", adapter.ListRoute);
        Assert.AreEqual("Detail / Review", adapter.DetailLabel);
        Assert.AreEqual("Create / Edit", adapter.EntryLabel);
    }
}
