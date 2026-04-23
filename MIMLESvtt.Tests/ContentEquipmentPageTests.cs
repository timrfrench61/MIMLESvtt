using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentEquipmentPageTests
{
    [TestMethod]
    public void ValidateEditor_WithNegativeWeight_Fails()
    {
        var page = new ContentEquipment();

        page.TestSetCreateMode();
        page.TestSetEditorFields("EQ900", "Heavy Pack", "Tool", "Utility", 5, -1);
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Weight must be 0 or greater.", page.TestEditorValidationMessage);
    }

    [TestMethod]
    public void SaveEquipment_InCreateMode_WithValidFields_AddsEquipmentAndShowsSuccess()
    {
        var page = new ContentEquipment();

        page.TestSetCreateMode();
        page.TestSetEditorFields("EQ900", "Climbing Kit", "Tool", "Utility", 25, 8);
        page.TestSaveEquipment();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved new equipment.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestEquipmentExists("EQ900", "Climbing Kit"));
    }

    [TestMethod]
    public void SaveEquipment_InEditMode_UpdatesSelectedEquipmentAndRetainsRoundTripSelection()
    {
        var page = new ContentEquipment();

        page.TestSelectEquipment("EQ001");
        page.TestSetEditorFields("EQ001", "Longsword +1", "Weapon", "Melee", 50, 3);
        page.TestSaveEquipment();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved equipment changes.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestEquipmentExists("EQ001", "Longsword +1"));
        Assert.AreEqual("Longsword +1", page.TestSelectedEquipmentName);
    }

    [TestMethod]
    public void CancelEdit_InEditMode_RevertsEditorValuesToSelectedEquipment()
    {
        var page = new ContentEquipment();

        page.TestSelectEquipment("EQ002");
        page.TestSetEditorFields("EQ002", "Temp Name", "Temp", "Temp", 1, 1);
        page.TestCancelEdit();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Canceled edit changes.", page.TestEditorSaveMessage);
        Assert.AreEqual("Shield", page.TestSelectedEquipmentName);
    }
}
