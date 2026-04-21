using System.Text;

namespace MIMLESvtt.src
{
    public class ContentPackFilePersistenceService
    {
        private readonly ContentPackSnapshotSerializer _serializer;

        public ContentPackFilePersistenceService()
            : this(new ContentPackSnapshotSerializer())
        {
        }

        public ContentPackFilePersistenceService(ContentPackSnapshotSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SaveToFile(ContentPackSnapshot contentPack, string path)
        {
            ArgumentNullException.ThrowIfNull(contentPack);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var json = _serializer.SerializeContentPack(contentPack);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public ContentPackSnapshot LoadFromFile(string path)
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
    }
}
