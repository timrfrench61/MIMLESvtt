using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class SnapshotFileLibraryService
    {
        private readonly SnapshotFileImportApplyWorkflowService _fileImportApplyWorkflowService;
        private readonly Dictionary<string, SnapshotFileDescriptor> _entries = new(StringComparer.OrdinalIgnoreCase);

        public SnapshotFileLibraryService()
            : this(new SnapshotFileImportApplyWorkflowService())
        {
        }

        public SnapshotFileLibraryService(SnapshotFileImportApplyWorkflowService fileImportApplyWorkflowService)
        {
            _fileImportApplyWorkflowService = fileImportApplyWorkflowService ?? throw new ArgumentNullException(nameof(fileImportApplyWorkflowService));
        }

        public void AddPath(string path)
        {
            var fullPath = NormalizePath(path);
            var format = DetectFormatFromPath(fullPath);

            var descriptor = BuildDescriptor(fullPath, format);
            _entries[fullPath] = descriptor;
        }

        public void RemovePath(string path)
        {
            var fullPath = NormalizePath(path);
            _entries.Remove(fullPath);
        }

        public IReadOnlyList<SnapshotFileDescriptor> ListEntries()
        {
            return _entries.Values.OrderBy(e => e.FullPath, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public void RefreshEntry(string path)
        {
            var fullPath = NormalizePath(path);

            if (!_entries.ContainsKey(fullPath))
            {
                throw new InvalidOperationException("Path is not present in the snapshot file library.");
            }

            var format = DetectFormatFromPath(fullPath);
            _entries[fullPath] = BuildDescriptor(fullPath, format);
        }

        public void RefreshAll()
        {
            var paths = _entries.Keys.ToList();
            foreach (var path in paths)
            {
                var format = DetectFormatFromPath(path);
                _entries[path] = BuildDescriptor(path, format);
            }
        }

        public SnapshotFileImportApplyResponse RunImportApply(
            string path,
            SnapshotImportApplyContext context,
            VttScenarioCandidateActivationMode scenarioActivationMode,
            SnapshotImportApplyPolicy policy)
        {
            var fullPath = NormalizePath(path);

            if (!_entries.TryGetValue(fullPath, out var descriptor))
            {
                throw new InvalidOperationException("Path is not present in the snapshot file library.");
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Snapshot file for known entry was not found.", fullPath);
            }

            return _fileImportApplyWorkflowService.ImportAndApplyFromFile(fullPath, context, scenarioActivationMode, policy);
        }

        internal IReadOnlyList<string> GetKnownPaths()
        {
            return _entries.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
        }

        internal void ReplaceKnownPaths(IEnumerable<string> paths)
        {
            ArgumentNullException.ThrowIfNull(paths);

            _entries.Clear();
            foreach (var path in paths)
            {
                AddPath(path);
            }
        }

        private static SnapshotFileDescriptor BuildDescriptor(string fullPath, SnapshotFormatKind format)
        {
            var exists = File.Exists(fullPath);
            DateTime? lastWriteTimeUtc = exists ? File.GetLastWriteTimeUtc(fullPath) : null;

            return new SnapshotFileDescriptor
            {
                FullPath = fullPath,
                FileName = Path.GetFileName(fullPath),
                Extension = GetExtensionForFormat(format),
                DetectedFormatKind = format,
                LastWriteTimeUtc = lastWriteTimeUtc,
                Exists = exists
            };
        }

        private static string GetExtensionForFormat(SnapshotFormatKind format)
        {
            return format switch
            {
                SnapshotFormatKind.VttSessionSnapshot => SnapshotFileExtensions.VttSession,
                SnapshotFormatKind.VttScenarioSnapshot => SnapshotFileExtensions.VttScenario,
                SnapshotFormatKind.VttContentPackSnapshot => SnapshotFileExtensions.VttContentPack,
                SnapshotFormatKind.ActionLogSnapshot => SnapshotFileExtensions.ActionLog,
                _ => string.Empty
            };
        }

        private static string NormalizePath(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            return Path.GetFullPath(path);
        }

        private static SnapshotFormatKind DetectFormatFromPath(string path)
        {
            if (path.EndsWith(SnapshotFileExtensions.VttSession, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.VttSessionSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.VttScenario, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.VttScenarioSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.VttContentPack, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.VttContentPackSnapshot;
            }

            if (path.EndsWith(SnapshotFileExtensions.ActionLog, StringComparison.OrdinalIgnoreCase))
            {
                return SnapshotFormatKind.ActionLogSnapshot;
            }

            throw new ArgumentException("Path does not use a supported snapshot file extension.", nameof(path));
        }
    }
}
