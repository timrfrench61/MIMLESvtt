using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Scenario;

namespace MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC
{
    public class VttScenarioPlanApplyRequest
    {
        public VttScenarioPendingApplicationPlan? PendingVttScenarioPlan { get; init; }

        public string? TargetSessionTitleOverride { get; init; }

        public VttSession? ActiveRuntimeVttSession { get; init; }
    }
}
