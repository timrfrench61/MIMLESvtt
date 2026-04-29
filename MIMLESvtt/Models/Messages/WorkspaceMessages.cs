namespace VttMvuModel.Messages;

public sealed record SelectGameSystemMessage(string GameSystemId) : AppMessage;
public sealed record SelectScenarioMessage(string ScenarioId) : AppMessage;
public sealed record StartNewSessionMessage : AppMessage;
public sealed record LoadSavedSessionMessage(string SaveId) : AppMessage;
