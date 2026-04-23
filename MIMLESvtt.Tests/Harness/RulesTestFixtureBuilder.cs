using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Rules;

namespace MIMLESvtt.Tests.Harness;

public class RulesTestFixtureBuilder
{
    private readonly VttSession _session = new()
    {
        Id = "rules-harness-session",
        Title = "Rules Harness Session",
        TurnState = new TurnState
        {
            CurrentPhase = "Combat",
            CurrentPhaseIndex = 0,
            RoundNumber = 1,
            TurnNumber = 1,
            PhaseSequence = ["Combat"]
        }
    };

    public RulesTestFixtureBuilder WithParticipant(string id, string name)
    {
        _session.Participants.Add(new Participant
        {
            Id = id,
            Name = name
        });

        if (!_session.TurnOrder.Contains(id, StringComparer.Ordinal))
        {
            _session.TurnOrder.Add(id);
        }

        return this;
    }

    public RulesTestFixtureBuilder WithSurface(string id = "surface-1")
    {
        _session.Surfaces.Add(new SurfaceInstance
        {
            Id = id,
            DefinitionId = $"def-{id}",
            Type = SurfaceType.Map,
            CoordinateSystem = CoordinateSystem.Square
        });

        return this;
    }

    public RulesTestFixtureBuilder WithPiece(string id, string ownerParticipantId = "", string surfaceId = "surface-1")
    {
        _session.Pieces.Add(new PieceInstance
        {
            Id = id,
            DefinitionId = $"def-{id}",
            OwnerParticipantId = ownerParticipantId,
            Location = new Location
            {
                SurfaceId = surfaceId,
                Coordinate = new Coordinate { X = 1, Y = 1 }
            },
            Rotation = new Rotation { Degrees = 0 }
        });

        return this;
    }

    public RulesTestFixtureBuilder WithTurnPhase(string phase, int roundNumber = 1, int turnNumber = 1)
    {
        _session.CurrentPhase = phase;
        _session.TurnNumber = turnNumber;
        _session.TurnState.CurrentPhase = phase;
        _session.TurnState.RoundNumber = roundNumber;
        _session.TurnState.TurnNumber = turnNumber;
        return this;
    }

    public RulesTestFixtureBuilder WithObjectiveMetadata(string key, object value)
    {
        _session.ModuleState[key] = value;
        _session.TurnState.Metadata[key] = value;
        return this;
    }

    public RulesContext BuildContext(string actorParticipantId, string moduleId, string moduleVersion, string scenarioMetadata)
    {
        return new RulesContext
        {
            CurrentSession = _session,
            CurrentTurnNumber = _session.TurnNumber,
            CurrentTurnIndex = _session.CurrentTurnIndex,
            CurrentPhase = _session.CurrentPhase,
            ActorParticipantId = actorParticipantId,
            SelectedRulesModuleId = moduleId,
            SelectedRulesModuleVersion = moduleVersion,
            ScenarioMetadata = scenarioMetadata,
            EnableStrictValidation = true
        };
    }
}
