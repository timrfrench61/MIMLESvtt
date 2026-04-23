using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models.Testing;

namespace MIMLESvtt.Tests;

[TestClass]
public class SubsystemTestBoundaryCatalogTests
{
    [TestMethod]
    public void OwnershipMatrix_ContainsAllExpectedSubsystems()
    {
        var matrix = SubsystemTestBoundaryCatalog.GetOwnershipMatrix();

        CollectionAssert.AreEquivalent(
            new[]
            {
                TestSubsystem.Engine,
                TestSubsystem.RulesFramework,
                TestSubsystem.UiPresentation,
                TestSubsystem.PersistenceImport,
                TestSubsystem.Networking
            },
            matrix.Select(m => m.Subsystem).ToArray());
    }

    [TestMethod]
    public void IntegrationSlices_ContainRequiredCrossSubsystemDefinitions()
    {
        var slices = SubsystemTestBoundaryCatalog.GetIntegrationSlices();

        Assert.IsTrue(slices.Any(s => s.Id == "INT-ENG-PERSIST"));
        Assert.IsTrue(slices.Any(s => s.Id == "INT-ENG-IMPORT"));
        Assert.IsTrue(slices.Any(s => s.Id == "INT-ENG-RULES"));
        Assert.IsTrue(slices.Any(s => s.Id == "INT-UI-WORKSPACE"));
    }

    [TestMethod]
    public void SeparationRules_AreDefinedAsExpected()
    {
        var rules = SubsystemTestBoundaryCatalog.GetSeparationRules();

        Assert.AreEqual(3, rules.Count);
        Assert.IsTrue(rules.Any(r => r.Contains("Engine tests remain separate from rules-module tests.", StringComparison.Ordinal)));
        Assert.IsTrue(rules.Any(r => r.Contains("UI tests remain separate from domain logic tests.", StringComparison.Ordinal)));
        Assert.IsTrue(rules.Any(r => r.Contains("Integration tests verify contract handoffs", StringComparison.Ordinal)));
    }
}
