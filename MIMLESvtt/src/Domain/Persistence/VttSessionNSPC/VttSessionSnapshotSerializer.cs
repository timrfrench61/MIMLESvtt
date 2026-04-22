using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Models;
using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.VttSessionNSPC
{
    public class VttSessionSnapshotSerializer
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new();

        public string Save(VttSession vttSession)
        {
            ArgumentNullException.ThrowIfNull(vttSession);

            var snapshot = new VttSessionSnapshot
            {
                Version = CurrentVersion,
                VttSession = vttSession
            };

            return SerializeSnapshot(snapshot);
        }

        public VttSession Load(string json)
        {
            var snapshot = DeserializeSnapshot(json);

            return snapshot.VttSession!;
        }

        public string SerializeSnapshot(VttSessionSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            ValidateSnapshot(snapshot);

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public VttSessionSnapshot DeserializeSnapshot(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            var snapshot = JsonSerializer.Deserialize<VttSessionSnapshot>(json, SerializerOptions);
            if (snapshot is null)
            {
                throw new InvalidOperationException("Failed to deserialize VttSession snapshot.");
            }

            ValidateSnapshot(snapshot);

            return snapshot;
        }

        private static void ValidateSnapshot(VttSessionSnapshot snapshot)
        {
            if (snapshot.Version != CurrentVersion)
            {
                throw new InvalidOperationException("Version is missing or unsupported for VttSession snapshot.");
            }

            if (snapshot.VttSession is null)
            {
                throw new InvalidOperationException("VttSession is required in the snapshot.");
            }
        }
    }
}
