using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Models.Visibility;
using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;

namespace MIMLESvtt.src.Domain.Persistence.Services.Scenario
{
    public class VttScenarioPlanApplyService
    {
        public VttScenarioPlanApplyResult Apply(VttScenarioPlanApplyRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.PendingVttScenarioPlan is null)
            {
                throw new InvalidOperationException("PendingVttScenarioPlan is required.");
            }

            var plan = request.PendingVttScenarioPlan;
            if (plan.Scenario is null)
            {
                throw new InvalidOperationException("PendingVttScenarioPlan.Scenario is required.");
            }

            var candidateTitle = string.IsNullOrWhiteSpace(request.TargetSessionTitleOverride)
                ? plan.ScenarioTitle
                : request.TargetSessionTitleOverride;

            var vttSessionCandidate = new VttSession
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = candidateTitle,
                Surfaces = plan.Scenario.Surfaces.Select(CloneSurface).ToList(),
                Pieces = plan.Scenario.Pieces.Select(ClonePiece).ToList(),
                Options = CloneTabletopOptions(plan.Scenario.Options),
                Participants = [],
                ActionLog = [],
                ModuleState = []
            };

            return new VttScenarioPlanApplyResult
            {
                IsSuccess = true,
                IsRuntimeStateMutated = false,
                VttSessionCandidate = vttSessionCandidate,
                Message = "Pending scenario plan converted to isolated VttSession candidate.",
                ErrorMessage = null
            };
        }

        private static SurfaceInstance CloneSurface(SurfaceInstance source)
        {
            return new SurfaceInstance
            {
                Id = source.Id,
                DefinitionId = source.DefinitionId,
                Type = source.Type,
                CoordinateSystem = source.CoordinateSystem,
                Layers = source.Layers.Select(CloneLayer).ToList(),
                Zones = source.Zones.Select(CloneZone).ToList(),
                Transform = CloneSurfaceTransform(source.Transform)
            };
        }

        private static Layer CloneLayer(Layer source)
        {
            return new Layer
            {
                Id = source.Id,
                Name = source.Name
            };
        }

        private static Zone CloneZone(Zone source)
        {
            return new Zone
            {
                Id = source.Id,
                Name = source.Name
            };
        }

        private static SurfaceTransform CloneSurfaceTransform(SurfaceTransform source)
        {
            return new SurfaceTransform
            {
                OffsetX = source.OffsetX,
                OffsetY = source.OffsetY,
                Scale = source.Scale
            };
        }

        private static PieceInstance ClonePiece(PieceInstance source)
        {
            return new PieceInstance
            {
                Id = source.Id,
                DefinitionId = source.DefinitionId,
                Location = CloneLocation(source.Location),
                Rotation = CloneRotation(source.Rotation),
                OwnerParticipantId = source.OwnerParticipantId,
                Visibility = CloneVisibility(source.Visibility),
                State = new Dictionary<string, object>(source.State),
                MarkerIds = source.MarkerIds.ToList(),
                StackId = source.StackId,
                ContainerId = source.ContainerId
            };
        }

        private static Location CloneLocation(Location source)
        {
            return new Location
            {
                SurfaceId = source.SurfaceId,
                Coordinate = CloneCoordinate(source.Coordinate),
                ZoneId = source.ZoneId,
                LayerId = source.LayerId
            };
        }

        private static Coordinate CloneCoordinate(Coordinate source)
        {
            return new Coordinate
            {
                X = source.X,
                Y = source.Y,
                Q = source.Q,
                R = source.R
            };
        }

        private static Rotation CloneRotation(Rotation source)
        {
            return new Rotation
            {
                Degrees = source.Degrees
            };
        }

        private static VisibilityState CloneVisibility(VisibilityState source)
        {
            return new VisibilityState
            {
                IsHidden = source.IsHidden,
                VisibleToParticipantIds = source.VisibleToParticipantIds.ToList()
            };
        }

        private static TabletopOptions CloneTabletopOptions(TabletopOptions source)
        {
            return new TabletopOptions
            {
                EnableFog = source.EnableFog,
                EnableTurnTracker = source.EnableTurnTracker,
                Options = new Dictionary<string, object>(source.Options)
            };
        }
    }
}
