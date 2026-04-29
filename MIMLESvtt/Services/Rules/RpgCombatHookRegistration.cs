namespace MIMLESvtt.src.Domain.Rules;

public class RpgCombatHookRegistration
{
    public string ModuleId { get; init; } = string.Empty;

    public string ModuleVersion { get; init; } = string.Empty;

    public IRpgCombatHookModule HookModule { get; init; } = new NoOpRpgCombatHookModule();
}
