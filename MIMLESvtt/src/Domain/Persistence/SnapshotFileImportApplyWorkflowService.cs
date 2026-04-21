using System.Text;

namespace MIMLESvtt.src
{
    public class SnapshotFileImportApplyWorkflowService
    {
        private const string UnknownFormatMarker = "does not match any known snapshot format";

        private readonly SnapshotImportWorkflowService _snapshotImportWorkflowService;
        private readonly SnapshotImportApplyWorkflowService _snapshotImportApplyWorkflowService;
        private readonly ScenarioActivationWorkflowService _scenarioActivationWorkflowService;

        public SnapshotFileImportApplyWorkflowService()
            : this(
                new SnapshotImportWorkflowService(),
                new SnapshotImportApplyWorkflowService(),
                new ScenarioActivationWorkflowService())
        {
        }

        public SnapshotFileImportApplyWorkflowService(
            SnapshotImportWorkflowService snapshotImportWorkflowService,
            SnapshotImportApplyWorkflowService snapshotImportApplyWorkflowService,
            ScenarioActivationWorkflowService scenarioActivationWorkflowService)
        {
            _snapshotImportWorkflowService = snapshotImportWorkflowService ?? throw new ArgumentNullException(nameof(snapshotImportWorkflowService));
            _snapshotImportApplyWorkflowService = snapshotImportApplyWorkflowService ?? throw new ArgumentNullException(nameof(snapshotImportApplyWorkflowService));
            _scenarioActivationWorkflowService = scenarioActivationWorkflowService ?? throw new ArgumentNullException(nameof(scenarioActivationWorkflowService));
        }

        public SnapshotFileImportApplyResponse ImportAndApplyFromFile(
            string path,
            SnapshotImportApplyContext context,
            ScenarioCandidateActivationMode scenarioActivationMode,
            SnapshotImportApplyPolicy policy)
        {
            var requestId = Guid.NewGuid().ToString("N");

            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(path);
                ArgumentNullException.ThrowIfNull(context);
                ArgumentNullException.ThrowIfNull(policy);

                var detectedFormat = DetectFormatFromExtension(path);
                if (detectedFormat is null)
                {
                    return CreateFailureResponse(
                        requestId,
                        path,
                        null,
                        "Unsupported snapshot file extension.",
                        SnapshotImportErrorCode.InvalidInput,
                        SnapshotImportFailureStage.FacadeInput,
                        context.CurrentTableSession);
                }

                if (!File.Exists(path))
                {
                    return CreateFailureResponse(
                        requestId,
                        path,
                        detectedFormat,
                        "Snapshot file was not found.",
                        SnapshotImportErrorCode.ValidationFailure,
                        SnapshotImportFailureStage.Dispatch,
                        context.CurrentTableSession);
                }

                var json = File.ReadAllText(path, Encoding.UTF8);

                return detectedFormat.Value switch
                {
                    SnapshotFormatKind.TableSessionSnapshot => RunTableSessionFlow(requestId, path, json, context, policy),
                    SnapshotFormatKind.ScenarioSnapshot => RunScenarioFlow(requestId, path, json, context, scenarioActivationMode, policy),
                    SnapshotFormatKind.ContentPackSnapshot => RunImportOnlyFlow(requestId, path, json, detectedFormat.Value, context),
                    SnapshotFormatKind.ActionLogSnapshot => RunImportOnlyFlow(requestId, path, json, detectedFormat.Value, context),
                    _ => CreateFailureResponse(
                        requestId,
                        path,
                        null,
                        "Unsupported snapshot format.",
                        SnapshotImportErrorCode.InvalidInput,
                        SnapshotImportFailureStage.FacadeInput,
                        context.CurrentTableSession)
                };
            }
            catch (ArgumentException ex)
            {
                return CreateFailureResponse(
                    requestId,
                    path,
                    null,
                    ex.Message,
                    SnapshotImportErrorCode.InvalidInput,
                    SnapshotImportFailureStage.FacadeInput,
                    context?.CurrentTableSession);
            }
            catch (InvalidOperationException ex) when (ex.InnerException is System.Text.Json.JsonException)
            {
                return CreateFailureResponse(
                    requestId,
                    path,
                    DetectFormatFromExtension(path),
                    ex.Message,
                    SnapshotImportErrorCode.MalformedJson,
                    SnapshotImportFailureStage.Dispatch,
                    context.CurrentTableSession);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains(UnknownFormatMarker, StringComparison.Ordinal))
            {
                return CreateFailureResponse(
                    requestId,
                    path,
                    DetectFormatFromExtension(path),
                    ex.Message,
                    SnapshotImportErrorCode.UnknownFormat,
                    SnapshotImportFailureStage.Dispatch,
                    context.CurrentTableSession);
            }
            catch (InvalidOperationException ex)
            {
                return CreateFailureResponse(
                    requestId,
                    path,
                    DetectFormatFromExtension(path),
                    ex.Message,
                    SnapshotImportErrorCode.ValidationFailure,
                    SnapshotImportFailureStage.FormatValidation,
                    context.CurrentTableSession);
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(
                    requestId,
                    path,
                    DetectFormatFromExtension(path),
                    ex.Message,
                    SnapshotImportErrorCode.UnexpectedError,
                    SnapshotImportFailureStage.Unexpected,
                    context.CurrentTableSession);
            }
        }

        private SnapshotFileImportApplyResponse RunTableSessionFlow(
            string requestId,
            string path,
            string json,
            SnapshotImportApplyContext context,
            SnapshotImportApplyPolicy policy)
        {
            var applyResponse = _snapshotImportApplyWorkflowService.ImportAndApply(json, context, policy);

            return new SnapshotFileImportApplyResponse
            {
                RequestId = requestId,
                FilePath = path,
                DetectedFormat = SnapshotFormatKind.TableSessionSnapshot,
                IsSuccess = applyResponse.IsSuccess,
                IsSupported = applyResponse.IsSupported,
                IsRuntimeStateMutated = applyResponse.IsRuntimeStateMutated,
                ResultingCurrentTableSession = context.CurrentTableSession,
                Message = applyResponse.Message,
                ErrorMessage = applyResponse.ErrorMessage,
                ErrorCode = applyResponse.ErrorCode,
                FailureStage = applyResponse.FailureStage,
                TableSessionApplyResponse = applyResponse,
                ScenarioActivationResponse = null,
                ImportOutcome = null
            };
        }

        private SnapshotFileImportApplyResponse RunScenarioFlow(
            string requestId,
            string path,
            string json,
            SnapshotImportApplyContext context,
            ScenarioCandidateActivationMode activationMode,
            SnapshotImportApplyPolicy policy)
        {
            var scenarioResponse = _scenarioActivationWorkflowService.Run(json, context, activationMode, policy);

            return new SnapshotFileImportApplyResponse
            {
                RequestId = requestId,
                FilePath = path,
                DetectedFormat = SnapshotFormatKind.ScenarioSnapshot,
                IsSuccess = scenarioResponse.IsSuccess,
                IsSupported = scenarioResponse.IsSupported,
                IsRuntimeStateMutated = scenarioResponse.IsRuntimeStateMutated,
                ResultingCurrentTableSession = context.CurrentTableSession,
                Message = scenarioResponse.Message,
                ErrorMessage = scenarioResponse.ErrorMessage,
                ErrorCode = scenarioResponse.ErrorCode,
                FailureStage = scenarioResponse.FailureStage,
                TableSessionApplyResponse = null,
                ScenarioActivationResponse = scenarioResponse,
                ImportOutcome = null
            };
        }

        private SnapshotFileImportApplyResponse RunImportOnlyFlow(
            string requestId,
            string path,
            string json,
            SnapshotFormatKind detectedFormat,
            SnapshotImportApplyContext context)
        {
            var outcome = _snapshotImportWorkflowService.Import(json);

            return new SnapshotFileImportApplyResponse
            {
                RequestId = requestId,
                FilePath = path,
                DetectedFormat = detectedFormat,
                IsSuccess = true,
                IsSupported = outcome.IsSupported,
                IsRuntimeStateMutated = false,
                ResultingCurrentTableSession = context.CurrentTableSession,
                Message = outcome.Message,
                ErrorMessage = null,
                ErrorCode = SnapshotImportErrorCode.None,
                FailureStage = SnapshotImportFailureStage.None,
                TableSessionApplyResponse = null,
                ScenarioActivationResponse = null,
                ImportOutcome = outcome
            };
        }

        private static SnapshotFileImportApplyResponse CreateFailureResponse(
            string requestId,
            string path,
            SnapshotFormatKind? detectedFormat,
            string errorMessage,
            SnapshotImportErrorCode errorCode,
            SnapshotImportFailureStage failureStage,
            TableSession? resultingCurrentTableSession)
        {
            return new SnapshotFileImportApplyResponse
            {
                RequestId = requestId,
                FilePath = path,
                DetectedFormat = detectedFormat,
                IsSuccess = false,
                IsSupported = false,
                IsRuntimeStateMutated = false,
                ResultingCurrentTableSession = resultingCurrentTableSession,
                Message = null,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                FailureStage = failureStage,
                TableSessionApplyResponse = null,
                ScenarioActivationResponse = null,
                ImportOutcome = null
            };
        }

        private static SnapshotFormatKind? DetectFormatFromExtension(string path)
        {
            if (path.EndsWith(SnapshotFileExtensions.TableSession, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.TableSessionSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.Scenario, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.ScenarioSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.ContentPack, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.ContentPackSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.ActionLog, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.ActionLogSnapshot;
            }

            return null;
        }
    }
}
