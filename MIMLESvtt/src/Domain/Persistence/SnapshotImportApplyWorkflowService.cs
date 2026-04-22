namespace MIMLESvtt.src
{
    public class SnapshotImportApplyWorkflowService
    {
        private const string UnknownFormatMarker = "does not match any known snapshot format";
        private const string OutcomePayloadMismatchMarker = "outcome must include";
        private const string IntentPayloadMismatchMarker = "intent must include";

        private readonly SnapshotImportWorkflowService _importWorkflowService;
        private readonly SnapshotImportIntentService _intentService;
        private readonly SnapshotImportApplyExecutor _applyExecutor;

        public SnapshotImportApplyWorkflowService()
            : this(
                new SnapshotImportWorkflowService(),
                new SnapshotImportIntentService(),
                new SnapshotImportApplyExecutor())
        {
        }

        public SnapshotImportApplyWorkflowService(
            SnapshotImportWorkflowService importWorkflowService,
            SnapshotImportIntentService intentService,
            SnapshotImportApplyExecutor applyExecutor)
        {
            _importWorkflowService = importWorkflowService ?? throw new ArgumentNullException(nameof(importWorkflowService));
            _intentService = intentService ?? throw new ArgumentNullException(nameof(intentService));
            _applyExecutor = applyExecutor ?? throw new ArgumentNullException(nameof(applyExecutor));
        }

        public SnapshotImportApplyResponse ImportAndApply(string json, SnapshotImportApplyContext context)
        {
            return ImportAndApply(json, context, SnapshotImportApplyPolicy.Default);
        }

        public SnapshotImportApplyResponse ImportAndApply(string json, SnapshotImportApplyContext context, SnapshotImportApplyPolicy policy)
        {
            var requestId = Guid.NewGuid().ToString("N");

            try
            {
                ArgumentNullException.ThrowIfNull(context);
                ArgumentNullException.ThrowIfNull(policy);

                var applicationOutcome = _importWorkflowService.Import(json);
                var intent = _intentService.CreateIntent(applicationOutcome);

                if (!IsAllowedByPolicy(intent, policy))
                {
                    var policyDeniedPendingScenarioTitle = intent.OperationKind == SnapshotImportApplyOperationKind.CreateScenarioFromImport
                        ? (intent.Payload as Scenario)?.Title
                        : null;

                    return new SnapshotImportApplyResponse
                    {
                        RequestId = requestId,
                        IsSuccess = true,
                        IsSupported = intent.IsSupported,
                        IsApplied = false,
                        IsRuntimeStateMutated = false,
                        FormatKind = intent.SourceFormat,
                        OperationKind = intent.OperationKind,
                        ResultingTableSession = context.CurrentTableSession,
                        PendingScenarioTitle = policyDeniedPendingScenarioTitle,
                        PendingScenarioPlan = null,
                        Message = $"{intent.OperationKind} is denied by current apply policy.",
                        ErrorMessage = null,
                        ErrorCode = SnapshotImportErrorCode.None,
                        FailureStage = SnapshotImportFailureStage.None
                    };
                }

                var applyResult = _applyExecutor.Execute(intent, context);

                var isSupported = intent.IsSupported;
                var isSuccess = applyResult.IsSuccess || !isSupported;
                var pendingScenarioTitle = intent.OperationKind == SnapshotImportApplyOperationKind.CreateScenarioFromImport
                    ? applyResult.PendingScenarioPlan?.ScenarioTitle ?? (intent.Payload as Scenario)?.Title
                    : null;

                return new SnapshotImportApplyResponse
                {
                    RequestId = requestId,
                    IsSuccess = isSuccess,
                    IsSupported = isSupported,
                    IsApplied = applyResult.IsApplied,
                    IsRuntimeStateMutated = applyResult.IsRuntimeStateMutated,
                    FormatKind = intent.SourceFormat,
                    OperationKind = applyResult.OperationKind,
                    ResultingTableSession = applyResult.ResultingTableSession,
                    PendingScenarioTitle = pendingScenarioTitle,
                    PendingScenarioPlan = applyResult.PendingScenarioPlan,
                    Message = applyResult.Message,
                    ErrorMessage = null,
                    ErrorCode = SnapshotImportErrorCode.None,
                    FailureStage = SnapshotImportFailureStage.None
                };
            }
            catch (ArgumentException ex)
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.InvalidInput,
                    SnapshotImportFailureStage.FacadeInput);
            }
            catch (InvalidOperationException ex) when (ex.InnerException is System.Text.Json.JsonException)
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.MalformedJson,
                    SnapshotImportFailureStage.Dispatch);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains(UnknownFormatMarker, StringComparison.Ordinal))
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.UnknownFormat,
                    SnapshotImportFailureStage.Dispatch);
            }
            catch (InvalidOperationException ex) when (
                ex.Message.Contains(OutcomePayloadMismatchMarker, StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains(IntentPayloadMismatchMarker, StringComparison.OrdinalIgnoreCase))
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.ValidationFailure,
                    SnapshotImportFailureStage.ApplicationMapping);
            }
            catch (InvalidOperationException ex)
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.ValidationFailure,
                    SnapshotImportFailureStage.FormatValidation);
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(
                    requestId,
                    ex.Message,
                    SnapshotImportErrorCode.UnexpectedError,
                    SnapshotImportFailureStage.Unexpected);
            }
        }

        private static bool IsAllowedByPolicy(SnapshotImportApplyIntent intent, SnapshotImportApplyPolicy policy)
        {
            return intent.OperationKind switch
            {
                SnapshotImportApplyOperationKind.ReplaceTableSession => policy.AllowReplaceTableSession,
                SnapshotImportApplyOperationKind.CreateScenarioFromImport => policy.AllowCreateScenarioFromImport,
                _ => true
            };
        }

        private static SnapshotImportApplyResponse CreateFailureResponse(
            string requestId,
            string errorMessage,
            SnapshotImportErrorCode errorCode,
            SnapshotImportFailureStage failureStage)
        {
            return new SnapshotImportApplyResponse
            {
                RequestId = requestId,
                IsSuccess = false,
                IsSupported = false,
                IsApplied = false,
                IsRuntimeStateMutated = false,
                FormatKind = null,
                OperationKind = null,
                ResultingTableSession = null,
                PendingScenarioTitle = null,
                PendingScenarioPlan = null,
                Message = null,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                FailureStage = failureStage
            };
        }
    }
}
