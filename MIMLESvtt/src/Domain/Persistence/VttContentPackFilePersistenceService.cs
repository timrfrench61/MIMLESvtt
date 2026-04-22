using System.Text;

namespace MIMLESvtt.src
{
    public class VttContentPackFilePersistenceService
    {
        private readonly VttContentPackSnapshotSerializer _serializer;

        public VttContentPackFilePersistenceService()
            : this(new VttContentPackSnapshotSerializer())
        {
        }

        public VttContentPackFilePersistenceService(VttContentPackSnapshotSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SaveToFile(VttContentPackSnapshot contentPack, string path)
        {
            ArgumentNullException.ThrowIfNull(contentPack);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var json = _serializer.SerializeContentPack(contentPack);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public void SaveToFile(VttContentPack contentPack, string path)
        {
            ArgumentNullException.ThrowIfNull(contentPack);

            var snapshot = VttContentPackMapper.ToSnapshot(contentPack, VttContentPackSnapshotSerializer.CurrentVersion);
            SaveToFile(snapshot, path);
        }

        public VttContentPackSnapshot LoadFromFile(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("ContentPack snapshot file was not found.", path);
            }

            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                return _serializer.DeserializeContentPack(json);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new InvalidOperationException("ContentPack snapshot file contains malformed JSON.", ex);
            }
        }

        public VttContentPack LoadModelFromFile(string path)
        {
            var snapshot = LoadFromFile(path);
            return VttContentPackMapper.FromSnapshot(snapshot);
        }
    }
}
