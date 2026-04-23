using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models.Placement;

namespace MIMLESvtt.Tests;

[TestClass]
public class HexGridCoordinateHelperTests
{
    [TestMethod]
    public void GetNeighbors_ReturnsSixAdjacentHexes()
    {
        var origin = new HexCoordinate { Q = 2, R = -1 };

        var neighbors = HexGridCoordinateHelper.GetNeighbors(origin);

        Assert.AreEqual(6, neighbors.Count);

        var keys = neighbors.Select(n => $"{n.Q},{n.R}").ToHashSet(StringComparer.Ordinal);
        CollectionAssert.AreEquivalent(
            new[]
            {
                "3,-1",
                "3,-2",
                "2,-2",
                "1,-1",
                "1,0",
                "2,0"
            },
            keys.ToList());
    }

    [TestMethod]
    public void AxialDistance_ComputesExpectedHexDistance()
    {
        var from = new HexCoordinate { Q = 0, R = 0 };
        var to = new HexCoordinate { Q = 2, R = -1 };

        var distance = HexGridCoordinateHelper.AxialDistance(from, to);

        Assert.AreEqual(2, distance);
    }
}
