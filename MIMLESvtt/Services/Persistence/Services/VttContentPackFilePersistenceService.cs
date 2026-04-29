using MIMLESvtt.src.Domain.Models.VttGamebox;
using MIMLESvtt.src.Domain.Persistence.VttGameboxNSPC;
using System.Text;

namespace MIMLESvtt.src.Domain.Persistence.Services
{
    public class VttGameboxFilePersistenceService
    {
        private readonly VttGameboxSnapshotSerializer _serializer;

        public VttGameboxFilePersistenceService()
            : this(new VttGameboxSnapshotSerializer())
        {
        }

        public VttGameboxFilePersistenceService(VttGameboxSnapshotSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SaveToFile(VttGameboxSnapshot gamebox, string path)
        {
            ArgumentNullException.ThrowIfNull(gamebox);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);
            var directoryPath = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var json = _serializer.SerializeVttGamebox(gamebox);
            File.WriteAllText(fullPath, json, Encoding.UTF8);
        }

        public void SaveToFile(VttGamebox gamebox, string path)
        {
            ArgumentNullException.ThrowIfNull(gamebox);

            var snapshot = VttGameboxMapper.ToSnapshot(gamebox, VttGameboxSnapshotSerializer.CurrentVersion);
            SaveToFile(snapshot, path);
        }

        public VttGameboxSnapshot LoadFromFile(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Gamebox snapshot file was not found.", path);
            }

            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                return _serializer.DeserializeVttGamebox(json);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new InvalidOperationException("Gamebox snapshot file contains malformed JSON.", ex);
            }
        }

        public VttGamebox LoadModelFromFile(string path)
        {
            var snapshot = LoadFromFile(path);
            return VttGameboxMapper.FromSnapshot(snapshot);
        }
    }
}
