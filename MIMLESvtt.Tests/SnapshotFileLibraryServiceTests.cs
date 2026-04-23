using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;
using MIMLESvtt.src.Domain.Persistence.Services.Import;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotFileLibraryServiceTests
{
    [TestMethod]
    public void SnapshotFileLibrary_AddPath_WithSupportedSnapshotFile_AddsEntry()
    {
        var library = new SnapshotFileLibraryService();
        var path = CreateTempFilePath("library-add", SnapshotFileExtensions.TableSession);

        try
        {
            File.WriteAllText(path, "{}", Encoding.UTF8);

            library.AddPath(path);
            var entries = library.ListEntries();

            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual(Path.GetFullPath(path), entries[0].FullPath);
            Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, entries[0].DetectedFormatKind);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void KnownGameSessionRegistry_LoadRegistry_WhenMainMalformed_UsesBackup()
    {
        var workflow = new SnapshotFileImportApplyWorkflowService();
        var library = new SnapshotFileLibraryService(workflow);
        var persistence = new KnownGameSessionRegistryPersistenceService(library);

        var sessionPath = CreateTempFilePath("known-registry-backup-session", SnapshotFileExtensions.TableSession);
        var registryPath = CreateTempFilePath("known-registry-backup-store", ".known-sessions.json");
        var backupPath = registryPath + ".bak";

        try
        {
            File.WriteAllText(sessionPath, "{}", Encoding.UTF8);
            library.AddPath(sessionPath);

            var fullSessionPath = Path.GetFullPath(sessionPath);
            persistence.SaveRegistry(registryPath, new Dictionary<string, KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord>(StringComparer.OrdinalIgnoreCase)
            {
                [fullSessionPath] = new KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord
                {
                    JoinCode = "ALPHA"
                }
            });

            persistence.SaveRegistry(registryPath, new Dictionary<string, KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord>(StringComparer.OrdinalIgnoreCase)
            {
                [fullSessionPath] = new KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord
                {
                    JoinCode = "BETA"
                }
            });

            File.WriteAllText(registryPath, "{\"Version\":1,\"Entries\":", Encoding.UTF8);

            var reloadedLibrary = new SnapshotFileLibraryService(workflow);
            var reloadedPersistence = new KnownGameSessionRegistryPersistenceService(reloadedLibrary);

            var loaded = reloadedPersistence.LoadRegistry(registryPath);
            Assert.AreEqual(1, loaded.Count);
            Assert.AreEqual("ALPHA", loaded[fullSessionPath].JoinCode);
            Assert.IsTrue(File.Exists(backupPath));
        }
        finally
        {
            DeleteFileIfExists(sessionPath);
            DeleteFileIfExists(registryPath);
            DeleteFileIfExists(backupPath);
            DeleteFileIfExists(registryPath + ".tmp");
        }
    }

    [TestMethod]
    public void KnownGameSessionRegistry_LoadRegistry_WhenMainAndBackupMalformed_FailsClearly()
    {
        var workflow = new SnapshotFileImportApplyWorkflowService();
        var library = new SnapshotFileLibraryService(workflow);
        var persistence = new KnownGameSessionRegistryPersistenceService(library);

        var registryPath = CreateTempFilePath("known-registry-corrupt-store", ".known-sessions.json");
        var backupPath = registryPath + ".bak";

        try
        {
            File.WriteAllText(registryPath, "{\"Version\":1,\"Entries\":", Encoding.UTF8);
            File.WriteAllText(backupPath, "{\"Version\":1,\"Entries\":", Encoding.UTF8);

            var ex = Assert.ThrowsException<InvalidOperationException>(() => persistence.LoadRegistry(registryPath));
            StringAssert.Contains(ex.Message, "could not be loaded");
        }
        finally
        {
            DeleteFileIfExists(registryPath);
            DeleteFileIfExists(backupPath);
            DeleteFileIfExists(registryPath + ".tmp");
        }
    }

    [TestMethod]
    public void KnownGameSessionRegistry_SaveRegistry_WhenUpdatingExistingFile_CreatesBackupOfPreviousVersion()
    {
        var workflow = new SnapshotFileImportApplyWorkflowService();
        var library = new SnapshotFileLibraryService(workflow);
        var persistence = new KnownGameSessionRegistryPersistenceService(library);

        var sessionPath = CreateTempFilePath("known-registry-save-session", SnapshotFileExtensions.TableSession);
        var registryPath = CreateTempFilePath("known-registry-save-store", ".known-sessions.json");
        var backupPath = registryPath + ".bak";

        try
        {
            File.WriteAllText(sessionPath, "{}", Encoding.UTF8);
            library.AddPath(sessionPath);

            var fullSessionPath = Path.GetFullPath(sessionPath);

            persistence.SaveRegistry(registryPath, new Dictionary<string, KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord>(StringComparer.OrdinalIgnoreCase)
            {
                [fullSessionPath] = new KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord
                {
                    JoinCode = "FIRST"
                }
            });

            persistence.SaveRegistry(registryPath, new Dictionary<string, KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord>(StringComparer.OrdinalIgnoreCase)
            {
                [fullSessionPath] = new KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord
                {
                    JoinCode = "SECOND"
                }
            });

            Assert.IsTrue(File.Exists(backupPath));

            var backupJson = File.ReadAllText(backupPath, Encoding.UTF8);
            StringAssert.Contains(backupJson, "FIRST");

            var currentJson = File.ReadAllText(registryPath, Encoding.UTF8);
            StringAssert.Contains(currentJson, "SECOND");
        }
        finally
        {
            DeleteFileIfExists(sessionPath);
            DeleteFileIfExists(registryPath);
            DeleteFileIfExists(backupPath);
            DeleteFileIfExists(registryPath + ".tmp");
        }
    }

    [TestMethod]
    public void SnapshotFileLibrary_AddPath_WithDuplicateNormalizedPath_DoesNotCreateDuplicate()
    {
        var library = new SnapshotFileLibraryService();
        var path = CreateTempFilePath("library-duplicate", SnapshotFileExtensions.Scenario);

        try
        {
            File.WriteAllText(path, "{}", Encoding.UTF8);

            library.AddPath(path);
            library.AddPath(Path.GetFullPath(path));

            var entries = library.ListEntries();
            Assert.AreEqual(1, entries.Count);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileLibrary_AddPath_WithUnsupportedExtension_FailsClearly()
    {
        var library = new SnapshotFileLibraryService();
        var path = CreateTempFilePath("library-unsupported", ".json");

        var exception = Assert.ThrowsException<ArgumentException>(() => library.AddPath(path));
        StringAssert.Contains(exception.Message, "supported snapshot file extension");
    }

    [TestMethod]
    public void SnapshotFileLibrary_RemovePath_RemovesEntry()
    {
        var library = new SnapshotFileLibraryService();
        var path = CreateTempFilePath("library-remove", SnapshotFileExtensions.ActionLog);

        try
        {
            File.WriteAllText(path, "{}", Encoding.UTF8);

            library.AddPath(path);
            library.RemovePath(path);

            Assert.AreEqual(0, library.ListEntries().Count);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileLibrary_RefreshEntry_UpdatesExistsAndTimestampMetadata()
    {
        var library = new SnapshotFileLibraryService();
        var path = CreateTempFilePath("library-refresh", SnapshotFileExtensions.ContentPack);

        try
        {
            File.WriteAllText(path, "{}", Encoding.UTF8);
            library.AddPath(path);

            var before = library.ListEntries().Single();
            Assert.IsTrue(before.Exists);
            Assert.IsNotNull(before.LastWriteTimeUtc);

            DeleteFileIfExists(path);
            library.RefreshEntry(path);

            var after = library.ListEntries().Single();
            Assert.IsFalse(after.Exists);
            Assert.IsNull(after.LastWriteTimeUtc);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileLibrary_ListEntries_ReturnsKnownEntriesWithDetectedFormats()
    {
        var library = new SnapshotFileLibraryService();
        var sessionPath = CreateTempFilePath("library-list-session", SnapshotFileExtensions.TableSession);
        var scenarioPath = CreateTempFilePath("library-list-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            File.WriteAllText(sessionPath, "{}", Encoding.UTF8);
            File.WriteAllText(scenarioPath, "{}", Encoding.UTF8);

            library.AddPath(sessionPath);
            library.AddPath(scenarioPath);

            var entries = library.ListEntries();
            Assert.AreEqual(2, entries.Count);
            CollectionAssert.AreEquivalent(
                new List<SnapshotFormatKind>
                {
                    SnapshotFormatKind.TableSessionSnapshot,
                    SnapshotFormatKind.ScenarioSnapshot
                },
                entries.Select(e => e.DetectedFormatKind).ToList());
        }
        finally
        {
            DeleteFileIfExists(sessionPath);
            DeleteFileIfExists(scenarioPath);
        }
    }

    [TestMethod]
    public void SnapshotFileLibrary_RunImportApply_ForKnownTableSessionEntry_DelegatesAndReturnsResponse()
    {
        var library = new SnapshotFileLibraryService();
        var serializer = new TableSessionSnapshotSerializer();
        var path = CreateTempFilePath("library-run-session", SnapshotFileExtensions.TableSession);

        try
        {
            var session = new TableSession { Id = "imported-session", Title = "Imported" };
            File.WriteAllText(path, serializer.Save(session), Encoding.UTF8);
            library.AddPath(path);

            var context = new SnapshotImportApplyContext
            {
                CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
            };

            var response = library.RunImportApply(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, response.DetectedFormat);
            Assert.IsTrue(response.IsRuntimeStateMutated);
            Assert.AreEqual("imported-session", context.CurrentTableSession!.Id);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileLibrary_RunImportApply_ForKnownScenarioEntry_DelegatesAndReturnsResponse()
    {
        var library = new SnapshotFileLibraryService();
        var serializer = new ScenarioSnapshotSerializer();
        var path = CreateTempFilePath("library-run-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            File.WriteAllText(path, serializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);
            library.AddPath(path);

            var current = new TableSession { Id = "current-session", Title = "Current" };
            var context = new SnapshotImportApplyContext { CurrentTableSession = current };

            var response = library.RunImportApply(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, response.DetectedFormat);
            Assert.IsFalse(response.IsRuntimeStateMutated);
            Assert.AreSame(current, context.CurrentTableSession);
            Assert.IsNotNull(response.ScenarioActivationResponse);
            Assert.IsTrue(response.ScenarioActivationResponse!.PendingScenarioPlanCreated);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileLibrary_RunImportApply_WhenFileMissing_FailsClearly()
    {
        var library = new SnapshotFileLibraryService();
        var path = CreateTempFilePath("library-run-missing", SnapshotFileExtensions.TableSession);

        library.AddPath(path);

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var exception = Assert.ThrowsException<FileNotFoundException>(() =>
            library.RunImportApply(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default));

        StringAssert.Contains(exception.Message, "not found");
    }

    [TestMethod]
    public void SnapshotFileLibraryPersistence_SaveAndLoad_RoundTripsKnownPaths()
    {
        var workflow = new SnapshotFileImportApplyWorkflowService();
        var library = new SnapshotFileLibraryService(workflow);
        var persistence = new SnapshotFileLibraryPersistenceService(library);

        var sessionPath = CreateTempFilePath("library-persist-session", SnapshotFileExtensions.TableSession);
        var scenarioPath = CreateTempFilePath("library-persist-scenario", SnapshotFileExtensions.Scenario);
        var storePath = CreateTempFilePath("library-persist-store", ".library.json");

        try
        {
            File.WriteAllText(sessionPath, "{}", Encoding.UTF8);
            File.WriteAllText(scenarioPath, "{}", Encoding.UTF8);

            library.AddPath(sessionPath);
            library.AddPath(scenarioPath);

            persistence.SaveLibrary(storePath);

            var reloadedLibrary = new SnapshotFileLibraryService(workflow);
            var reloadedPersistence = new SnapshotFileLibraryPersistenceService(reloadedLibrary);
            reloadedPersistence.LoadLibrary(storePath);

            var entries = reloadedLibrary.ListEntries();
            Assert.AreEqual(2, entries.Count);
            CollectionAssert.AreEquivalent(
                new List<string> { Path.GetFullPath(sessionPath), Path.GetFullPath(scenarioPath) },
                entries.Select(e => e.FullPath).ToList());
        }
        finally
        {
            DeleteFileIfExists(sessionPath);
            DeleteFileIfExists(scenarioPath);
            DeleteFileIfExists(storePath);
        }
    }

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Scenario",
            Surfaces =
            [
                new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" }
            ],
            Pieces =
            [
                new PieceInstance
                {
                    Id = "piece-1",
                    DefinitionId = "def-piece-1",
                    Location = new Location
                    {
                        SurfaceId = "surface-1",
                        Coordinate = new Coordinate { X = 2, Y = 3 }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = false
            }
        };
    }

    private static string CreateTempFilePath(string prefix, string extension)
    {
        return Path.Combine(Path.GetTempPath(), $"{prefix}-{Guid.NewGuid():N}{extension}");
    }

    private static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
