using MIMLESvtt.src.Domain.Persistence.Model;
using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC
{
    public class VttScenarioSnapshotSerializer
    {
        public const int CurrentVersion = 1;

        private static readonly JsonSerializerOptions SerializerOptions = new();

        public string SerializeVttScenario(VttScenario scenario)
        {
            ArgumentNullException.ThrowIfNull(scenario);

            var snapshot = new VttScenarioSnapshot
            {
                Version = CurrentVersion,
                VttScenario = scenario
            };

            return SerializeSnapshot(snapshot);
        }

        public VttScenario DeserializeVttScenario(string json)
        {
            var snapshot = DeserializeSnapshot(json);

            return snapshot.VttScenario!;
        }

        public string SerializeSnapshot(VttScenarioSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            ValidateSnapshot(snapshot);

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public VttScenarioSnapshot DeserializeSnapshot(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            var snapshot = JsonSerializer.Deserialize<VttScenarioSnapshot>(json, SerializerOptions);
            if (snapshot is null)
            {
                throw new InvalidOperationException("Failed to deserialize Scenario snapshot.");
            }

            ValidateSnapshot(snapshot);

            return snapshot;
        }

        private static void ValidateSnapshot(VttScenarioSnapshot snapshot)
        {
            if (snapshot.Version != CurrentVersion)
            {
                throw new InvalidOperationException("Version is missing or unsupported for Scenario snapshot.");
            }

            if (snapshot.VttScenario is null)
            {
                throw new InvalidOperationException("VttScenario is required in the snapshot.");
            }
        }
    }
}
