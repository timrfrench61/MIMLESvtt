using MIMLESvtt.src.Domain.Persistence.Models;
using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.ActionLog
{
    public class ActionLogSnapshotSerializer
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new();

        public string SerializeActionLog(ActionLogSnapshot actionLog)
        {
            ArgumentNullException.ThrowIfNull(actionLog);

            return SerializeSnapshot(actionLog);
        }

        public ActionLogSnapshot DeserializeActionLog(string json)
        {
            return DeserializeSnapshot(json);
        }

        public string SerializeSnapshot(ActionLogSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            ValidateSnapshot(snapshot);

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public ActionLogSnapshot DeserializeSnapshot(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            var snapshot = JsonSerializer.Deserialize<ActionLogSnapshot>(json, SerializerOptions);
            if (snapshot is null)
            {
                throw new InvalidOperationException("Failed to deserialize Action Log snapshot.");
            }

            ValidateSnapshot(snapshot);

            return snapshot;
        }

        private static void ValidateSnapshot(ActionLogSnapshot snapshot)
        {
            if (snapshot.Version != CurrentVersion)
            {
                throw new InvalidOperationException("Version is missing or unsupported for Action Log snapshot.");
            }

            if (string.IsNullOrWhiteSpace(snapshot.SessionId))
            {
                throw new InvalidOperationException("SessionId is required in the Action Log snapshot.");
            }

            if (snapshot.Actions is null)
            {
                throw new InvalidOperationException("Actions are required in the Action Log snapshot.");
            }
        }
    }
}
