namespace MIMLESvtt.src.Domain.Persistence.Snapshot
{
    public class SnapshotImportApplyIntent
    {
        public SnapshotImportApplyIntent(
            SnapshotFormatKind sourceFormat,
            SnapshotImportApplyOperationKind operationKind,
            bool isSupported,
            object? payload,
            string message)
        {
            SourceFormat = sourceFormat;
            OperationKind = operationKind;
            IsSupported = isSupported;
            Payload = payload;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public SnapshotFormatKind SourceFormat { get; }

        public SnapshotImportApplyOperationKind OperationKind { get; }

        public bool IsSupported { get; }

        public object? Payload { get; }

        public string Message { get; }
    }
}
