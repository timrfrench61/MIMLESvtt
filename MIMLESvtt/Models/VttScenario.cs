using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Surfaces;

namespace MIMLESvtt.src
{
    public class VttScenario
    {
        public string Title { get; set; } = string.Empty;

        public List<SurfaceInstance> Surfaces { get; set; } = [];

        public List<PieceInstance> Pieces { get; set; } = [];

        public TabletopOptions Options { get; set; } = new();
    }

    public class Scenario : VttScenario
    {
    }
}
