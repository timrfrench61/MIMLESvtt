using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class SetupRefreshWorkflowServiceTests
{
    [TestMethod]
    public void BuildSelection_UsesMappedOptions_AndPreservesCurrentWhenPresent()
    {
        var result = SetupRefreshWorkflowService.BuildSelection(
            ["A", "B"],
            items => items.Select(id => new TestOption(id)).ToList(),
            option => option.Id,
            "B");

        Assert.AreEqual(2, result.Options.Count);
        Assert.AreEqual("B", result.SelectedId);
    }

    [TestMethod]
    public void BuildSelection_UsesFallbackFactory_WhenMappedOptionsEmpty()
    {
        var result = SetupRefreshWorkflowService.BuildSelection<string, TestOption>(
            [],
            _ => [],
            option => option.Id,
            currentSelectedId: null,
            fallbackOptionsFactory: () => [new TestOption("FALLBACK")]);

        Assert.AreEqual(1, result.Options.Count);
        Assert.AreEqual("FALLBACK", result.Options[0].Id);
        Assert.AreEqual("FALLBACK", result.SelectedId);
    }

    [TestMethod]
    public void BuildSelection_ReturnsNullSelectedId_WhenNoOptionsAndNoFallback()
    {
        var result = SetupRefreshWorkflowService.BuildSelection<string, TestOption>(
            [],
            _ => [],
            option => option.Id,
            currentSelectedId: "X");

        Assert.AreEqual(0, result.Options.Count);
        Assert.IsNull(result.SelectedId);
    }

    private sealed record TestOption(string Id);
}
