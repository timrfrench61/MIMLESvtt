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
        page.TestSetOptionalFields("lb", "Mountaineering harness kit", "Manual", "tool,adventure");
        page.TestSetExtensions(("AD&D1.WeaponSpeed", "0"), ("BRP.SkillInteraction", "Climb"));
        page.TestSaveEquipment();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved new equipment and returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestEquipmentExists("EQ900", "Climbing Kit"));
        Assert.AreEqual("Climb", page.TestSelectedEquipmentExtensionValue("BRP.SkillInteraction"));
    }

    [TestMethod]
    public void SaveEquipment_InEditMode_UpdatesSelectedEquipmentAndRetainsRoundTripSelection()
    {
        var page = new ContentEquipment();

        page.TestSelectEquipment("EQ001");
        page.TestSetEditorFields("EQ001", "Longsword +1", "Weapon", "Melee", 50, 3);
        page.TestSetOptionalFields("lb", "Enchanted martial blade", "Manual", "weapon,magic");
        page.TestSetExtensions(("AD&D1.WeaponSpeed", "4"));
        page.TestSaveEquipment();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved equipment changes and returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestEquipmentExists("EQ001", "Longsword +1"));
        Assert.AreEqual("Longsword +1", page.TestSelectedEquipmentName);
        Assert.AreEqual("4", page.TestSelectedEquipmentExtensionValue("AD&D1.WeaponSpeed"));
    }

    [TestMethod]
    public void CancelEdit_InEditMode_RevertsEditorValuesToSelectedEquipment()
    {
        var page = new ContentEquipment();

        page.TestSelectEquipment("EQ002");
        page.TestSetEditorFields("EQ002", "Temp Name", "Temp", "Temp", 1, 1);
        page.TestSetOptionalFields("kg", "Temp", "Temp", "Temp");
        page.TestSetExtensions(("Temp", "Temp"));
        page.TestCancelEdit();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Canceled edit changes. No mutation persisted; returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.AreEqual("Shield", page.TestSelectedEquipmentName);
    }

    [TestMethod]
    public void SaveEquipment_InCreateMode_WithDuplicateId_IsRejected()
    {
        var page = new ContentEquipment();

        page.TestSetCreateMode();
        page.TestSetEditorFields("EQ001", "Duplicate Longsword", "Weapon", "Melee", 20, 3);
        page.TestSaveEquipment();

        Assert.IsFalse(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Save failed. Equipment id already exists.", page.TestEditorSaveMessage);
    }

    [TestMethod]
    public void SaveEquipment_InEditMode_ChangingIdToDuplicate_IsRejectedAndSelectionRetained()
    {
        var page = new ContentEquipment();

        page.TestSelectEquipment("EQ002");
        page.TestSetEditorFields("EQ001", "Shield Renamed", "Armor", "Defense", 10, 6);
        page.TestSaveEquipment();

        Assert.IsFalse(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Save failed. Equipment id already exists.", page.TestEditorSaveMessage);
        Assert.AreEqual("EQ002", page.TestSelectedEquipmentId);
    }

    [TestMethod]
    public void ValidateEditor_WithExtensionValueButMissingKey_Fails()
    {
        var page = new ContentEquipment();

        page.TestSetCreateMode();
        page.TestSetEditorFields("EQ901", "Test Gear", "Tool", "Utility", 5, 1);
        page.TestSetExtensions(("", "value-without-key"));
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Extension row 1 requires a non-empty key.", page.TestEditorValidationMessage);
    }
}
