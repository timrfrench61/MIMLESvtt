using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Rules;

namespace MIMLESvtt.Tests;

[TestClass]
public class DiceRandomizationServiceTests
{
    [TestMethod]
    public void RollD20_ReturnsStructuredResultWithExpectedBounds()
    {
        var service = new DiceRandomizationService();

        var result = service.RollD20(count: 3, modifier: 2);

        Assert.AreEqual(20, result.Sides);
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(3, result.Rolls.Count);
        Assert.AreEqual(2, result.Modifier);
        Assert.IsTrue(result.Rolls.All(v => v >= 1 && v <= 20));
        Assert.AreEqual(result.Rolls.Sum() + 2, result.Total);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.RandomProvider));
        Assert.IsTrue(result.TimestampUtc != default);
    }

    [TestMethod]
    public void RollD100_ReturnsValuesWithinPercentileRange()
    {
        var service = new DiceRandomizationService();

        var result = service.RollD100(count: 5);

        Assert.AreEqual(100, result.Sides);
        Assert.AreEqual(5, result.Count);
        Assert.IsTrue(result.Rolls.All(v => v >= 1 && v <= 100));
    }

    [TestMethod]
    public void RollDie_WithInvalidSides_FailsClearly()
    {
        var service = new DiceRandomizationService();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => service.RollDie(1));
    }

    [TestMethod]
    public void RollDie_WithInvalidCount_FailsClearly()
    {
        var service = new DiceRandomizationService();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => service.RollDie(6, count: 0));
    }

    [TestMethod]
    public void RollDie_UsesDeterministicProvider_ForReproducibleResults()
    {
        var provider = new DeterministicDiceRandomProvider([3, 5, 2]);
        var service = new DiceRandomizationService(provider);

        var result = service.RollDie(6, count: 3, modifier: 1, contextTag: "test:deterministic");

        CollectionAssert.AreEqual(new[] { 3, 5, 2 }, result.Rolls);
        Assert.AreEqual(11, result.Total);
        Assert.AreEqual("Deterministic", result.RandomProvider);
        Assert.AreEqual("test:deterministic", result.ContextTag);
    }

    [TestMethod]
    public void RollDie_AddsDiagnosticsTimestampAndContext()
    {
        var provider = new DeterministicDiceRandomProvider([4]);
        var service = new DiceRandomizationService(provider);

        var before = DateTime.UtcNow;
        var result = service.RollD6(contextTag: "rules:diag");
        var after = DateTime.UtcNow;

        Assert.IsTrue(result.TimestampUtc >= before && result.TimestampUtc <= after);
        Assert.AreEqual("rules:diag", result.ContextTag);
    }

    [TestMethod]
    public void RollDie_ExtremeCountAndModifier_ComputesTotalCorrectly()
    {
        var provider = new DeterministicDiceRandomProvider(Enumerable.Repeat(1, 100));
        var service = new DiceRandomizationService(provider);

        var result = service.RollDie(6, count: 100, modifier: 500, contextTag: "test:extreme");

        Assert.AreEqual(100, result.Rolls.Count);
        Assert.AreEqual(600, result.Total);
    }

    [TestMethod]
    public void RulesModuleStubs_ReuseSharedDiceService()
    {
        var provider = new DeterministicDiceRandomProvider([4, 88]);
        IDiceRandomizationService sharedService = new DiceRandomizationService(provider);

        var checkers = new CheckersRulesModuleStub(sharedService);
        var brp = new BrpRulesModuleStub(sharedService);

        var checkersValue = checkers.RollInitiativeLikeValue();
        var brpValue = brp.RollPercentileSkillCheck();

        Assert.AreEqual(4, checkersValue);
        Assert.AreEqual(88, brpValue);
    }
}
