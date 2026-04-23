using MIMLESvtt.src.Domain.Persistence.Snapshot;
using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class KnownGameSessionRegistryPersistenceService
    {
        private const int CurrentVersion = 1;
        private readonly SnapshotFileLibraryService _libraryService;

        public KnownGameSessionRegistryPersistenceService(SnapshotFileLibraryService libraryService)
        {
            _libraryService = libraryService ?? throw new ArgumentNullException(nameof(libraryService));
        }

        public Dictionary<string, string> LoadRegistry(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var json = File.ReadAllText(fullPath);
            var document = JsonSerializer.Deserialize<KnownGameSessionRegistryDocument>(json);
            if (document is null)
            {
                throw new InvalidOperationException("Failed to load known game session registry.");
            }

            var joinCodeByPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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
                    if (!string.IsNullOrWhiteSpace(joinCode))
                    {
                        joinCodeByPath[normalizedPath] = joinCode;
                    }
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
                    joinCodeByPath[normalizedPath] = NormalizeJoinCode(null, Path.GetFileName(normalizedPath));
                }
            }

            _libraryService.ReplaceKnownPaths(normalizedPaths);
            _libraryService.RefreshAll();

            return joinCodeByPath;
        }

        public void SaveRegistry(string path, IReadOnlyDictionary<string, string> joinCodeByPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            ArgumentNullException.ThrowIfNull(joinCodeByPath);

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
                    var joinCode = joinCodeByPath.TryGetValue(entry.FullPath, out var persistedJoinCode)
                        ? NormalizeJoinCode(persistedJoinCode, entry.FileName)
                        : NormalizeJoinCode(null, entry.FileName);

                    return new KnownGameSessionRegistryEntry
                    {
                        SessionPath = entry.FullPath,
                        JoinCode = joinCode,
                        LastWriteTimeUtc = entry.LastWriteTimeUtc
                    };
                })
                .OrderBy(entry => entry.SessionPath, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var document = new KnownGameSessionRegistryDocument
            {
                Version = CurrentVersion,
                Entries = entries,
                KnownPaths = entries.Select(e => e.SessionPath).ToList()
            };

            var json = JsonSerializer.Serialize(document);
            File.WriteAllText(fullPath, json);
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

        private sealed class KnownGameSessionRegistryDocument
        {
            public int Version { get; set; } = CurrentVersion;

            public List<KnownGameSessionRegistryEntry> Entries { get; set; } = [];

            public List<string> KnownPaths { get; set; } = [];
        }

        private sealed class KnownGameSessionRegistryEntry
        {
            public string SessionPath { get; set; } = string.Empty;

            public string JoinCode { get; set; } = string.Empty;

            public DateTime? LastWriteTimeUtc { get; set; }
        }
    }
}
