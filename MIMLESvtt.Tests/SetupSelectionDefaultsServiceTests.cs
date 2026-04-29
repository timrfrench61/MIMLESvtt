using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class SetupSelectionDefaultsServiceTests
{
    [TestMethod]
    public void ResolveSelectedId_ReturnsNull_WhenNoIds()
    {
        var result = SetupSelectionDefaultsService.ResolveSelectedId("ANY", []);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ResolveSelectedId_ReturnsCurrent_WhenPresent()
    {
        var result = SetupSelectionDefaultsService.ResolveSelectedId("B", ["A", "B", "C"]);
        Assert.AreEqual("B", result);
    }

    [TestMethod]
    public void ResolveSelectedId_ReturnsFirst_WhenCurrentMissing()
    {
        var result = SetupSelectionDefaultsService.ResolveSelectedId("Z", ["A", "B", "C"]);
        Assert.AreEqual("A", result);
    }
}
