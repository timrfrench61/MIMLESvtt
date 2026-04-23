using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class ZoneTerrainMetadataTests
{
    [TestMethod]
    public void Zone_TerrainMetadata_SupportsGenericValues()
    {
        var zone = new Zone
        {
            Id = "zone-1",
            Name = "Difficult Terrain"
        };

        zone.TerrainMetadata["MovementCost"] = 2;
        zone.TerrainMetadata["BlocksLineOfSight"] = true;
        zone.TerrainMetadata["SurfaceType"] = "Forest";

        Assert.AreEqual(3, zone.TerrainMetadata.Count);
        Assert.AreEqual(2, zone.TerrainMetadata["MovementCost"]);
        Assert.AreEqual(true, zone.TerrainMetadata["BlocksLineOfSight"]);
        Assert.AreEqual("Forest", zone.TerrainMetadata["SurfaceType"]);
    }

    [TestMethod]
    public void VttSessionSnapshotSerializer_RoundTrip_PreservesZoneTerrainMetadata()
    {
        var serializer = new VttSessionSnapshotSerializer();
        var session = new VttSession
        {
            Id = "session-1",
            Title = "Terrain Metadata Session",
            Surfaces =
            [
                new SurfaceInstance
                {
                    Id = "surface-1",
                    DefinitionId = "def-surface-1",
                    Zones =
                    [
                        new Zone
                        {
                            Id = "zone-1",
                            Name = "Forest",
                            TerrainMetadata =
                            {
                                ["MovementCost"] = 2,
                                ["CoverBonus"] = 1,
                                ["BlocksLineOfSight"] = true
                            }
                        }
                    ]
                }
            ]
        };

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(1, loaded.Surfaces.Count);
        Assert.AreEqual(1, loaded.Surfaces[0].Zones.Count);

        var loadedZone = loaded.Surfaces[0].Zones[0];
        Assert.AreEqual("Forest", loadedZone.Name);
        Assert.AreEqual(3, loadedZone.TerrainMetadata.Count);

        var moveCost = (JsonElement)loadedZone.TerrainMetadata["MovementCost"];
        var coverBonus = (JsonElement)loadedZone.TerrainMetadata["CoverBonus"];
        var blocksLos = (JsonElement)loadedZone.TerrainMetadata["BlocksLineOfSight"];

        Assert.AreEqual(2, moveCost.GetInt32());
        Assert.AreEqual(1, coverBonus.GetInt32());
        Assert.IsTrue(blocksLos.GetBoolean());
    }
}
