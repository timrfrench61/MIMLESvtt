using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ScenarioCandidateActivationServiceTests
{
    [TestMethod]
    public void Activation_WithValidCandidate_DryRun_ReturnsSuccessAndDoesNotMutateContext()
    {
        var service = new ScenarioCandidateActivationService();
        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext { CurrentTableSession = currentSession };
        var candidate = new TableSession { Id = "candidate-session", Title = "Candidate" };

        var request = new ScenarioCandidateActivationRequest
        {
            TableSessionCandidate = candidate,
            TargetContext = context,
            Mode = ScenarioCandidateActivationMode.DryRun
        };

        var result = service.Activate(request);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsRuntimeStateMutated);
        Assert.AreEqual(ScenarioCandidateActivationMode.DryRun, result.Mode);
        Assert.AreSame(currentSession, context.CurrentTableSession);
        Assert.AreSame(currentSession, result.ResultingCurrentTableSession);
    }

    [TestMethod]
    public void Activation_WithValidCandidate_Activate_ReplacesCurrentTableSessionWhenPolicyAllows()
    {
        var service = new ScenarioCandidateActivationService();
        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var candidate = new TableSession { Id = "candidate-session", Title = "Candidate" };
        var request = new ScenarioCandidateActivationRequest
        {
            TableSessionCandidate = candidate,
            TargetContext = context,
            Mode = ScenarioCandidateActivationMode.Activate
        };

        var policy = new SnapshotImportApplyPolicy
        {
            AllowActivateScenarioCandidate = true
        };

        var result = service.Activate(request, policy);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.IsRuntimeStateMutated);
        Assert.AreEqual(ScenarioCandidateActivationMode.Activate, result.Mode);
        Assert.AreSame(candidate, context.CurrentTableSession);
        Assert.AreSame(candidate, result.ResultingCurrentTableSession);
    }

    [TestMethod]
    public void Activation_WithValidCandidate_Activate_DoesNotMutateWhenPolicyDenies()
    {
        var service = new ScenarioCandidateActivationService();
        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext { CurrentTableSession = currentSession };
        var candidate = new TableSession { Id = "candidate-session", Title = "Candidate" };

        var request = new ScenarioCandidateActivationRequest
        {
            TableSessionCandidate = candidate,
            TargetContext = context,
            Mode = ScenarioCandidateActivationMode.Activate
        };

        var policy = new SnapshotImportApplyPolicy
        {
            AllowActivateScenarioCandidate = false
        };

        var result = service.Activate(request, policy);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsFalse(result.IsRuntimeStateMutated);
        Assert.AreEqual(ScenarioCandidateActivationMode.Activate, result.Mode);
        Assert.AreSame(currentSession, context.CurrentTableSession);
        Assert.AreSame(currentSession, result.ResultingCurrentTableSession);
        StringAssert.Contains(result.Message!, "denied");
    }

    [TestMethod]
    public void Activation_WithMissingCandidate_FailsClearly()
    {
        var service = new ScenarioCandidateActivationService();

        var request = new ScenarioCandidateActivationRequest
        {
            TableSessionCandidate = null,
            TargetContext = new SnapshotImportApplyContext(),
            Mode = ScenarioCandidateActivationMode.DryRun
        };

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.Activate(request));
        StringAssert.Contains(exception.Message, "TableSessionCandidate");
    }

    [TestMethod]
    public void Activation_WithMissingContext_FailsClearlyWhenActivateRequested()
    {
        var service = new ScenarioCandidateActivationService();

        var request = new ScenarioCandidateActivationRequest
        {
            TableSessionCandidate = new TableSession { Id = "candidate-session", Title = "Candidate" },
            TargetContext = null,
            Mode = ScenarioCandidateActivationMode.Activate
        };

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.Activate(request));
        StringAssert.Contains(exception.Message, "TargetContext");
    }

    [TestMethod]
    public void Activation_WithDefaultPolicy_PreservesIntendedActivationBehavior()
    {
        var service = new ScenarioCandidateActivationService();
        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var candidate = new TableSession { Id = "candidate-session", Title = "Candidate" };
        var request = new ScenarioCandidateActivationRequest
        {
            TableSessionCandidate = candidate,
            TargetContext = context,
            Mode = ScenarioCandidateActivationMode.Activate
        };

        var result = service.Activate(request);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.IsRuntimeStateMutated);
        Assert.AreSame(candidate, context.CurrentTableSession);
    }
}
