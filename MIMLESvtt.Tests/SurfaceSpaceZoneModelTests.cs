using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class SurfaceSpaceZoneModelTests
{
    [TestMethod]
    public void Space_Model_SupportsOccupancyAndAdjacency()
    {
        var space = new Space
        {
            Id = "space-1",
            Coordinate = new Coordinate { X = 2, Y = 3 },
            HexCoordinate = new HexCoordinate { Q = 1, R = -1 },
            OccupantPieceIds = ["piece-1", "piece-2"],
            AdjacentSpaceIds = ["space-2", "space-3"]
        };

        Assert.AreEqual("space-1", space.Id);
        Assert.AreEqual(2f, space.Coordinate.X);
        Assert.AreEqual(3f, space.Coordinate.Y);
        Assert.IsNotNull(space.HexCoordinate);
        Assert.AreEqual(1, space.HexCoordinate!.Q);
        Assert.AreEqual(-1, space.HexCoordinate.R);
        CollectionAssert.AreEqual(new List<string> { "piece-1", "piece-2" }, space.OccupantPieceIds);
        CollectionAssert.AreEqual(new List<string> { "space-2", "space-3" }, space.AdjacentSpaceIds);
    }

    [TestMethod]
    public void VttSessionSnapshotSerializer_RoundTrip_PreservesSurfaceSpacesAndZoneSpaceReferences()
    {
        var serializer = new VttSessionSnapshotSerializer();
        var session = new VttSession
        {
            Id = "session-space-1",
            Title = "Surface Space Session",
            Surfaces =
            [
                new SurfaceInstance
                {
                    Id = "surface-1",
                    DefinitionId = "def-surface-1",
                    Spaces =
                    [
                        new Space
                        {
                            Id = "space-1",
                            Coordinate = new Coordinate { X = 0, Y = 0 },
                            OccupantPieceIds = ["piece-a"],
                            AdjacentSpaceIds = ["space-2"]
                        },
                        new Space
                        {
                            Id = "space-2",
                            Coordinate = new Coordinate { X = 1, Y = 0 },
                            OccupantPieceIds = [],
                            AdjacentSpaceIds = ["space-1"]
                        }
                    ],
                    Zones =
                    [
                        new Zone
                        {
                            Id = "zone-1",
                            Name = "Deployment",
                            SpaceIds = ["space-1", "space-2"],
                            Metadata =
                            {
                                ["Kind"] = "Deployment",
                                ["Priority"] = 1
                            }
                        }
                    ]
                }
            ]
        };

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(1, loaded.Surfaces.Count);
        var loadedSurface = loaded.Surfaces[0];
        Assert.AreEqual(2, loadedSurface.Spaces.Count);
        Assert.AreEqual("space-1", loadedSurface.Spaces[0].Id);
        CollectionAssert.AreEqual(new List<string> { "piece-a" }, loadedSurface.Spaces[0].OccupantPieceIds);
        CollectionAssert.AreEqual(new List<string> { "space-2" }, loadedSurface.Spaces[0].AdjacentSpaceIds);

        Assert.AreEqual(1, loadedSurface.Zones.Count);
        var loadedZone = loadedSurface.Zones[0];
        CollectionAssert.AreEqual(new List<string> { "space-1", "space-2" }, loadedZone.SpaceIds);

        var kind = (JsonElement)loadedZone.Metadata["Kind"];
        var priority = (JsonElement)loadedZone.Metadata["Priority"];

        Assert.AreEqual("Deployment", kind.GetString());
        Assert.AreEqual(1, priority.GetInt32());
    }
}
