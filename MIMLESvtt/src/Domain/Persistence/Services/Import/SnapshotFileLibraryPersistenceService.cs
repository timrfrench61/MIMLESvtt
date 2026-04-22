using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.Services.Import
{
    public class SnapshotFileLibraryPersistenceService
    {
        private readonly SnapshotFileLibraryService _libraryService;

        public SnapshotFileLibraryPersistenceService(SnapshotFileLibraryService libraryService)
        {
            _libraryService = libraryService ?? throw new ArgumentNullException(nameof(libraryService));
        }

        public void SaveLibrary(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var document = new SnapshotFileLibraryDocument
            {
                KnownPaths = _libraryService.GetKnownPaths().ToList()
            };

            var json = JsonSerializer.Serialize(document);
            File.WriteAllText(path, json);
        }

        public void LoadLibrary(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Snapshot file library storage file was not found.", path);
            }

            var json = File.ReadAllText(path);
            var document = JsonSerializer.Deserialize<SnapshotFileLibraryDocument>(json);
            if (document is null || document.KnownPaths is null)
            {
                throw new InvalidOperationException("Failed to load snapshot file library paths.");
            }

            _libraryService.ReplaceKnownPaths(document.KnownPaths);
            _libraryService.RefreshAll();
        }

        private class SnapshotFileLibraryDocument
        {
            public List<string> KnownPaths { get; set; } = [];
        }
    }
}
