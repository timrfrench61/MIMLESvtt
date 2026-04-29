namespace VttMvuModel.Rules;

public sealed record RuleSetDefinition(string RuleSetId, string Name, IReadOnlyList<ActionDefinition> AvailableActions, IReadOnlyList<RuleConstraint> Constraints);
