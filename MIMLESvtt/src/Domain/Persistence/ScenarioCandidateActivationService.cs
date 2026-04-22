using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class ScenarioCandidateActivationService
    {
        public ScenarioCandidateActivationResult Activate(ScenarioCandidateActivationRequest request)
        {
            return Activate(request, SnapshotImportApplyPolicy.Default);
        }

        public ScenarioCandidateActivationResult Activate(ScenarioCandidateActivationRequest request, SnapshotImportApplyPolicy policy)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(policy);

            if (request.TableSessionCandidate is null)
            {
                throw new InvalidOperationException("TableSessionCandidate is required.");
            }

            ValidateCandidate(request.TableSessionCandidate);

            if (request.Mode == ScenarioCandidateActivationMode.DryRun)
            {
                return new ScenarioCandidateActivationResult
                {
                    IsSuccess = true,
                    IsRuntimeStateMutated = false,
                    Mode = ScenarioCandidateActivationMode.DryRun,
                    ResultingCurrentTableSession = request.TargetContext?.CurrentTableSession,
                    Message = "DryRun completed. Scenario candidate is valid for activation.",
                    ErrorMessage = null
                };
            }

            if (request.TargetContext is null)
            {
                throw new InvalidOperationException("TargetContext is required for Activate mode.");
            }

            if (!policy.AllowActivateScenarioCandidate)
            {
                return new ScenarioCandidateActivationResult
                {
                    IsSuccess = false,
                    IsRuntimeStateMutated = false,
                    Mode = ScenarioCandidateActivationMode.Activate,
                    ResultingCurrentTableSession = request.TargetContext.CurrentTableSession,
                    Message = "Scenario candidate activation is denied by current apply policy.",
                    ErrorMessage = null
                };
            }

            request.TargetContext.CurrentTableSession = request.TableSessionCandidate;

            return new ScenarioCandidateActivationResult
            {
                IsSuccess = true,
                IsRuntimeStateMutated = true,
                Mode = ScenarioCandidateActivationMode.Activate,
                ResultingCurrentTableSession = request.TargetContext.CurrentTableSession,
                Message = "Scenario candidate activated into runtime context.",
                ErrorMessage = null
            };
        }

        private static void ValidateCandidate(VttSession candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate.Id))
            {
                throw new InvalidOperationException("TableSessionCandidate.Id is required.");
            }

            if (string.IsNullOrWhiteSpace(candidate.Title))
            {
                throw new InvalidOperationException("TableSessionCandidate.Title is required.");
            }
        }
    }
}
