using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class TabletopLaunchStateFactoryTests
{
    [TestMethod]
    public void CreateInitialState_PlayMode_SetsMoveToolAndLaunchStatus()
    {
        var result = TabletopLaunchStateFactory.CreateInitialState("play", "NEW:CHECKERS");

        Assert.AreEqual("Move", result.ActiveToolId);
        Assert.AreEqual("Launched in play mode from NEW:CHECKERS.", result.StatusMessage);
    }

    [TestMethod]
    public void CreateInitialState_CheckersSource_SeedsCheckersTokens()
    {
        var result = TabletopLaunchStateFactory.CreateInitialState("edit", "CHECKERS-CAMPAIGN");

        Assert.AreEqual(4, result.Tokens.Count);
        Assert.IsTrue(result.Tokens.Any(t => t.TokenId == "B-1"));
        Assert.IsTrue(result.Tokens.Any(t => t.TokenId == "R-2"));
    }

    [TestMethod]
    public void CreateInitialState_DefaultSource_UsesDefaultTokens()
    {
        var result = TabletopLaunchStateFactory.CreateInitialState("edit", null);

        Assert.AreEqual(2, result.Tokens.Count);
        Assert.AreEqual("T-1", result.Tokens[0].TokenId);
        Assert.AreEqual("T-2", result.Tokens[1].TokenId);
        Assert.AreEqual("Select", result.ActiveToolId);
        Assert.AreEqual("Launched in edit mode from default source.", result.StatusMessage);
    }
}
