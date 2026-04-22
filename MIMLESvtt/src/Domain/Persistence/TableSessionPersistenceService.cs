using MIMLESvtt.src.Domain.Models;

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

        public string Save(VttSession tableSession)
        {
            ArgumentNullException.ThrowIfNull(tableSession);

            return _serializer.Save(tableSession);
        }

        public VttSession Load(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            return _serializer.Load(json);
        }
    }
}
