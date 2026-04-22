using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC
{
    public class VttScenarioPlanApplyResult
    {
        public bool IsSuccess { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public VttSession? VttSessionCandidate { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }
    }
}
