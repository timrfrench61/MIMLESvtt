using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class ScenarioPlanApplyRequest
    {
        public PendingScenarioApplicationPlan? PendingScenarioPlan { get; init; }

        public string? TargetSessionTitleOverride { get; init; }

        public VttSession? ActiveRuntimeTableSession { get; init; }
    }
}
