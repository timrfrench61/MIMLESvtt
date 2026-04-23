namespace MIMLESvtt.src.Domain.Models.Testing;

public enum TestSubsystem
{
    Engine,
    RulesFramework,
    UiPresentation,
    PersistenceImport,
    Networking
}

public class SubsystemOwnershipMatrixEntry
{
    public TestSubsystem Subsystem { get; init; }

    public List<string> Owns { get; init; } = [];

    public List<string> DoesNotOwn { get; init; } = [];
}

public class IntegrationSliceDefinition
{
    public string Id { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<TestSubsystem> ParticipatingSubsystems { get; init; } = [];
}

public static class SubsystemTestBoundaryCatalog
{
    public static IReadOnlyList<SubsystemOwnershipMatrixEntry> GetOwnershipMatrix()
    {
        return
        [
            new SubsystemOwnershipMatrixEntry
            {
                Subsystem = TestSubsystem.Engine,
                Owns = ["core session/state models", "action validation/apply behavior", "board/space/zone/piece state transformations", "turn/phase state behavior"],
                DoesNotOwn = ["ruleset-specific legality logic", "UI rendering behavior"]
            },
            new SubsystemOwnershipMatrixEntry
            {
                Subsystem = TestSubsystem.RulesFramework,
                Owns = ["rules request/result contracts", "module validation/rejection logic", "module resolution payloads", "dice/randomization usage in module flows"],
                DoesNotOwn = ["UI interaction tests", "generic engine persistence tests"]
            },
            new SubsystemOwnershipMatrixEntry
            {
                Subsystem = TestSubsystem.UiPresentation,
                Owns = ["page/component transitions", "route access/flow behavior", "user-visible validation and feedback messaging"],
                DoesNotOwn = ["deep domain rule correctness", "persistence serializer invariants"]
            },
            new SubsystemOwnershipMatrixEntry
            {
                Subsystem = TestSubsystem.PersistenceImport,
                Owns = ["serializer round-trip compatibility", "file workflow safety/recovery", "import parse/map/validate/report behavior", "import packet baseline and edge-case expectations"],
                DoesNotOwn = ["final UI interaction workflows (except handoff checks)"]
            },
            new SubsystemOwnershipMatrixEntry
            {
                Subsystem = TestSubsystem.Networking,
                Owns = ["connection/session sync behavior", "server-authoritative apply ordering", "reconnect/rehydration behavior"],
                DoesNotOwn = []
            }
        ];
    }

    public static IReadOnlyList<IntegrationSliceDefinition> GetIntegrationSlices()
    {
        return
        [
            new IntegrationSliceDefinition
            {
                Id = "INT-ENG-PERSIST",
                Description = "engine + persistence save/load/restore",
                ParticipatingSubsystems = [TestSubsystem.Engine, TestSubsystem.PersistenceImport]
            },
            new IntegrationSliceDefinition
            {
                Id = "INT-ENG-IMPORT",
                Description = "engine + import outcome and state mutation",
                ParticipatingSubsystems = [TestSubsystem.Engine, TestSubsystem.PersistenceImport]
            },
            new IntegrationSliceDefinition
            {
                Id = "INT-ENG-RULES",
                Description = "engine + rules validation/resolution handoff",
                ParticipatingSubsystems = [TestSubsystem.Engine, TestSubsystem.RulesFramework]
            },
            new IntegrationSliceDefinition
            {
                Id = "INT-UI-WORKSPACE",
                Description = "UI + workspace service operator flow and feedback",
                ParticipatingSubsystems = [TestSubsystem.UiPresentation, TestSubsystem.Engine]
            }
        ];
    }

    public static IReadOnlyList<string> GetSeparationRules()
    {
        return
        [
            "Engine tests remain separate from rules-module tests.",
            "UI tests remain separate from domain logic tests.",
            "Integration tests verify contract handoffs and should not duplicate unit-level assertions."
        ];
    }
}
