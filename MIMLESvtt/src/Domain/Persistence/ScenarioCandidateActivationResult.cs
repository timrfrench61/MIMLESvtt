namespace MIMLESvtt.src
{
    public class ScenarioCandidateActivationResult
    {
        public bool IsSuccess { get; init; }

        public bool IsRuntimeStateMutated { get; init; }

        public ScenarioCandidateActivationMode Mode { get; init; }

        public TableSession? ResultingCurrentTableSession { get; init; }

        public string? Message { get; init; }

        public string? ErrorMessage { get; init; }
    }
}
