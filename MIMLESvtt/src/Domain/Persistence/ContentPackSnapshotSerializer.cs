using System.Text.Json;

namespace MIMLESvtt.src
{
    public class ContentPackSnapshotSerializer
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new();

        public string SerializeContentPack(ContentPackSnapshot contentPack)
        {
            ArgumentNullException.ThrowIfNull(contentPack);

            return SerializeSnapshot(contentPack);
        }

        public ContentPackSnapshot DeserializeContentPack(string json)
        {
            return DeserializeSnapshot(json);
        }

        public string SerializeSnapshot(ContentPackSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            ValidateSnapshot(snapshot);

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public ContentPackSnapshot DeserializeSnapshot(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            var snapshot = JsonSerializer.Deserialize<ContentPackSnapshot>(json, SerializerOptions);
            if (snapshot is null)
            {
                throw new InvalidOperationException("Failed to deserialize Content Pack snapshot.");
            }

            ValidateSnapshot(snapshot);

            return snapshot;
        }

        private static void ValidateSnapshot(ContentPackSnapshot snapshot)
        {
            if (snapshot.Version != CurrentVersion)
            {
                throw new InvalidOperationException("Version is missing or unsupported for Content Pack snapshot.");
            }

            if (snapshot.Manifest is null)
            {
                throw new InvalidOperationException("Manifest is required in the Content Pack snapshot.");
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
