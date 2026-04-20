namespace MIMLESvtt.src
{
    public class TableSessionPersistenceService
    {
        private readonly TableSessionSnapshotSerializer _serializer;

        public TableSessionPersistenceService()
            : this(new TableSessionSnapshotSerializer())
        {
        }

        public TableSessionPersistenceService(TableSessionSnapshotSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public string Save(TableSession tableSession)
        {
            ArgumentNullException.ThrowIfNull(tableSession);

            return _serializer.Save(tableSession);
        }

        public TableSession Load(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            return _serializer.Load(json);
        }
    }
}
