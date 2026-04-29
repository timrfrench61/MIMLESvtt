using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;
using System.Text;

namespace MIMLESvtt.src.Domain.Persistence.Services.VttFile
{
    public class VttScenarioFilePersistenceService
    {
        private readonly VttScenarioSnapshotSerializer _serializer;

        public VttScenarioFilePersistenceService()
            : this(new VttScenarioSnapshotSerializer())
        {
        }

        public VttScenarioFilePersistenceService(VttScenarioSnapshotSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SaveToFile(VttScenario scenario, string path)
        {
            ArgumentNullException.ThrowIfNull(scenario);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var json = _serializer.SerializeVttScenario(scenario);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public VttScenario LoadFromFile(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Scenario snapshot file was not found.", path);
            }

            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                return _serializer.DeserializeVttScenario(json);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new InvalidOperationException("Scenario snapshot file contains malformed JSON.", ex);
            }
        }
    }
}
