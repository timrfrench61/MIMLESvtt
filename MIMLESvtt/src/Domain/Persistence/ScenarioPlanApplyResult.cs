using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class ScenarioPlanApplyResult
    {
        public bool IsSuccess { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public VttSession? TableSessionCandidate { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }
    }
}
