using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Rules;
using MIMLESvtt.src.Domain.Rules.Add1;
using MIMLESvtt.src.Domain.Rules.Brp;

namespace MIMLESvtt.Tests.Harness;

public static class RulesAssertionHelpers
{
    public static void AssertValid(RulesValidationResult result)
    {
        Assert.IsTrue(result.IsValid, result.Message);
    }

    public static void AssertInvalid(RulesValidationResult result, string expectedMessage)
    {
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(expectedMessage, result.Message);
    }

    public static T AssertOutcomePayload<T>(RulesActionResolutionResult resolution)
        where T : class
    {
        Assert.IsTrue(resolution.IsSuccess);
        Assert.IsNotNull(resolution.EngineActionRequest);
        var payload = resolution.EngineActionRequest!.Payload as T;
        Assert.IsNotNull(payload, $"Expected payload type {typeof(T).Name}.");
        return payload!;
    }

    public static void AssertAdd1AttackHit(Add1AttackResult result, int expectedRoll)
    {
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.IsHit);
        Assert.AreEqual(expectedRoll, result.AttackRoll);
    }

    public static void AssertBrpPercentileResult(BrpPercentileCheckResult result, bool expectedPassed)
    {
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(expectedPassed, result.Passed);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Summary));
    }
}
