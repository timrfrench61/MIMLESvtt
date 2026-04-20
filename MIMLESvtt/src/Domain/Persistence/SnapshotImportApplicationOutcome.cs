namespace MIMLESvtt.src
{
    public class SnapshotImportApplicationOutcome
    {
        public SnapshotImportApplicationOutcome(SnapshotFormatKind formatKind, bool isSupported, object? payload, string message)
        {
            FormatKind = formatKind;
            IsSupported = isSupported;
            Payload = payload;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public SnapshotFormatKind FormatKind { get; }

        public bool IsSupported { get; }

        public object? Payload { get; }

        public string Message { get; }
    }
}
