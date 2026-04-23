using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Application.Content.ManualEntry;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentManualEntryWorkflowTests
{
    [TestMethod]
    public void WorkflowRegistry_ContainsAdaptersForMonsterTreasureEquipmentMagicItem()
    {
        var registry = new ContentManualEntryWorkflowRegistry();

        var adapters = registry.ListAdapters();

        Assert.AreEqual(4, adapters.Count);
        CollectionAssert.AreEqual(
            new[] { "/content/monsters", "/content/treasure", "/content/equipment", "/content/magic-items" },
            adapters.Select(a => a.ListRoute).ToArray());
    }

    [TestMethod]
    public void WorkflowAdapter_ExposesReusableListDetailCreateEditLabels()
    {
        IContentManualEntryWorkflowAdapter adapter = new ContentManualEntryWorkflowAdapter
        {
            ContentTypeName = "Monsters",
            ListRoute = "/content/monsters",
            DetailLabel = "Detail / Review",
            EntryLabel = "Create / Edit"
        };

        Assert.AreEqual("Monsters", adapter.ContentTypeName);
        Assert.AreEqual("/content/monsters", adapter.ListRoute);
        Assert.AreEqual("Detail / Review", adapter.DetailLabel);
        Assert.AreEqual("Create / Edit", adapter.EntryLabel);
    }
}
