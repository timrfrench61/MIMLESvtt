using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.VttGameboxNSPC
{
    public class VttGameboxSnapshotSerializer   
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new();

        public string SerializeVttGamebox(VttGameboxSnapshot vttGamebox)
        {
            ArgumentNullException.ThrowIfNull(vttGamebox);

            return SerializeSnapshot(vttGamebox);
        }

        public string SerializeVttGameboxSnapshot(VttGameboxSnapshot vttGamebox)
        {
            return SerializeSnapshot(vttGamebox);
        }

        public VttGameboxSnapshot DeserializeVttGameboxSnapshot(string json)
        {
            return DeserializeSnapshot(json);
        }

        public VttGameboxSnapshot DeserializeVttGamebox(string json)
        {
            return DeserializeVttGameboxSnapshot(json);
        }

        public string SerializeSnapshot(VttGameboxSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            ValidateSnapshot(snapshot);

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public VttGameboxSnapshot DeserializeSnapshot(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            var snapshot = JsonSerializer.Deserialize<VttGameboxSnapshot>(json, SerializerOptions);
            if (snapshot is null)
            {
                throw new InvalidOperationException("Failed to deserialize VttGamebox snapshot.");
            }

            ValidateSnapshot(snapshot);

            return snapshot;
        }

        private static void ValidateSnapshot(VttGameboxSnapshot snapshot)
        {
            if (snapshot.Version != CurrentVersion)
            {
                throw new InvalidOperationException("Version is missing or unsupported for VttGamebox snapshot.");
            }

            if (snapshot.Manifest is null)
            {
                throw new InvalidOperationException("Manifest is required in the VttGamebox snapshot.");
            }

            if (snapshot.Definitions is null)
            {
                throw new InvalidOperationException("Definitions are required in the VttGamebox snapshot.");
            }

            if (snapshot.Assets is null)
            {
                throw new InvalidOperationException("Assets are required in the VttGamebox snapshot.");
            }
        }
    }
}
