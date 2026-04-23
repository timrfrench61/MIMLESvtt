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
}
