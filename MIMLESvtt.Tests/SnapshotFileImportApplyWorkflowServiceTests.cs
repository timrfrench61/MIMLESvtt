using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotFileImportApplyWorkflowServiceTests
{
    [TestMethod]
    public void FileImportWorkflow_WithTableSessionFile_ReplacesCurrentTableSessionWhenAllowed()
    {
        var service = new SnapshotFileImportApplyWorkflowService();
        var serializer = new TableSessionSnapshotSerializer();
        var path = CreateTempFilePath("workflow-import-session", SnapshotFileExtensions.TableSession);

        try
        {
            var imported = new TableSession { Id = "imported-session", Title = "Imported Session" };
            File.WriteAllText(path, serializer.Save(imported), Encoding.UTF8);

            var context = new SnapshotImportApplyContext
            {
                CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
            };

            var response = service.ImportAndApplyFromFile(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, response.DetectedFormat);
            Assert.IsTrue(response.IsRuntimeStateMutated);
            Assert.IsNotNull(response.TableSessionApplyResponse);
            Assert.AreEqual("imported-session", context.CurrentTableSession!.Id);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void FileImportWorkflow_WithScenarioFile_DryRun_ReturnsPlanCandidateAndDoesNotMutateContext()
    {
        var service = new SnapshotFileImportApplyWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var path = CreateTempFilePath("workflow-import-scenario-dry", SnapshotFileExtensions.Scenario);

        try
        {
            File.WriteAllText(path, serializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            var current = new TableSession { Id = "current-session", Title = "Current" };
            var context = new SnapshotImportApplyContext { CurrentTableSession = current };

            var response = service.ImportAndApplyFromFile(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, response.DetectedFormat);
            Assert.IsFalse(response.IsRuntimeStateMutated);
            Assert.IsNotNull(response.ScenarioActivationResponse);
            Assert.IsTrue(response.ScenarioActivationResponse!.PendingScenarioPlanCreated);
            Assert.IsTrue(response.ScenarioActivationResponse.CandidateCreated);
            Assert.AreSame(current, context.CurrentTableSession);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void FileImportWorkflow_WithScenarioFile_Activate_ReplacesCurrentTableSessionWhenPolicyAllows()
    {
        var service = new SnapshotFileImportApplyWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var path = CreateTempFilePath("workflow-import-scenario-activate", SnapshotFileExtensions.Scenario);

        try
        {
            File.WriteAllText(path, serializer.SerializeScenario(CreateScenarioFixture()), Encoding.UTF8);

            var context = new SnapshotImportApplyContext
            {
                CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
            };

            var policy = new SnapshotImportApplyPolicy
            {
                AllowCreateScenarioFromImport = true,
                AllowActivateScenarioCandidate = true
            };

            var response = service.ImportAndApplyFromFile(path, context, ScenarioCandidateActivationMode.Activate, policy);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, response.DetectedFormat);
            Assert.IsTrue(response.IsRuntimeStateMutated);
            Assert.IsNotNull(response.ScenarioActivationResponse);
            Assert.AreEqual("Imported Scenario", context.CurrentTableSession!.Title);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void FileImportWorkflow_WithContentPackFile_ReturnsUnsupportedOrNonAppliedResponse()
    {
        var service = new SnapshotFileImportApplyWorkflowService();
        var serializer = new ContentPackSnapshotSerializer();
        var path = CreateTempFilePath("workflow-import-contentpack", SnapshotFileExtensions.ContentPack);

        try
        {
            var contentPack = new ContentPackSnapshot
            {
                Version = ContentPackSnapshotSerializer.CurrentVersion,
                Manifest = new ContentPackManifest { Name = "Pack", Description = "Desc" },
                Definitions = [],
                Assets = []
            };

            File.WriteAllText(path, serializer.SerializeContentPack(contentPack), Encoding.UTF8);

            var context = new SnapshotImportApplyContext { CurrentTableSession = new TableSession { Id = "current-session" } };

            var response = service.ImportAndApplyFromFile(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(SnapshotFormatKind.ContentPackSnapshot, response.DetectedFormat);
            Assert.IsFalse(response.IsRuntimeStateMutated);
            Assert.IsNotNull(response.ImportOutcome);
            Assert.IsFalse(response.ImportOutcome!.IsSupported);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void FileImportWorkflow_WithActionLogFile_ReturnsUnsupportedOrNonAppliedResponse()
    {
        var service = new SnapshotFileImportApplyWorkflowService();
        var serializer = new ActionLogSnapshotSerializer();
        var path = CreateTempFilePath("workflow-import-actionlog", SnapshotFileExtensions.ActionLog);

        try
        {
            var actionLog = new ActionLogSnapshot
            {
                Version = ActionLogSnapshotSerializer.CurrentVersion,
                SessionId = "session-1",
                Actions = []
            };

            File.WriteAllText(path, serializer.SerializeActionLog(actionLog), Encoding.UTF8);

            var context = new SnapshotImportApplyContext { CurrentTableSession = new TableSession { Id = "current-session" } };

            var response = service.ImportAndApplyFromFile(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(SnapshotFormatKind.ActionLogSnapshot, response.DetectedFormat);
            Assert.IsFalse(response.IsRuntimeStateMutated);
            Assert.IsNotNull(response.ImportOutcome);
            Assert.IsFalse(response.ImportOutcome!.IsSupported);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void FileImportWorkflow_WithUnsupportedExtension_ReturnsFailureResponse()
    {
        var service = new SnapshotFileImportApplyWorkflowService();
        var path = CreateTempFilePath("workflow-import-unsupported", ".json");
        var context = new SnapshotImportApplyContext { CurrentTableSession = new TableSession { Id = "current-session" } };

        var response = service.ImportAndApplyFromFile(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.AreEqual(SnapshotImportErrorCode.InvalidInput, response.ErrorCode);
    }

    [TestMethod]
    public void FileImportWorkflow_WithMissingFile_ReturnsFailureResponse()
    {
        var service = new SnapshotFileImportApplyWorkflowService();
        var path = CreateTempFilePath("workflow-import-missing", SnapshotFileExtensions.TableSession);
        var context = new SnapshotImportApplyContext { CurrentTableSession = new TableSession { Id = "current-session" } };

        var response = service.ImportAndApplyFromFile(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.AreEqual(SnapshotImportErrorCode.ValidationFailure, response.ErrorCode);
    }

    [TestMethod]
    public void FileImportWorkflow_WithMalformedFile_ReturnsFailureResponse()
    {
        var service = new SnapshotFileImportApplyWorkflowService();
        var path = CreateTempFilePath("workflow-import-malformed", SnapshotFileExtensions.Scenario);

        try
        {
            File.WriteAllText(path, "{\"Version\":1,\"Scenario\":", Encoding.UTF8);
            var context = new SnapshotImportApplyContext { CurrentTableSession = new TableSession { Id = "current-session" } };

            var response = service.ImportAndApplyFromFile(path, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

            Assert.IsFalse(response.IsSuccess);
            Assert.IsFalse(response.IsSupported);
            Assert.AreEqual(SnapshotImportErrorCode.MalformedJson, response.ErrorCode);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Imported Scenario",
            Surfaces =
            [
                new SurfaceInstance
                {
                    Id = "surface-1",
                    DefinitionId = "def-surface-1"
                }
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
                        Coordinate = new Coordinate { X = 1, Y = 1 }
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
