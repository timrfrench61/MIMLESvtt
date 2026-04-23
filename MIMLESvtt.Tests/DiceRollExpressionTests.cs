using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Rules;

namespace MIMLESvtt.Tests;

[TestClass]
public class DiceRollExpressionTests
{
    [TestMethod]
    public void RollExpression_WithSimpleExpression_ReturnsParsedResult()
    {
        var service = new DiceRandomizationService();

        var result = service.RollExpression("2d6");

        Assert.AreEqual("2d6", result.Expression);
        Assert.AreEqual(2, result.DiceCount);
        Assert.AreEqual(6, result.Sides);
        Assert.AreEqual(0, result.Modifier);
        Assert.AreEqual(2, result.RollResult.Count);
        Assert.IsTrue(result.RollResult.Rolls.All(v => v >= 1 && v <= 6));
        Assert.AreEqual(RollExpressionParseErrorCode.None, result.ParseErrorCode);
    }

    [TestMethod]
    public void RollExpression_WithWhitespace_IsParsedSuccessfully()
    {
        var provider = new DeterministicDiceRandomProvider([2, 4]);
        var service = new DiceRandomizationService(provider);

        var result = service.RollExpression(" 2 d 6 + 1 ");

        Assert.AreEqual("2d6+1", result.Expression);
        Assert.AreEqual(2, result.DiceCount);
        Assert.AreEqual(6, result.Sides);
        Assert.AreEqual(1, result.Modifier);
        Assert.AreEqual(7, result.RollResult.Total);
    }

    [TestMethod]
    public void RollExpression_WithPositiveModifier_ReturnsModifierInResult()
    {
        var service = new DiceRandomizationService();

        var result = service.RollExpression("1d20+4");

        Assert.AreEqual(1, result.DiceCount);
        Assert.AreEqual(20, result.Sides);
        Assert.AreEqual(4, result.Modifier);
        Assert.AreEqual(result.RollResult.Rolls.Sum() + 4, result.RollResult.Total);
    }

    [TestMethod]
    public void RollExpression_WithNegativeModifier_ReturnsModifierInResult()
    {
        var service = new DiceRandomizationService();

        var result = service.RollExpression("3d8-2");

        Assert.AreEqual(3, result.DiceCount);
        Assert.AreEqual(8, result.Sides);
        Assert.AreEqual(-2, result.Modifier);
        Assert.AreEqual(result.RollResult.Rolls.Sum() - 2, result.RollResult.Total);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("d20")]
    [DataRow("2d")]
    [DataRow("2d1")]
    [DataRow("abc")]
    [DataRow("2d6+")]
    [DataRow("2d6-+")]
    public void RollExpression_WithInvalidFormat_FailsClearly(string expression)
    {
        var service = new DiceRandomizationService();

        Assert.ThrowsException<ArgumentException>(() => service.RollExpression(expression));
    }

    [TestMethod]
    public void BasicRollExpressionParser_ReturnsSpecificErrorCode_ForMissingDiceDelimiter()
    {
        var parser = new BasicRollExpressionParser();

        var result = parser.Parse("2x6");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(RollExpressionParseErrorCode.MissingDiceDelimiter, result.ErrorCode);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [TestMethod]
    public void BasicRollExpressionParser_ReturnsSpecificErrorCode_ForInvalidModifier()
    {
        var parser = new BasicRollExpressionParser();

        var result = parser.Parse("2d6+-");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(RollExpressionParseErrorCode.InvalidModifier, result.ErrorCode);
    }

    [TestMethod]
    public void RulesModuleStubs_MapExpressionOutput_FromSharedDiceService()
    {
        var provider = new DeterministicDiceRandomProvider([5, 9]);
        IDiceRandomizationService sharedService = new DiceRandomizationService(provider);

        var checkers = new CheckersRulesModuleStub(sharedService);
        var brp = new BrpRulesModuleStub(sharedService);

        var checkersResult = checkers.ResolveExpression("1d6");
        var brpResult = brp.ResolveExpression("1d10");

        Assert.AreEqual(5, checkersResult.RollResult.Total);
        Assert.AreEqual("checkers:expression", checkersResult.ContextTag);
        Assert.AreEqual(9, brpResult.RollResult.Total);
        Assert.AreEqual("brp:expression", brpResult.ContextTag);
    }
}
