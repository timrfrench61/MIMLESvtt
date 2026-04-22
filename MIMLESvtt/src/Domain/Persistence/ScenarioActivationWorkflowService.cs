using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class ScenarioActivationWorkflowService
    {
        private readonly SnapshotImportApplyWorkflowService _importApplyWorkflowService;
        private readonly ScenarioPlanApplyService _scenarioPlanApplyService;
        private readonly ScenarioCandidateActivationService _scenarioCandidateActivationService;

        public ScenarioActivationWorkflowService()
            : this(
                new SnapshotImportApplyWorkflowService(),
                new ScenarioPlanApplyService(),
                new ScenarioCandidateActivationService())
        {
        }

        public ScenarioActivationWorkflowService(
            SnapshotImportApplyWorkflowService importApplyWorkflowService,
            ScenarioPlanApplyService scenarioPlanApplyService,
            ScenarioCandidateActivationService scenarioCandidateActivationService)
        {
            _importApplyWorkflowService = importApplyWorkflowService ?? throw new ArgumentNullException(nameof(importApplyWorkflowService));
            _scenarioPlanApplyService = scenarioPlanApplyService ?? throw new ArgumentNullException(nameof(scenarioPlanApplyService));
            _scenarioCandidateActivationService = scenarioCandidateActivationService ?? throw new ArgumentNullException(nameof(scenarioCandidateActivationService));
        }

        public ScenarioActivationWorkflowResponse Run(
            string json,
            SnapshotImportApplyContext context,
            ScenarioCandidateActivationMode activationMode,
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
                        importApplyResponse.PendingScenarioPlan,
                        null,
                        null,
                        activationMode);
                }

                if (importApplyResponse.FormatKind != SnapshotFormatKind.ScenarioSnapshot)
                {
                    return new ScenarioActivationWorkflowResponse
                    {
                        RequestId = requestId,
                        IsSuccess = false,
                        IsSupported = false,
                        PendingScenarioPlanCreated = false,
                        CandidateCreated = false,
                        IsRuntimeStateMutated = false,
                        ActivationMode = activationMode,
                        ResultingCurrentTableSession = context.CurrentTableSession,
                        PendingScenarioPlan = null,
                        TableSessionCandidate = null,
                        Message = "Scenario activation workflow supports only ScenarioSnapshot format.",
                        ErrorMessage = null,
                        ErrorCode = SnapshotImportErrorCode.ValidationFailure,
                        FailureStage = SnapshotImportFailureStage.ApplicationMapping
                    };
                }

                if (importApplyResponse.PendingScenarioPlan is null)
                {
                    return new ScenarioActivationWorkflowResponse
                    {
                        RequestId = requestId,
                        IsSuccess = false,
                        IsSupported = false,
                        PendingScenarioPlanCreated = false,
                        CandidateCreated = false,
                        IsRuntimeStateMutated = false,
                        ActivationMode = activationMode,
                        ResultingCurrentTableSession = context.CurrentTableSession,
                        PendingScenarioPlan = null,
                        TableSessionCandidate = null,
                        Message = "Scenario workflow did not produce a pending scenario plan.",
                        ErrorMessage = null,
                        ErrorCode = SnapshotImportErrorCode.ValidationFailure,
                        FailureStage = SnapshotImportFailureStage.ApplicationMapping
                    };
                }

                var planApplyResult = _scenarioPlanApplyService.Apply(new ScenarioPlanApplyRequest
                {
                    PendingScenarioPlan = importApplyResponse.PendingScenarioPlan,
                    ActiveRuntimeTableSession = context.CurrentTableSession
                });

                if (!planApplyResult.IsSuccess || planApplyResult.TableSessionCandidate is null)
                {
                    return CreateFailureResponse(
                        requestId,
                        planApplyResult.ErrorMessage ?? "Scenario plan candidate creation failed.",
                        SnapshotImportErrorCode.ValidationFailure,
                        SnapshotImportFailureStage.ApplicationMapping,
                        importApplyResponse.PendingScenarioPlan,
                        null,
                        context.CurrentTableSession,
                        activationMode);
                }

                var activationResult = _scenarioCandidateActivationService.Activate(
                    new ScenarioCandidateActivationRequest
                    {
                        TableSessionCandidate = planApplyResult.TableSessionCandidate,
                        TargetContext = context,
                        Mode = activationMode
                    },
                    policy);

                return new ScenarioActivationWorkflowResponse
                {
                    RequestId = requestId,
                    IsSuccess = activationResult.IsSuccess,
                    IsSupported = true,
                    PendingScenarioPlanCreated = true,
                    CandidateCreated = true,
                    IsRuntimeStateMutated = activationResult.IsRuntimeStateMutated,
                    ActivationMode = activationMode,
                    ResultingCurrentTableSession = activationResult.ResultingCurrentTableSession,
                    PendingScenarioPlan = importApplyResponse.PendingScenarioPlan,
                    TableSessionCandidate = planApplyResult.TableSessionCandidate,
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
                    context.CurrentTableSession,
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
                    context.CurrentTableSession,
                    activationMode);
            }
        }

        private static ScenarioActivationWorkflowResponse CreateFailureResponse(
            string requestId,
            string errorMessage,
            SnapshotImportErrorCode errorCode,
            SnapshotImportFailureStage failureStage,
            PendingScenarioApplicationPlan? pendingScenarioPlan,
            VttSession? tableSessionCandidate,
            VttSession? resultingCurrentTableSession,
            ScenarioCandidateActivationMode activationMode)
        {
            return new ScenarioActivationWorkflowResponse
            {
                RequestId = requestId,
                IsSuccess = false,
                IsSupported = false,
                PendingScenarioPlanCreated = pendingScenarioPlan is not null,
                CandidateCreated = tableSessionCandidate is not null,
                IsRuntimeStateMutated = false,
                ActivationMode = activationMode,
                ResultingCurrentTableSession = resultingCurrentTableSession,
                PendingScenarioPlan = pendingScenarioPlan,
                TableSessionCandidate = tableSessionCandidate,
                Message = null,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                FailureStage = failureStage
            };
        }
    }
}
