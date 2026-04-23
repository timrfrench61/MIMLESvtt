using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models.Placement;

namespace MIMLESvtt.Tests;

[TestClass]
public class SquareGridCoordinateHelperTests
{
    [TestMethod]
    public void GetOrthogonalNeighbors_ReturnsExpectedFourNeighbors()
    {
        var origin = new Coordinate { X = 3, Y = 5 };

        var neighbors = SquareGridCoordinateHelper.GetOrthogonalNeighbors(origin);

        Assert.AreEqual(4, neighbors.Count);
        CollectionAssert.AreEqual(new[] { 3f, 4f }, new[] { neighbors[0].X, neighbors[0].Y });
        CollectionAssert.AreEqual(new[] { 4f, 5f }, new[] { neighbors[1].X, neighbors[1].Y });
        CollectionAssert.AreEqual(new[] { 3f, 6f }, new[] { neighbors[2].X, neighbors[2].Y });
        CollectionAssert.AreEqual(new[] { 2f, 5f }, new[] { neighbors[3].X, neighbors[3].Y });
    }

    [TestMethod]
    public void GetNeighborsIncludingDiagonals_ReturnsExpectedEightNeighbors()
    {
        var origin = new Coordinate { X = 0, Y = 0 };

        var neighbors = SquareGridCoordinateHelper.GetNeighborsIncludingDiagonals(origin);

        Assert.AreEqual(8, neighbors.Count);

        var asKeys = neighbors.Select(n => $"{n.X},{n.Y}").ToHashSet(StringComparer.Ordinal);
        CollectionAssert.AreEquivalent(
            new[]
            {
                "-1,-1", "0,-1", "1,-1",
                "-1,0", "1,0",
                "-1,1", "0,1", "1,1"
            },
            asKeys.ToList());
    }

    [TestMethod]
    public void ManhattanDistance_ComputesGridDistance()
    {
        var from = new Coordinate { X = 2, Y = 3 };
        var to = new Coordinate { X = 7, Y = -1 };

        var distance = SquareGridCoordinateHelper.ManhattanDistance(from, to);

        Assert.AreEqual(9, distance);
    }

    [TestMethod]
    public void ChebyshevDistance_ComputesGridDistance()
    {
        var from = new Coordinate { X = 2, Y = 3 };
        var to = new Coordinate { X = 7, Y = -1 };

        var distance = SquareGridCoordinateHelper.ChebyshevDistance(from, to);

        Assert.AreEqual(5, distance);
    }
}
