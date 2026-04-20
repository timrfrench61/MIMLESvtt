using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportFacadeTests
{
    [TestMethod]
    public void ImportFacade_WithTableSessionJson_ReturnsSuccessResponseWithRequestId()
    {
        var facade = new SnapshotImportFacade();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new TableSessionSnapshot
        {
            Version = TableSessionSnapshotSerializer.CurrentVersion,
            TableSession = new TableSession
            {
                Id = "session-1",
                Title = "Session"
            }
        });

        var response = facade.Import(json);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(response.Outcome);
        Assert.IsNull(response.ErrorMessage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
        Assert.AreEqual(SnapshotImportErrorCode.None, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.None, response.FailureStage);
        Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, response.Outcome.FormatKind);
        Assert.IsTrue(response.Outcome.IsSupported);
    }

    [TestMethod]
    public void ImportFacade_WithScenarioJson_ReturnsSuccessResponseWithRequestId()
    {
        var facade = new SnapshotImportFacade();
        var serializer = new ScenarioSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ScenarioSnapshot
        {
            Version = ScenarioSnapshotSerializer.CurrentVersion,
            Scenario = new ScenarioExport
            {
                Title = "Scenario"
            }
        });

        var response = facade.Import(json);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(response.Outcome);
        Assert.IsNull(response.ErrorMessage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
        Assert.AreEqual(SnapshotImportErrorCode.None, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.None, response.FailureStage);
        Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, response.Outcome.FormatKind);
        Assert.IsTrue(response.Outcome.IsSupported);
    }

    [TestMethod]
    public void ImportFacade_WithContentPackJson_ReturnsSuccessfulResponseWithUnsupportedOutcome()
    {
        var facade = new SnapshotImportFacade();
        var serializer = new ContentPackSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ContentPackSnapshot
        {
            Version = ContentPackSnapshotSerializer.CurrentVersion,
            Manifest = new ContentPackManifest { Name = "Pack", Description = "Desc" },
            Definitions = [],
            Assets = []
        });

        var response = facade.Import(json);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsNotNull(response.Outcome);
        Assert.IsNull(response.ErrorMessage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
        Assert.AreEqual(SnapshotImportErrorCode.None, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.None, response.FailureStage);
        Assert.AreEqual(SnapshotFormatKind.ContentPackSnapshot, response.Outcome.FormatKind);
        Assert.IsFalse(response.Outcome.IsSupported);
    }

    [TestMethod]
    public void ImportFacade_WhenInputMalformed_ReturnsFailureResponseWithRequestId()
    {
        var facade = new SnapshotImportFacade();

        var response = facade.Import("{ \"Version\": 1, \"Scenario\":");

        Assert.IsFalse(response.IsSuccess);
        Assert.IsNull(response.Outcome);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
        Assert.AreEqual(SnapshotImportErrorCode.MalformedJson, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.Dispatch, response.FailureStage);
    }

    [TestMethod]
    public void ImportFacade_WhenFormatUnknown_ReturnsFailureResponseWithRequestId()
    {
        var facade = new SnapshotImportFacade();

        var response = facade.Import("{\"Version\":1,\"Unknown\":{}}");

        Assert.IsFalse(response.IsSuccess);
        Assert.IsNull(response.Outcome);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
        Assert.AreEqual(SnapshotImportErrorCode.UnknownFormat, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.Dispatch, response.FailureStage);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void ImportFacade_WhenInputNullEmptyOrWhitespace_ReturnsFailureResponseWithRequestId(string input)
    {
        var facade = new SnapshotImportFacade();

        var response = facade.Import(input);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsNull(response.Outcome);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
        Assert.AreEqual(SnapshotImportErrorCode.InvalidInput, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.FacadeInput, response.FailureStage);
    }

    [TestMethod]
    public void ImportFacade_WhenKnownFormatFailsValidation_ReturnsFormatValidationStage()
    {
        var facade = new SnapshotImportFacade();
        var invalidKnownFormatJson = "{\"Version\":1,\"Scenario\":null}";

        var response = facade.Import(invalidKnownFormatJson);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsNull(response.Outcome);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.AreEqual(SnapshotImportErrorCode.ValidationFailure, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.FormatValidation, response.FailureStage);
    }

    [TestMethod]
    public void ImportFacade_WithSeparateCalls_ReturnsDifferentRequestIds()
    {
        var facade = new SnapshotImportFacade();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new TableSessionSnapshot
        {
            Version = TableSessionSnapshotSerializer.CurrentVersion,
            TableSession = new TableSession
            {
                Id = "session-1",
                Title = "Session"
            }
        });

        var firstResponse = facade.Import(json);
        var secondResponse = facade.Import(json);

        Assert.IsTrue(firstResponse.IsSuccess);
        Assert.IsTrue(secondResponse.IsSuccess);
        Assert.IsFalse(string.IsNullOrWhiteSpace(firstResponse.RequestId));
        Assert.IsFalse(string.IsNullOrWhiteSpace(secondResponse.RequestId));
        Assert.AreNotEqual(firstResponse.RequestId, secondResponse.RequestId);
    }
}
