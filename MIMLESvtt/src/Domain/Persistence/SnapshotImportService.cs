using System.Text.Json;

namespace MIMLESvtt.src
{
    public class SnapshotImportService
    {
        private readonly TableSessionSnapshotSerializer _tableSessionSerializer;
        private readonly ScenarioSnapshotSerializer _scenarioSerializer;
        private readonly ContentPackSnapshotSerializer _contentPackSerializer;
        private readonly ActionLogSnapshotSerializer _actionLogSerializer;

        public SnapshotImportService()
            : this(
                new TableSessionSnapshotSerializer(),
                new ScenarioSnapshotSerializer(),
                new ContentPackSnapshotSerializer(),
                new ActionLogSnapshotSerializer())
        {
        }

        public SnapshotImportService(
            TableSessionSnapshotSerializer tableSessionSerializer,
            ScenarioSnapshotSerializer scenarioSerializer,
            ContentPackSnapshotSerializer contentPackSerializer,
            ActionLogSnapshotSerializer actionLogSerializer)
        {
            _tableSessionSerializer = tableSessionSerializer ?? throw new ArgumentNullException(nameof(tableSessionSerializer));
            _scenarioSerializer = scenarioSerializer ?? throw new ArgumentNullException(nameof(scenarioSerializer));
            _contentPackSerializer = contentPackSerializer ?? throw new ArgumentNullException(nameof(contentPackSerializer));
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

                if (root.TryGetProperty("TableSession", out _))
                {
                    var payload = _tableSessionSerializer.DeserializeSnapshot(json);
                    return new SnapshotImportResult(SnapshotFormatKind.TableSessionSnapshot, payload);
                }

                if (root.TryGetProperty("Scenario", out _))
                {
                    var payload = _scenarioSerializer.DeserializeSnapshot(json);
                    return new SnapshotImportResult(SnapshotFormatKind.ScenarioSnapshot, payload);
                }

                if (root.TryGetProperty("Manifest", out _)
                    && root.TryGetProperty("Definitions", out _)
                    && root.TryGetProperty("Assets", out _))
                {
                    var payload = _contentPackSerializer.DeserializeSnapshot(json);
                    return new SnapshotImportResult(SnapshotFormatKind.ContentPackSnapshot, payload);
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
