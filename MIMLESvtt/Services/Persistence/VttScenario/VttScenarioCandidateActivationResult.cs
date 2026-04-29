using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC
{
    public class VttScenarioCandidateActivationResult
    {
        public bool IsSuccess { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public VttScenarioCandidateActivationMode Mode { get; init; }

        public VttSession? ResultingCurrentVttSession { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }
    }
}
