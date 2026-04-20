using System.Text.Json;

namespace MIMLESvtt.src
{
    public class ScenarioSnapshotSerializer
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new();

        public string SerializeScenario(ScenarioExport scenario)
        {
            ArgumentNullException.ThrowIfNull(scenario);

            var snapshot = new ScenarioSnapshot
            {
                Version = CurrentVersion,
                Scenario = scenario
            };

            return SerializeSnapshot(snapshot);
        }

        public ScenarioExport DeserializeScenario(string json)
        {
            var snapshot = DeserializeSnapshot(json);

            return snapshot.Scenario!;
        }

        public string SerializeSnapshot(ScenarioSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            ValidateSnapshot(snapshot);

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public ScenarioSnapshot DeserializeSnapshot(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            var snapshot = JsonSerializer.Deserialize<ScenarioSnapshot>(json, SerializerOptions);
            if (snapshot is null)
            {
                throw new InvalidOperationException("Failed to deserialize Scenario snapshot.");
            }

            ValidateSnapshot(snapshot);

            return snapshot;
        }

        private static void ValidateSnapshot(ScenarioSnapshot snapshot)
        {
            if (snapshot.Version != CurrentVersion)
            {
                throw new InvalidOperationException("Version is missing or unsupported for Scenario snapshot.");
            }

            if (snapshot.Scenario is null)
            {
                throw new InvalidOperationException("Scenario is required in the snapshot.");
            }
        }
    }
}
