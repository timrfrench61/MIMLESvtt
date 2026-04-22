using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.VttContentPackNSPC
{
    public class VttContentPackSnapshotSerializer
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new();

        public string SerializeVttContentPack(VttContentPackSnapshot vttContentPack)
        {
            ArgumentNullException.ThrowIfNull(vttContentPack);

            return SerializeSnapshot(vttContentPack);
        }

        public string SerializeContentPack(VttContentPackSnapshot vttContentPack)
        {
            return SerializeVttContentPack(vttContentPack);
        }

        public VttContentPackSnapshot DeserializeVttContentPack(string json)
        {
            return DeserializeSnapshot(json);
        }

        public VttContentPackSnapshot DeserializeContentPack(string json)
        {
            return DeserializeVttContentPack(json);
        }

        public string SerializeSnapshot(VttContentPackSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            ValidateSnapshot(snapshot);

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public VttContentPackSnapshot DeserializeSnapshot(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            var snapshot = JsonSerializer.Deserialize<VttContentPackSnapshot>(json, SerializerOptions);
            if (snapshot is null)
            {
                throw new InvalidOperationException("Failed to deserialize VttContentPack snapshot.");
            }

            ValidateSnapshot(snapshot);

            return snapshot;
        }

        private static void ValidateSnapshot(VttContentPackSnapshot snapshot)
        {
            if (snapshot.Version != CurrentVersion)
            {
                throw new InvalidOperationException("Version is missing or unsupported for VttContentPack snapshot.");
            }

            if (snapshot.Manifest is null)
            {
                throw new InvalidOperationException("Manifest is required in the VttContentPack snapshot.");
            }

            if (snapshot.Definitions is null)
            {
                throw new InvalidOperationException("Definitions are required in the Content Pack snapshot.");
            }

            if (snapshot.Assets is null)
            {
                throw new InvalidOperationException("Assets are required in the Content Pack snapshot.");
            }
        }
    }
}
