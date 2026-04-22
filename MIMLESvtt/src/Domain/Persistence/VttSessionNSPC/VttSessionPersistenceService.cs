using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Persistence.VttSessionNSPC
{
    public class VttSessionPersistenceService
    {
        private readonly VttSessionSnapshotSerializer _serializer;

        public VttSessionPersistenceService()
            : this(new VttSessionSnapshotSerializer())
        {
        }

        public VttSessionPersistenceService(VttSessionSnapshotSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public string Save(VttSession vttSession)
        {
            ArgumentNullException.ThrowIfNull(vttSession);

            return _serializer.Save(vttSession);
        }

        public VttSession Load(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            return _serializer.Load(json);
        }
    }
}
