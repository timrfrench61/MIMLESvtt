namespace MIMLESvtt.src.Domain.Persistence.Snapshot
{
    public class SnapshotImportFacade
    {
        private const string UnknownFormatMarker = "does not match any known snapshot format";
        private const string PayloadMismatchMarker = "payload does not match";

        private readonly SnapshotImportWorkflowService _workflowService;

        public SnapshotImportFacade()
            : this(new SnapshotImportWorkflowService())
        {
        }

        public SnapshotImportFacade(SnapshotImportWorkflowService workflowService)
        {
            _workflowService = workflowService ?? throw new ArgumentNullException(nameof(workflowService));
        }

        public SnapshotImportResponse Import(string json)
        {
            var requestId = Guid.NewGuid().ToString("N");

            try
            {
                var outcome = _workflowService.Import(json);
                return new SnapshotImportResponse
                {
                    RequestId = requestId,
                    IsSuccess = true,
                    Outcome = outcome,
                    ErrorMessage = null,
                    ErrorCode = SnapshotImportErrorCode.None,
                    FailureStage = SnapshotImportFailureStage.None
                };
            }
            catch (ArgumentException ex)
            {
                return new SnapshotImportResponse
                {
                    RequestId = requestId,
                    IsSuccess = false,
                    Outcome = null,
                    ErrorMessage = ex.Message,
                    ErrorCode = SnapshotImportErrorCode.InvalidInput,
                    FailureStage = SnapshotImportFailureStage.FacadeInput
                };
            }
            catch (InvalidOperationException ex) when (ex.InnerException is System.Text.Json.JsonException)
            {
                return new SnapshotImportResponse
                {
                    RequestId = requestId,
                    IsSuccess = false,
                    Outcome = null,
                    ErrorMessage = ex.Message,
                    ErrorCode = SnapshotImportErrorCode.MalformedJson,
                    FailureStage = SnapshotImportFailureStage.Dispatch
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains(UnknownFormatMarker, StringComparison.Ordinal))
            {
                return new SnapshotImportResponse
                {
                    RequestId = requestId,
                    IsSuccess = false,
                    Outcome = null,
                    ErrorMessage = ex.Message,
                    ErrorCode = SnapshotImportErrorCode.UnknownFormat,
                    FailureStage = SnapshotImportFailureStage.Dispatch
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains(PayloadMismatchMarker, StringComparison.OrdinalIgnoreCase))
            {
                return new SnapshotImportResponse
                {
                    RequestId = requestId,
                    IsSuccess = false,
                    Outcome = null,
                    ErrorMessage = ex.Message,
                    ErrorCode = SnapshotImportErrorCode.ValidationFailure,
                    FailureStage = SnapshotImportFailureStage.ApplicationMapping
                };
            }
            catch (InvalidOperationException ex)
            {
                return new SnapshotImportResponse
                {
                    RequestId = requestId,
                    IsSuccess = false,
                    Outcome = null,
                    ErrorMessage = ex.Message,
                    ErrorCode = SnapshotImportErrorCode.ValidationFailure,
                    FailureStage = SnapshotImportFailureStage.FormatValidation
                };
            }
            catch (Exception ex)
            {
                return new SnapshotImportResponse
                {
                    RequestId = requestId,
                    IsSuccess = false,
                    Outcome = null,
                    ErrorMessage = ex.Message,
                    ErrorCode = SnapshotImportErrorCode.UnexpectedError,
                    FailureStage = SnapshotImportFailureStage.Unexpected
                };
            }
        }
    }
}
