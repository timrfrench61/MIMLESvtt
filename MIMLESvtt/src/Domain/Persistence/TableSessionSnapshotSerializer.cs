using MIMLESvtt.src.Domain.Models;
using System.Text.Json;

namespace MIMLESvtt.src
{
    public class TableSessionSnapshotSerializer
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new();

        public string Save(VttSession tableSession)
        {
            ArgumentNullException.ThrowIfNull(tableSession);

            var snapshot = new TableSessionSnapshot
            {
                Version = CurrentVersion,
                TableSession = tableSession
            };

            return SerializeSnapshot(snapshot);
        }

        public VttSession Load(string json)
        {
            var snapshot = DeserializeSnapshot(json);

            return snapshot.TableSession!;
        }

        public string SerializeSnapshot(TableSessionSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            ValidateSnapshot(snapshot);

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public TableSessionSnapshot DeserializeSnapshot(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            var snapshot = JsonSerializer.Deserialize<TableSessionSnapshot>(json, SerializerOptions);
            if (snapshot is null)
            {
                throw new InvalidOperationException("Failed to deserialize TableSession snapshot.");
            }

            ValidateSnapshot(snapshot);

            return snapshot;
        }

        private static void ValidateSnapshot(TableSessionSnapshot snapshot)
        {
            if (snapshot.Version != CurrentVersion)
            {
                throw new InvalidOperationException("Version is missing or unsupported for TableSession snapshot.");
            }

            if (snapshot.TableSession is null)
            {
                throw new InvalidOperationException("TableSession is required in the snapshot.");
            }
        }
    }
}
