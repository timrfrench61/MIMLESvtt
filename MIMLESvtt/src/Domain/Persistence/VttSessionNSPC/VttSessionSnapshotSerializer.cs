using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MIMLESvtt.src.Domain.Persistence.VttSessionNSPC
{
    public class VttSessionSnapshotSerializer
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            Converters =
            {
                new ParticipantRoleJsonConverter(),
                new JsonStringEnumConverter()
            }
        };

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

        private sealed class ParticipantRoleJsonConverter : JsonConverter<ParticipantRole>
        {
            public override ParticipantRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var raw = reader.GetString()?.Trim();
                    if (TryParseParticipantRole(raw, out var parsed))
                    {
                        return parsed;
                    }

                    throw new JsonException($"Invalid ParticipantRole value '{raw}'.");
                }

                if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var roleValue))
                {
                    if (Enum.IsDefined(typeof(ParticipantRole), roleValue))
                    {
                        return (ParticipantRole)roleValue;
                    }

                    throw new JsonException($"Invalid numeric ParticipantRole value '{roleValue}'.");
                }

                throw new JsonException("ParticipantRole must be a string or integer value.");
            }

            private static bool TryParseParticipantRole(string? raw, out ParticipantRole role)
            {
                role = default;

                if (string.IsNullOrWhiteSpace(raw))
                {
                    return false;
                }

                if (Enum.TryParse<ParticipantRole>(raw, ignoreCase: true, out role))
                {
                    return true;
                }

                var normalized = raw
                    .Replace(" ", string.Empty, StringComparison.Ordinal)
                    .Replace("-", string.Empty, StringComparison.Ordinal)
                    .Replace("_", string.Empty, StringComparison.Ordinal);

                if (normalized.Equals("DM", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("DungeonMaster", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("GameMaster", StringComparison.OrdinalIgnoreCase))
                {
                    role = ParticipantRole.GM;
                    return true;
                }

                if (normalized.Equals("Spectator", StringComparison.OrdinalIgnoreCase))
                {
                    role = ParticipantRole.Observer;
                    return true;
                }

                return false;
            }

            public override void Write(Utf8JsonWriter writer, ParticipantRole value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
