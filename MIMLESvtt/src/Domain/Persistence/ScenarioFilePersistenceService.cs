using System.Text;

namespace MIMLESvtt.src
{
    public class ScenarioFilePersistenceService
    {
        private readonly ScenarioSnapshotSerializer _serializer;

        public ScenarioFilePersistenceService()
            : this(new ScenarioSnapshotSerializer())
        {
        }

        public ScenarioFilePersistenceService(ScenarioSnapshotSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SaveToFile(ScenarioExport scenario, string path)
        {
            ArgumentNullException.ThrowIfNull(scenario);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var json = _serializer.SerializeScenario(scenario);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public ScenarioExport LoadFromFile(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Scenario snapshot file was not found.", path);
            }

            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                return _serializer.DeserializeScenario(json);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new InvalidOperationException("Scenario snapshot file contains malformed JSON.", ex);
            }
        }
    }
}
