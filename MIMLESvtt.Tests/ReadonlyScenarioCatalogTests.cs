using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class ReadonlyScenarioCatalogTests
{
    [TestMethod]
    public void ReadonlyScenarioCatalog_ContainsEmptyAndCheckersScenarios()
    {
        var service = new VttSessionWorkspaceService();

        var scenarios = service.ListReadonlyScenarios();

        Assert.IsTrue(scenarios.Any(s => s.ScenarioId == "EMPTY-SCENARIO"));
        Assert.IsTrue(scenarios.Any(s => s.ScenarioId == "CHECKERS-SCENARIO"));
    }

    [TestMethod]
    public void ReadonlyCampaign_CanBeHiddenAndUnhidden()
    {
        var service = new VttSessionWorkspaceService();

        service.SetReadonlyCampaignHidden("CHECKERS-CAMPAIGN", true);
        var hiddenScenarios = service.ListReadonlyScenarios();

        Assert.IsFalse(hiddenScenarios.Any(s => s.CampaignId == "CHECKERS-CAMPAIGN"));

        service.SetReadonlyCampaignHidden("CHECKERS-CAMPAIGN", false);
        var visibleScenarios = service.ListReadonlyScenarios();

        Assert.IsTrue(visibleScenarios.Any(s => s.CampaignId == "CHECKERS-CAMPAIGN"));
    }

    [TestMethod]
    public void ActivateReadonlyScenario_AsAdmin_ActivatesCheckersCampaignScenario()
    {
        var service = new VttSessionWorkspaceService();
        service.SetCanCreateSession(true);

        service.ActivateReadonlyScenario("CHECKERS-SCENARIO");

        Assert.IsNotNull(service.State.CurrentVttSession);
        Assert.AreEqual(1, service.State.CurrentVttSession!.Campaigns.Count);
        Assert.AreEqual("CHECKERS-CAMPAIGN", service.State.CurrentVttSession.Campaigns[0].Id);
        Assert.IsTrue(service.State.CurrentVttSession.Pieces.Count > 0);
        Assert.IsTrue(service.State.CurrentVttSession.ModuleState.TryGetValue("RulesProfile", out var rulesProfile));
        Assert.AreEqual("Checkers", rulesProfile?.ToString());
    }
}
