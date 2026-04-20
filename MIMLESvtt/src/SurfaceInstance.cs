using System.Reflection.Emit;

namespace MIMLESvtt.src
{
    class SurfaceInstance
    {
        string Id;
        string DefinitionId;

        SurfaceType Type;

        CoordinateSystem CoordinateSystem;

        List<Layer> Layers;
        List<Zone> Zones;

        SurfaceTransform Transform;
    }
}
