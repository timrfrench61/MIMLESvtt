using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class BoardDefinitionModelTests
{
    [TestMethod]
    public void BoardDefinition_Model_SupportsDimensionsAndLayoutType()
    {
        var boardDefinition = new BoardDefinition
        {
            Id = "board-1",
            Name = "Starter Board",
            Width = 64,
            Height = 48,
            LayoutType = BoardLayoutType.SquareGrid,
            Metadata =
            {
                ["CellSize"] = 1,
                ["Theme"] = "Dungeon"
            }
        };

        Assert.AreEqual("board-1", boardDefinition.Id);
        Assert.AreEqual("Starter Board", boardDefinition.Name);
        Assert.AreEqual(64, boardDefinition.Width);
        Assert.AreEqual(48, boardDefinition.Height);
        Assert.AreEqual(BoardLayoutType.SquareGrid, boardDefinition.LayoutType);
        Assert.AreEqual(2, boardDefinition.Metadata.Count);
    }

    [TestMethod]
    public void VttSessionSnapshotSerializer_RoundTrip_PreservesSurfaceBoardDefinition()
    {
        var serializer = new VttSessionSnapshotSerializer();
        var session = new VttSession
        {
            Id = "session-board-1",
            Title = "Board Definition Session",
            Surfaces =
            [
                new SurfaceInstance
                {
                    Id = "surface-1",
                    DefinitionId = "def-surface-1",
                    BoardDefinition = new BoardDefinition
                    {
                        Id = "board-main",
                        Name = "Hex Battlefield",
                        Width = 120,
                        Height = 80,
                        LayoutType = BoardLayoutType.HexGrid,
                        Metadata =
                        {
                            ["HexSize"] = 2,
                            ["WrapEdges"] = false
                        }
                    }
                }
            ]
        };

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(1, loaded.Surfaces.Count);
        var boardDefinition = loaded.Surfaces[0].BoardDefinition;

        Assert.AreEqual("board-main", boardDefinition.Id);
        Assert.AreEqual("Hex Battlefield", boardDefinition.Name);
        Assert.AreEqual(120, boardDefinition.Width);
        Assert.AreEqual(80, boardDefinition.Height);
        Assert.AreEqual(BoardLayoutType.HexGrid, boardDefinition.LayoutType);

        var hexSize = (JsonElement)boardDefinition.Metadata["HexSize"];
        var wrapEdges = (JsonElement)boardDefinition.Metadata["WrapEdges"];

        Assert.AreEqual(2, hexSize.GetInt32());
        Assert.IsFalse(wrapEdges.GetBoolean());
    }
}
