using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Scenario;
using MIMLESvtt.src.Domain.Persistence.Snapshot;

using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;
using System.Text;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class SnapshotFileImportApplyWorkflowService
    {
        private const string UnknownFormatMarker = "does not match any known snapshot format";

        private readonly SnapshotImportWorkflowService _snapshotImportWorkflowService;
        private readonly SnapshotImportApplyWorkflowService _snapshotImportApplyWorkflowService;
        private readonly VttScenarioActivationWorkflowService _scenarioActivationWorkflowService;

        public SnapshotFileImportApplyWorkflowService()
            : this(
                new SnapshotImportWorkflowService(),
                new SnapshotImportApplyWorkflowService(),
                new VttScenarioActivationWorkflowService())
        {
        }

        public SnapshotFileImportApplyWorkflowService(
            SnapshotImportWorkflowService snapshotImportWorkflowService,
            SnapshotImportApplyWorkflowService snapshotImportApplyWorkflowService,
            VttScenarioActivationWorkflowService scenarioActivationWorkflowService)
        {
            _snapshotImportWorkflowService = snapshotImportWorkflowService ?? throw new ArgumentNullException(nameof(snapshotImportWorkflowService));
            _snapshotImportApplyWorkflowService = snapshotImportApplyWorkflowService ?? throw new ArgumentNullException(nameof(snapshotImportApplyWorkflowService));
            _scenarioActivationWorkflowService = scenarioActivationWorkflowService ?? throw new ArgumentNullException(nameof(scenarioActivationWorkflowService));
        }

        public SnapshotFileImportApplyResponse ImportAndApplyFromFile(
            string path,
            SnapshotImportApplyContext context,
            VttScenarioCandidateActivationMode scenarioActivationMode,
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
                        context.CurrentVttSession);
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
                        context.CurrentVttSession);
                }

                var json = File.ReadAllText(path, Encoding.UTF8);

                return detectedFormat.Value switch
                {
                    SnapshotFormatKind.VttSessionSnapshot => RunVttSessionFlow(requestId, path, json, context, policy),
                    SnapshotFormatKind.VttScenarioSnapshot => RunScenarioFlow(requestId, path, json, context, scenarioActivationMode, policy),
                    SnapshotFormatKind.VttGameboxSnapshot => RunImportOnlyFlow(requestId, path, json, detectedFormat.Value, context),
                    SnapshotFormatKind.ActionLogSnapshot => RunImportOnlyFlow(requestId, path, json, detectedFormat.Value, context),
                    _ => CreateFailureResponse(
                        requestId,
                        path,
                        null,
                        "Unsupported snapshot format.",
                        SnapshotImportErrorCode.InvalidInput,
                        SnapshotImportFailureStage.FacadeInput,
                        context.CurrentVttSession)
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
                    context?.CurrentVttSession);
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
                    context.CurrentVttSession);
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
                    context.CurrentVttSession);
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
                    context.CurrentVttSession);
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
                    context.CurrentVttSession);
            }
        }

        private SnapshotFileImportApplyResponse RunVttSessionFlow(
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
                DetectedFormat = SnapshotFormatKind.VttSessionSnapshot,
                IsSuccess = applyResponse.IsSuccess,
                IsSupported = applyResponse.IsSupported,
                IsRuntimeStateMutated = applyResponse.IsRuntimeStateMutated,
                ResultingCurrentVttSession = context.CurrentVttSession,
                Message = applyResponse.Message,
                ErrorMessage = applyResponse.ErrorMessage,
                ErrorCode = applyResponse.ErrorCode,
                FailureStage = applyResponse.FailureStage,
                VttSessionApplyResponse = applyResponse,
                ScenarioActivationResponse = null,
                ImportOutcome = null
            };
        }

        private SnapshotFileImportApplyResponse RunScenarioFlow(
            string requestId,
            string path,
            string json,
            SnapshotImportApplyContext context,
            VttScenarioCandidateActivationMode activationMode,
            SnapshotImportApplyPolicy policy)
        {
            var scenarioResponse = _scenarioActivationWorkflowService.Run(json, context, activationMode, policy);

            return new SnapshotFileImportApplyResponse
            {
                RequestId = requestId,
                FilePath = path,
                DetectedFormat = SnapshotFormatKind.VttScenarioSnapshot,
                IsSuccess = scenarioResponse.IsSuccess,
                IsSupported = scenarioResponse.IsSupported,
                IsRuntimeStateMutated = scenarioResponse.IsRuntimeStateMutated,
                ResultingCurrentVttSession = context.CurrentVttSession,
                Message = scenarioResponse.Message,
                ErrorMessage = scenarioResponse.ErrorMessage,
                ErrorCode = scenarioResponse.ErrorCode,
                FailureStage = scenarioResponse.FailureStage,
                VttSessionApplyResponse = null,
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
                ResultingCurrentVttSession = context.CurrentVttSession,
                Message = outcome.Message,
                ErrorMessage = null,
                ErrorCode = SnapshotImportErrorCode.None,
                FailureStage = SnapshotImportFailureStage.None,
                VttSessionApplyResponse = null,
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
            VttSession? resultingCurrentVttSession)
        {
            return new SnapshotFileImportApplyResponse
            {
                RequestId = requestId,
                FilePath = path,
                DetectedFormat = detectedFormat,
                IsSuccess = false,
                IsSupported = false,
                IsRuntimeStateMutated = false,
                ResultingCurrentVttSession = resultingCurrentVttSession,
                Message = null,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                FailureStage = failureStage,
                VttSessionApplyResponse = null,
                ScenarioActivationResponse = null,
                ImportOutcome = null
            };
        }

        private static SnapshotFormatKind? DetectFormatFromExtension(string path)
        {
            if (path.EndsWith(SnapshotFileExtensions.VttSession, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.VttSessionSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.VttScenario, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.VttScenarioSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.VttGamebox, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.VttGameboxSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.ActionLog, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.ActionLogSnapshot;
            }

            return null;
        }
    }
}
