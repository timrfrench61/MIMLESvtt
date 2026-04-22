namespace MIMLESvtt.src
{
    public class VttScenario
    {
        public string Title { get; set; } = string.Empty;

        public List<SurfaceInstance> Surfaces { get; set; } = [];

        public List<PieceInstance> Pieces { get; set; } = [];

        public TableOptions Options { get; set; } = new();
    }

    public class Scenario : VttScenario
    {
    }
}
