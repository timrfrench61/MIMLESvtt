using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Services.Scenario    
{
    public class VttScenarioPendingApplicationPlan
    {
        public string ScenarioTitle { get; init; } = string.Empty;

        public VttScenario Scenario { get; init; } = new();

        public SnapshotImportApplyOperationKind IntendedOperationKind { get; init; }

        public bool IsReadyForApply { get; init; }

        public int SurfaceCount { get; init; }

        public int PieceCount { get; init; }
    }
}
