using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;
using System.Text;

namespace MIMLESvtt.src.Domain.Persistence.Services.VttFile
{
    public class VttSessionFilePersistenceService
    {
        private readonly VttSessionPersistenceService _persistenceService;

        public VttSessionFilePersistenceService()
            : this(new VttSessionPersistenceService())
        {
        }

        public VttSessionFilePersistenceService(VttSessionPersistenceService persistenceService)
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
                throw new FileNotFoundException("VttSession snapshot file was not found.", path);
            }

            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                return _persistenceService.Load(json);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new InvalidOperationException("VttSession snapshot file contains malformed JSON.", ex);
            }
        }
    }
}
