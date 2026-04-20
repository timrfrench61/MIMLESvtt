namespace MIMLESvtt.src
{
    public class SnapshotImportResult
    {
        public SnapshotImportResult(SnapshotFormatKind formatKind, object payload)
        {
            FormatKind = formatKind;
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        public SnapshotFormatKind FormatKind { get; }

        public object Payload { get; }
    }
}
