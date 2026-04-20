namespace MIMLESvtt.src
{
    public class ScenarioPlanApplyRequest
    {
        public PendingScenarioApplicationPlan? PendingScenarioPlan { get; init; }

        public string? TargetSessionTitleOverride { get; init; }

        public TableSession? ActiveRuntimeTableSession { get; init; }
    }
}
