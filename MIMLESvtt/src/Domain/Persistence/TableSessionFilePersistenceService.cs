using MIMLESvtt.src.Domain.Models;
using System.Text;

namespace MIMLESvtt.src
{
    public class TableSessionFilePersistenceService
    {
        private readonly TableSessionPersistenceService _persistenceService;

        public TableSessionFilePersistenceService()
            : this(new TableSessionPersistenceService())
        {
        }

        public TableSessionFilePersistenceService(TableSessionPersistenceService persistenceService)
        {
            _persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
        }

        public void SaveToFile(VttSession session, string path)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var json = _persistenceService.Save(session);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public VttSession LoadFromFile(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("TableSession snapshot file was not found.", path);
            }

            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                return _persistenceService.Load(json);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new InvalidOperationException("TableSession snapshot file contains malformed JSON.", ex);
            }
        }
    }
}
