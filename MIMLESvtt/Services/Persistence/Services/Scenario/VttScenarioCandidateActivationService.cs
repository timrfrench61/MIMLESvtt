using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Services.Scenario
{
    public class VttScenarioCandidateActivationService
    {
        public VttScenarioCandidateActivationResult Activate(VttScenarioCandidateActivationRequest request)
        {
            return Activate(request, SnapshotImportApplyPolicy.Default);
        }

        public VttScenarioCandidateActivationResult Activate(VttScenarioCandidateActivationRequest request, SnapshotImportApplyPolicy policy)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(policy);

            if (request.VttSessionCandidate is null)
            {
                throw new InvalidOperationException("VttSessionCandidate is required.");
            }

            ValidateCandidate(request.VttSessionCandidate);

            if (request.Mode == VttScenarioCandidateActivationMode.DryRun)
            {
                return new VttScenarioCandidateActivationResult
                {
                    IsSuccess = true,
                    IsRuntimeStateMutated = false,
                    Mode = VttScenarioCandidateActivationMode.DryRun,
                    ResultingCurrentVttSession = request.TargetContext?.CurrentVttSession,
                    Message = "DryRun completed. Scenario candidate is valid for activation.",
                    ErrorMessage = null
                };
            }

            if (request.TargetContext is null)
            {
                throw new InvalidOperationException("TargetContext is required for Activate mode.");
            }

            if (!policy.AllowActivateVttScenarioCandidate)
            {
                return new VttScenarioCandidateActivationResult
                {
                    IsSuccess = false,
                    IsRuntimeStateMutated = false,
                    Mode = VttScenarioCandidateActivationMode.Activate,
                    ResultingCurrentVttSession = request.TargetContext.CurrentVttSession,
                    Message = "Scenario candidate activation is denied by current apply policy.",
                    ErrorMessage = null
                };
            }

            request.TargetContext.CurrentVttSession = request.VttSessionCandidate;

            return new VttScenarioCandidateActivationResult
            {
                IsSuccess = true,
                IsRuntimeStateMutated = true,
                Mode = VttScenarioCandidateActivationMode.Activate,
                ResultingCurrentVttSession = request.TargetContext.CurrentVttSession,
                Message = "Scenario candidate activated into runtime context.",
                ErrorMessage = null
            };
        }

        private static void ValidateCandidate(VttSession candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate.Id))
            {
                throw new InvalidOperationException("VttSessionCandidate.Id is required.");
            }

            if (string.IsNullOrWhiteSpace(candidate.Title))
            {
                throw new InvalidOperationException("VttSessionCandidate.Title is required.");
            }
        }
    }
}
