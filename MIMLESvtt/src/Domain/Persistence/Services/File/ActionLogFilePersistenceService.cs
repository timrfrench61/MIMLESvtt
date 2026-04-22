using MIMLESvtt.src.Domain.Persistence.ActionLog;
using MIMLESvtt.src.Domain.Persistence.Models;
using System.Text;

namespace MIMLESvtt.src.Domain.Persistence.Services.VttFile
{
    public class ActionLogFilePersistenceService
    {
        private readonly ActionLogSnapshotSerializer _serializer;

        public ActionLogFilePersistenceService()
            : this(new ActionLogSnapshotSerializer())
        {
        }

        public ActionLogFilePersistenceService(ActionLogSnapshotSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SaveToFile(ActionLogSnapshot actionLog, string path)
        {
            ArgumentNullException.ThrowIfNull(actionLog);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var json = _serializer.SerializeActionLog(actionLog);
           File.WriteAllText(path, json, Encoding.UTF8);
        }

        public ActionLogSnapshot LoadFromFile(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("ActionLog snapshot file was not found.", path);
            }

            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                return _serializer.DeserializeActionLog(json);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new InvalidOperationException("ActionLog snapshot file contains malformed JSON.", ex);
            }
        }
    }
}
