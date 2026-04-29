using MIMLESvtt.src.Domain.Persistence.Snapshot;
using System.Text;
using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class KnownGameSessionRegistryPersistenceService
    {
        private const int CurrentVersion = 1;
        private const string DefaultCampaignBrowserViewMode = "DetailList";
        private readonly SnapshotFileLibraryService _libraryService;

        public sealed class KnownGameSessionRegistryRecord
        {
            public string JoinCode { get; set; } = string.Empty;

            public DateTime? LastJoinCodeUpdatedUtc { get; set; }

            public DateTime? LastJoinedUtc { get; set; }
        }

        public KnownGameSessionRegistryPersistenceService(SnapshotFileLibraryService libraryService)
        {
            _libraryService = libraryService ?? throw new ArgumentNullException(nameof(libraryService));
        }

        public Dictionary<string, KnownGameSessionRegistryRecord> LoadRegistry(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                return new Dictionary<string, KnownGameSessionRegistryRecord>(StringComparer.OrdinalIgnoreCase);
            }

            KnownGameSessionRegistryDocument document;
            try
            {
                document = LoadDocumentCore(fullPath);
            }
            catch (InvalidOperationException mainException)
            {
                var backupPath = GetBackupPath(fullPath);
                if (!File.Exists(backupPath))
                {
                    throw;
                }

                try
                {
                    document = LoadDocumentCore(backupPath);
                }
                catch (Exception backupException)
                {
                    throw new InvalidOperationException("Known game session registry and backup are invalid and could not be loaded.", new AggregateException(mainException, backupException));
                }
            }

            var registryByPath = new Dictionary<string, KnownGameSessionRegistryRecord>(StringComparer.OrdinalIgnoreCase);

            var normalizedPaths = new List<string>();
            if (document.Entries is not null && document.Entries.Count > 0)
            {
                foreach (var entry in document.Entries)
                {
                    if (string.IsNullOrWhiteSpace(entry.SessionPath))
                    {
                        continue;
                    }

                    var normalizedPath = Path.GetFullPath(entry.SessionPath);
                    if (!normalizedPath.EndsWith(SnapshotFileExtensions.VttSession, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    normalizedPaths.Add(normalizedPath);

                    var joinCode = NormalizeJoinCode(entry.JoinCode, Path.GetFileName(normalizedPath));
                    registryByPath[normalizedPath] = new KnownGameSessionRegistryRecord
                    {
                        JoinCode = joinCode,
                        LastJoinCodeUpdatedUtc = entry.LastJoinCodeUpdatedUtc,
                        LastJoinedUtc = entry.LastJoinedUtc
                    };
                }
            }
            else
            {
                foreach (var legacyPath in document.KnownPaths ?? [])
                {
                    if (string.IsNullOrWhiteSpace(legacyPath))
                    {
                        continue;
                    }

                    var normalizedPath = Path.GetFullPath(legacyPath);
                    if (!normalizedPath.EndsWith(SnapshotFileExtensions.VttSession, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    normalizedPaths.Add(normalizedPath);
                    registryByPath[normalizedPath] = new KnownGameSessionRegistryRecord
                    {
                        JoinCode = NormalizeJoinCode(null, Path.GetFileName(normalizedPath))
                    };
                }
            }

            _libraryService.ReplaceKnownPaths(normalizedPaths);
            _libraryService.RefreshAll();

            return registryByPath;
        }

        public string LoadCampaignBrowserViewMode(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                return DefaultCampaignBrowserViewMode;
            }

            KnownGameSessionRegistryDocument document;
            try
            {
                document = LoadDocumentCore(fullPath);
            }
            catch (InvalidOperationException mainException)
            {
                var backupPath = GetBackupPath(fullPath);
                if (!File.Exists(backupPath))
                {
                    throw;
                }

                try
                {
                    document = LoadDocumentCore(backupPath);
                }
                catch (Exception backupException)
                {
                    throw new InvalidOperationException("Known game session registry and backup are invalid and could not be loaded.", new AggregateException(mainException, backupException));
                }
            }

            return NormalizeCampaignBrowserViewMode(document.CampaignBrowserViewMode);
        }

        public IReadOnlyList<string> LoadHiddenReadonlyCampaignIds(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                return [];
            }

            KnownGameSessionRegistryDocument document;
            try
            {
                document = LoadDocumentCore(fullPath);
            }
            catch (InvalidOperationException mainException)
            {
                var backupPath = GetBackupPath(fullPath);
                if (!File.Exists(backupPath))
                {
                    throw;
                }

                try
                {
                    document = LoadDocumentCore(backupPath);
                }
                catch (Exception backupException)
                {
                    throw new InvalidOperationException("Known game session registry and backup are invalid and could not be loaded.", new AggregateException(mainException, backupException));
                }
            }

            return (document.HiddenReadonlyCampaignIds ?? [])
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public void SaveRegistry(
            string path,
            IReadOnlyDictionary<string, KnownGameSessionRegistryRecord> registryByPath,
            string campaignBrowserViewMode,
            IReadOnlyCollection<string>? hiddenReadonlyCampaignIds = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            ArgumentNullException.ThrowIfNull(registryByPath);

            var fullPath = Path.GetFullPath(path);
            var directoryPath = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var entries = _libraryService
                .ListEntries()
                .Where(entry => entry.DetectedFormatKind == SnapshotFormatKind.VttSessionSnapshot)
                .Select(entry =>
                {
                    var registryRecord = registryByPath.TryGetValue(entry.FullPath, out var persistedRecord)
                        ? persistedRecord
                        : null;

                    var joinCode = NormalizeJoinCode(registryRecord?.JoinCode, entry.FileName);

                    return new KnownGameSessionRegistryEntry
                    {
                        SessionPath = entry.FullPath,
                        JoinCode = joinCode,
                        LastWriteTimeUtc = entry.LastWriteTimeUtc,
                        LastJoinCodeUpdatedUtc = registryRecord?.LastJoinCodeUpdatedUtc,
                        LastJoinedUtc = registryRecord?.LastJoinedUtc
                    };
                })
                .OrderBy(entry => entry.SessionPath, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var document = new KnownGameSessionRegistryDocument
            {
                Version = CurrentVersion,
                Entries = entries,
                KnownPaths = entries.Select(e => e.SessionPath).ToList(),
                CampaignBrowserViewMode = NormalizeCampaignBrowserViewMode(campaignBrowserViewMode),
                HiddenReadonlyCampaignIds = (hiddenReadonlyCampaignIds ?? [])
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                    .ToList()
            };

            var json = JsonSerializer.Serialize(document);

            var tempPath = GetTempPath(fullPath);
            var backupPath = GetBackupPath(fullPath);

            File.WriteAllText(tempPath, json, Encoding.UTF8);
            if (File.Exists(fullPath))
            {
                File.Replace(tempPath, fullPath, backupPath, ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(tempPath, fullPath);
            }

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }

        private static KnownGameSessionRegistryDocument LoadDocumentCore(string path)
        {
            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                var document = JsonSerializer.Deserialize<KnownGameSessionRegistryDocument>(json);
                if (document is null)
                {
                    throw new InvalidOperationException("Failed to load known game session registry.");
                }

                return document;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Known game session registry contains malformed JSON.", ex);
            }
        }

        private static string GetBackupPath(string fullPath)
        {
            return fullPath + ".bak";
        }

        private static string GetTempPath(string fullPath)
        {
            return fullPath + ".tmp";
        }

        private static string NormalizeJoinCode(string? joinCode, string fileName)
        {
            var candidate = string.IsNullOrWhiteSpace(joinCode)
                ? BuildDefaultJoinCode(fileName)
                : joinCode.Trim();

            return candidate.ToUpperInvariant();
        }

        private static string BuildDefaultJoinCode(string fileName)
        {
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrWhiteSpace(baseName))
            {
                return fileName;
            }

            var withoutSnapshotSuffix = baseName.EndsWith(".vttsession", StringComparison.OrdinalIgnoreCase)
                ? baseName[..^".vttsession".Length]
                : baseName;

            return string.IsNullOrWhiteSpace(withoutSnapshotSuffix) ? fileName : withoutSnapshotSuffix;
        }

        private static string NormalizeCampaignBrowserViewMode(string? mode)
        {
            if (string.Equals(mode, "Card", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mode, "CardGrid", StringComparison.OrdinalIgnoreCase))
            {
                return "Card";
            }

            return DefaultCampaignBrowserViewMode;
        }

        private sealed class KnownGameSessionRegistryDocument
        {
            public int Version { get; set; } = CurrentVersion;

            public List<KnownGameSessionRegistryEntry> Entries { get; set; } = [];

            public List<string> KnownPaths { get; set; } = [];

            public string CampaignBrowserViewMode { get; set; } = DefaultCampaignBrowserViewMode;

            public List<string> HiddenReadonlyCampaignIds { get; set; } = [];
        }

        private sealed class KnownGameSessionRegistryEntry
        {
            public string SessionPath { get; set; } = string.Empty;

            public string JoinCode { get; set; } = string.Empty;

            public DateTime? LastWriteTimeUtc { get; set; }

            public DateTime? LastJoinCodeUpdatedUtc { get; set; }

            public DateTime? LastJoinedUtc { get; set; }
        }
    }
}
