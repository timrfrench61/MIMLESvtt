using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Import;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Services.Scenario
{
    public class VttScenarioActivationWorkflowService
    {
        private readonly SnapshotImportApplyWorkflowService _importApplyWorkflowService;
        private readonly VttScenarioPlanApplyService _vttScenarioPlanApplyService;
        private readonly VttScenarioCandidateActivationService _vttScenarioCandidateActivationService;

        public VttScenarioActivationWorkflowService()
            : this(
                new SnapshotImportApplyWorkflowService(),
                new VttScenarioPlanApplyService(),
                new VttScenarioCandidateActivationService())
        {
        }

        public VttScenarioActivationWorkflowService(
            SnapshotImportApplyWorkflowService importApplyWorkflowService,
            VttScenarioPlanApplyService vttScenarioPlanApplyService,
            VttScenarioCandidateActivationService vttScenarioCandidateActivationService)
        {
            _importApplyWorkflowService = importApplyWorkflowService ?? throw new ArgumentNullException(nameof(importApplyWorkflowService));
            _vttScenarioPlanApplyService = vttScenarioPlanApplyService ?? throw new ArgumentNullException(nameof(vttScenarioPlanApplyService));
            _vttScenarioCandidateActivationService = vttScenarioCandidateActivationService ?? throw new ArgumentNullException(nameof(vttScenarioCandidateActivationService));
        }

        public VttScenarioActivationWorkflowResponse Run(
            string json,
            SnapshotImportApplyContext context,
            VttScenarioCandidateActivationMode activationMode,
            SnapshotImportApplyPolicy policy)
        {
            var requestId = Guid.NewGuid().ToString("N");

            try
            {
                ArgumentNullException.ThrowIfNull(context);
                ArgumentNullException.ThrowIfNull(policy);

                var importApplyResponse = _importApplyWorkflowService.ImportAndApply(json, context, policy);
                if (!importApplyResponse.IsSuccess)
                {
                    return CreateFailureResponse(
                        requestId,
                        importApplyResponse.ErrorMessage ?? "Scenario import/apply workflow failed.",
                        importApplyResponse.ErrorCode,
                        importApplyResponse.FailureStage,
                        importApplyResponse.PendingVttScenarioPlan,
                        null,
                        null,
                        activationMode);
                }

                if (importApplyResponse.FormatKind != SnapshotFormatKind.VttScenarioSnapshot)
                {
                    return new VttScenarioActivationWorkflowResponse
                    {
                        RequestId = requestId,
                        IsSuccess = false,
                        IsSupported = false,
                        PendingVttScenarioPlanCreated = false,
                        CandidateCreated = false,
                        IsRuntimeStateMutated = false,
                        ActivationMode = activationMode,
                        ResultingCurrentVttSession = context.CurrentVttSession,
                        PendingVttScenarioPlan = null,
                        VttSessionCandidate = null,
                        Message = "Scenario activation workflow supports only ScenarioSnapshot format.",
                        ErrorMessage = null,
                        ErrorCode = SnapshotImportErrorCode.ValidationFailure,
                        FailureStage = SnapshotImportFailureStage.ApplicationMapping
                    };
                }

                if (importApplyResponse.PendingVttScenarioPlan is null)
                {
                    return new VttScenarioActivationWorkflowResponse
                    {
                        RequestId = requestId,
                        IsSuccess = false,
                        IsSupported = false,
                        PendingVttScenarioPlanCreated = false,
                        CandidateCreated = false,
                        IsRuntimeStateMutated = false,
                        ActivationMode = activationMode,
                        ResultingCurrentVttSession = context.CurrentVttSession,
                        PendingVttScenarioPlan = null,
                        VttSessionCandidate = null,
                        Message = "Scenario workflow did not produce a pending scenario plan.",
                        ErrorMessage = null,
                        ErrorCode = SnapshotImportErrorCode.ValidationFailure,
                        FailureStage = SnapshotImportFailureStage.ApplicationMapping
                    };
                }

                var planApplyResult = _vttScenarioPlanApplyService.Apply(new VttScenarioPlanApplyRequest
                {
                    PendingVttScenarioPlan = importApplyResponse.PendingVttScenarioPlan,
                    ActiveRuntimeVttSession = context.CurrentVttSession
                });

                if (!planApplyResult.IsSuccess || planApplyResult.VttSessionCandidate is null)
                {
                    return CreateFailureResponse(
                        requestId,
                        planApplyResult.ErrorMessage ?? "Scenario plan candidate creation failed.",
                        SnapshotImportErrorCode.ValidationFailure,
                        SnapshotImportFailureStage.ApplicationMapping,
                        importApplyResponse.PendingVttScenarioPlan,
                        null,
                        context.CurrentVttSession,
                        activationMode);
                }

                var activationResult = _vttScenarioCandidateActivationService.Activate(
                    new VttScenarioCandidateActivationRequest
                    {
                        VttSessionCandidate = planApplyResult.VttSessionCandidate,
                        TargetContext = context,
                        Mode = activationMode
                    },
                    policy);

                return new VttScenarioActivationWorkflowResponse
                {
                    RequestId = requestId,
                    IsSuccess = activationResult.IsSuccess,
                    IsSupported = true,
                    PendingVttScenarioPlanCreated = true,
                    CandidateCreated = true,
                    IsRuntimeStateMutated = activationResult.IsRuntimeStateMutated,
                    ActivationMode = activationMode,
                    ResultingCurrentVttSession = activationResult.ResultingCurrentVttSession,
                    PendingVttScenarioPlan = importApplyResponse.PendingVttScenarioPlan,
                    VttSessionCandidate = planApplyResult.VttSessionCandidate,
                    Message = activationResult.Message,
                    ErrorMessage = activationResult.ErrorMessage,
                    ErrorCode = activationResult.IsSuccess ? SnapshotImportErrorCode.None : SnapshotImportErrorCode.ValidationFailure,
                    FailureStage = activationResult.IsSuccess ? SnapshotImportFailureStage.None : SnapshotImportFailureStage.ApplicationMapping
                };
            }
            catch (ArgumentException ex)
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.InvalidInput,
                    SnapshotImportFailureStage.FacadeInput,
                    null,
                    null,
                    null,
                    activationMode);
            }
            catch (InvalidOperationException ex)
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.ValidationFailure,
                    SnapshotImportFailureStage.ApplicationMapping,
                    null,
                    null,
                    context.CurrentVttSession,
                    activationMode);
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.UnexpectedError,
                    SnapshotImportFailureStage.Unexpected,
                    null,
                    null,
                    context.CurrentVttSession,
                    activationMode);
            }
        }

        private static VttScenarioActivationWorkflowResponse CreateFailureResponse(
            string requestId,
            string errorMessage,
            SnapshotImportErrorCode errorCode,
            SnapshotImportFailureStage failureStage,
            VttScenarioPendingApplicationPlan? pendingVttScenarioPlan,
            VttSession? VttSessionCandidate,
            VttSession? resultingCurrentVttSession,
            VttScenarioCandidateActivationMode activationMode)
        {
            return new VttScenarioActivationWorkflowResponse
            {
                RequestId = requestId,
                IsSuccess = false,
                IsSupported = false,
                PendingVttScenarioPlanCreated = pendingVttScenarioPlan is not null,
                CandidateCreated = VttSessionCandidate is not null,
                IsRuntimeStateMutated = false,
                ActivationMode = activationMode,
                ResultingCurrentVttSession = resultingCurrentVttSession,
                PendingVttScenarioPlan = pendingVttScenarioPlan,
                VttSessionCandidate = VttSessionCandidate,
                Message = null,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                FailureStage = failureStage
            };
        }
    }
}
