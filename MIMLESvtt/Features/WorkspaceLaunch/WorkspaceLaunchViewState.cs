namespace VttMvuView.WorkspaceLaunch;
public sealed record WorkspaceLaunchViewState(GameSystemPickerViewState GameSystemPicker, ScenarioPickerViewState ScenarioPicker, WorkspaceLaunchStep CurrentStep, string? ErrorMessage, string? LaunchPath);
