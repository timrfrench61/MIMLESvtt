using MIMLESvtt.src.Domain.Persistence.ActionLog;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttGameboxNSPC;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;
using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class SnapshotImportService
    {
        private readonly VttSessionSnapshotSerializer VttSessionSerializer;
        private readonly VttScenarioSnapshotSerializer _scenarioSerializer;
        private readonly VttGameboxSnapshotSerializer _VttGameboxSerializer;
        private readonly ActionLogSnapshotSerializer _actionLogSerializer;

        public SnapshotImportService()
            : this(
                new VttSessionSnapshotSerializer(),
                new VttScenarioSnapshotSerializer(),
                new VttGameboxSnapshotSerializer(),
                new ActionLogSnapshotSerializer())
        {
        }

        public SnapshotImportService(
            VttSessionSnapshotSerializer vttSessionSerializer,
            VttScenarioSnapshotSerializer scenarioSerializer,
            VttGameboxSnapshotSerializer gameboxSerializer,
            ActionLogSnapshotSerializer actionLogSerializer)
        {
            VttSessionSerializer = vttSessionSerializer ?? throw new ArgumentNullException(nameof(vttSessionSerializer));
            _scenarioSerializer = scenarioSerializer ?? throw new ArgumentNullException(nameof(scenarioSerializer));
            _VttGameboxSerializer = gameboxSerializer ?? throw new ArgumentNullException(nameof(gameboxSerializer));
            _actionLogSerializer = actionLogSerializer ?? throw new ArgumentNullException(nameof(actionLogSerializer));
        }

        public SnapshotImportResult Import(string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);

            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(json);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Input JSON is malformed.", ex);
            }

            using (document)
            {
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException("Input JSON does not match any known snapshot format.");
                }

                var hasVersion = root.TryGetProperty("Version", out _);
                if (!hasVersion)
                {
                    throw new InvalidOperationException("Input JSON does not match any known snapshot format.");
                }

                if (root.TryGetProperty("VttSession", out _))
                {
                    var payload = VttSessionSerializer.DeserializeSnapshot(json);
                    return new SnapshotImportResult(SnapshotFormatKind.VttSessionSnapshot, payload);
                }

                if (root.TryGetProperty("Scenario", out _)
                    || root.TryGetProperty("VttScenario", out _))
                {
                    var payload = _scenarioSerializer.DeserializeSnapshot(json);
                    return new SnapshotImportResult(SnapshotFormatKind.VttScenarioSnapshot, payload);
                }

                if (root.TryGetProperty("Manifest", out _)
                    && root.TryGetProperty("Definitions", out _)
                    && root.TryGetProperty("Assets", out _))
                {
                    var payload = _VttGameboxSerializer.DeserializeSnapshot(json);
                    return new SnapshotImportResult(SnapshotFormatKind.VttGameboxSnapshot, payload);
                }

                if (root.TryGetProperty("SessionId", out _)
                    && root.TryGetProperty("Actions", out _))
                {
                    var payload = _actionLogSerializer.DeserializeSnapshot(json);
                    return new SnapshotImportResult(SnapshotFormatKind.ActionLogSnapshot, payload);
                }

                throw new InvalidOperationException("Input JSON does not match any known snapshot format.");
            }
        }
    }
}
