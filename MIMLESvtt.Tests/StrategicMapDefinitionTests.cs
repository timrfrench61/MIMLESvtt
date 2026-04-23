using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class StrategicMapDefinitionTests
{
    [TestMethod]
    public void StrategicMapDefinition_Model_SupportsRegionAdjacency()
    {
        var map = new StrategicMapDefinition
        {
            Regions =
            [
                new StrategicRegion
                {
                    Id = "north",
                    Name = "Northern Territory",
                    AdjacentRegionIds = ["center"],
                    Metadata = { ["ControlValue"] = 2 }
                },
                new StrategicRegion
                {
                    Id = "center",
                    Name = "Central Plains",
                    AdjacentRegionIds = ["north", "south"],
                    Metadata = { ["ControlValue"] = 3 }
                }
            ]
        };

        Assert.AreEqual(2, map.Regions.Count);
        CollectionAssert.AreEqual(new List<string> { "center" }, map.Regions[0].AdjacentRegionIds);
        CollectionAssert.AreEqual(new List<string> { "north", "south" }, map.Regions[1].AdjacentRegionIds);
    }

    [TestMethod]
    public void VttSessionSnapshotSerializer_RoundTrip_PreservesStrategicMapDefinition()
    {
        var serializer = new VttSessionSnapshotSerializer();
        var session = new VttSession
        {
            Id = "session-strategic-1",
            Title = "Strategic Session",
            Surfaces =
            [
                new SurfaceInstance
                {
                    Id = "surface-strategic",
                    DefinitionId = "def-surface-strategic",
                    BoardDefinition = new BoardDefinition
                    {
                        Id = "board-strategic",
                        Name = "Strategic Theater",
                        LayoutType = BoardLayoutType.Region,
                        StrategicMap =
                        {
                            Regions =
                            [
                                new StrategicRegion
                                {
                                    Id = "region-a",
                                    Name = "Region A",
                                    AdjacentRegionIds = ["region-b"],
                                    Metadata =
                                    {
                                        ["Supply"] = 2,
                                        ["HasPort"] = true
                                    }
                                },
                                new StrategicRegion
                                {
                                    Id = "region-b",
                                    Name = "Region B",
                                    AdjacentRegionIds = ["region-a"]
                                }
                            ],
                            Metadata =
                            {
                                ["Scale"] = "Strategic",
                                ["WrapEdges"] = false
                            }
                        }
                    }
                }
            ]
        };

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        var loadedMap = loaded.Surfaces[0].BoardDefinition.StrategicMap;
        Assert.AreEqual(2, loadedMap.Regions.Count);
        Assert.AreEqual("region-a", loadedMap.Regions[0].Id);
        CollectionAssert.AreEqual(new List<string> { "region-b" }, loadedMap.Regions[0].AdjacentRegionIds);

        var supply = (JsonElement)loadedMap.Regions[0].Metadata["Supply"];
        var hasPort = (JsonElement)loadedMap.Regions[0].Metadata["HasPort"];

        Assert.AreEqual(2, supply.GetInt32());
        Assert.IsTrue(hasPort.GetBoolean());

        var scale = (JsonElement)loadedMap.Metadata["Scale"];
        var wrapEdges = (JsonElement)loadedMap.Metadata["WrapEdges"];

        Assert.AreEqual("Strategic", scale.GetString());
        Assert.IsFalse(wrapEdges.GetBoolean());
    }
}
