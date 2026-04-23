using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models.Pieces;

namespace MIMLESvtt.Tests;

[TestClass]
public class PieceDefinitionModelTests
{
    [TestMethod]
    public void PieceDefinition_Model_SupportsCategoryPresentationAndRulesetExtensions()
    {
        var definition = new PieceDefinition
        {
            Id = "def-skeleton-1",
            Name = "Skeleton",
            Category = PieceCategory.Unit,
            Presentation =
            {
                ["Sprite"] = "assets/skeleton.png",
                ["Scale"] = 1.2,
                ["Tint"] = "#CFCFCF"
            },
            RulesetExtensions =
            {
                ["ArmorClass"] = 7,
                ["HitDice"] = "1d8",
                ["Morale"] = 12
            }
        };

        Assert.AreEqual("def-skeleton-1", definition.Id);
        Assert.AreEqual("Skeleton", definition.Name);
        Assert.AreEqual(PieceCategory.Unit, definition.Category);
        Assert.AreEqual(3, definition.Presentation.Count);
        Assert.AreEqual(3, definition.RulesetExtensions.Count);
    }

    [TestMethod]
    public void PieceDefinition_CategoryEnum_ContainsCoreAndCustomOptions()
    {
        var values = Enum.GetValues<PieceCategory>();

        CollectionAssert.AreEquivalent(
            new[]
            {
                PieceCategory.Token,
                PieceCategory.Unit,
                PieceCategory.Character,
                PieceCategory.Marker,
                PieceCategory.Terrain,
                PieceCategory.Custom
            },
            values.Cast<PieceCategory>().ToArray());
    }
}
