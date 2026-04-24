using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Import;
using MIMLESvtt.src.Domain.Persistence.VttGameboxNSPC;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;
using MIMLESvtt.src.Domain.Persistence.Workspace;

namespace MIMLESvtt.src;

public class TableSession : VttSession
{
}

public class ScenarioExport : VttScenario
{
}

public class TableOptions : TabletopOptions
{
}

public class ContentPackSnapshot : VttGameboxSnapshot
{
}

public class ContentPackManifest : Domain.Models.VttGamebox.VttGameboxManifest
{
}

public class ContentPackDefinition : Domain.Models.VttGamebox.VttGameboxDefinition
{
}

public class ContentPackAsset : Domain.Models.VttGamebox.VttGameboxAsset
{
}

public class ContentPackSnapshotSerializer : VttGameboxSnapshotSerializer
{
    public string SerializeContentPack(ContentPackSnapshot contentPack)
    {
        return SerializeVttGamebox(contentPack);
    }

    public ContentPackSnapshot DeserializeContentPack(string json)
    {
        var snapshot = DeserializeVttGamebox(json);
        return new ContentPackSnapshot
        {
            Version = snapshot.Version,
            Manifest = snapshot.Manifest,
            Definitions = snapshot.Definitions,
            Assets = snapshot.Assets
        };
    }
}

public class TableSessionSnapshotSerializer : VttSessionSnapshotSerializer
{
    public new TableSession Load(string json)
    {
        var session = base.Load(json);
        return ToTableSession(session);
    }

    public string Save(TableSession tableSession)
    {
        return base.Save(tableSession);
    }

    private static TableSession ToTableSession(VttSession session)
    {
        return new TableSession
        {
            Id = session.Id,
            Title = session.Title,
            Participants = session.Participants,
            PlayerSeats = session.PlayerSeats,
            Surfaces = session.Surfaces,
            Pieces = session.Pieces,
            TurnOrder = session.TurnOrder,
            CurrentTurnIndex = session.CurrentTurnIndex,
            TurnNumber = session.TurnNumber,
            CurrentPhase = session.CurrentPhase,
            TurnState = session.TurnState,
            Options = session.Options,
            Visibility = session.Visibility,
            ActionLog = session.ActionLog,
            ModuleState = session.ModuleState,
            Campaigns = session.Campaigns
        };
    }
}

public class ScenarioSnapshotSerializer : VttScenarioSnapshotSerializer
{
    public string SerializeScenario(ScenarioExport scenario)
    {
        return SerializeVttScenario(scenario);
    }

    public ScenarioExport DeserializeScenario(string json)
    {
        var scenario = DeserializeVttScenario(json);
        return new ScenarioExport
        {
            Title = scenario.Title,
            Surfaces = scenario.Surfaces,
            Pieces = scenario.Pieces,
            Options = scenario.Options
        };
    }
}

public class TableSessionPersistenceService
{
    private readonly TableSessionSnapshotSerializer _serializer = new();

    public string Save(TableSession session)
    {
        return _serializer.Save(session);
    }

    public TableSession Load(string json)
    {
        return _serializer.Load(json);
    }
}

public interface ITableSessionCommandService : IVttSessionCommandService
{
    TableSession? CurrentTableSession { get; }
}

public class SessionWorkspaceService : VttSessionWorkspaceService, ITableSessionCommandService
{
    public TableSession? CurrentTableSession => CurrentVttSession is null
        ? null
        : new TableSession
        {
            Id = CurrentVttSession.Id,
            Title = CurrentVttSession.Title,
            Participants = CurrentVttSession.Participants,
            PlayerSeats = CurrentVttSession.PlayerSeats,
            Surfaces = CurrentVttSession.Surfaces,
            Pieces = CurrentVttSession.Pieces,
            TurnOrder = CurrentVttSession.TurnOrder,
            CurrentTurnIndex = CurrentVttSession.CurrentTurnIndex,
            TurnNumber = CurrentVttSession.TurnNumber,
            CurrentPhase = CurrentVttSession.CurrentPhase,
            TurnState = CurrentVttSession.TurnState,
            Options = CurrentVttSession.Options,
            Visibility = CurrentVttSession.Visibility,
            ActionLog = CurrentVttSession.ActionLog,
            ModuleState = CurrentVttSession.ModuleState,
            Campaigns = CurrentVttSession.Campaigns
        };

    public void OpenTableSessionFromFile(string path)
    {
        OpenVttSessionFromFile(path);
    }

    public new WorkspaceRecoveryDiagnostics RestoreWorkspaceStateWithDiagnostics(string path, WorkspaceRestoreOptions? options = null)
    {
        return base.RestoreWorkspaceStateWithDiagnostics(path, options ?? WorkspaceRestoreOptions.Default);
    }
}

public class PendingScenarioApplicationPlan : Domain.Persistence.Services.Scenario.VttScenarioPendingApplicationPlan
{
}

public class SessionWorkspaceRecoveryState : VttSessionWorkspaceRecoveryState
{
    public string? PendingScenarioSourcePath
    {
        get => PendingVttScenarioSourcePath;
        init => PendingVttScenarioSourcePath = value;
    }
}
