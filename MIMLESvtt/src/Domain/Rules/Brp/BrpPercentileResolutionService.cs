namespace MIMLESvtt.src.Domain.Rules.Brp;

public class BrpPercentileResolutionService
{
    private readonly IDiceRandomizationService _dice;

    public BrpPercentileResolutionService(IDiceRandomizationService dice)
    {
        _dice = dice ?? throw new ArgumentNullException(nameof(dice));
    }

    public RulesValidationResult ValidatePercentileCheck(BrpPercentileCheckRequest request)
    {
        if (request is null)
        {
            return RulesValidationResult.Failure("BrpPercentileCheck payload is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ActorParticipantId))
        {
            return RulesValidationResult.Failure("BrpPercentileCheck actor participant id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.SkillOrAttributeId))
        {
            return RulesValidationResult.Failure("BrpPercentileCheck skill/attribute id is required.");
        }

        if (request.TargetValue < 1 || request.TargetValue > 100)
        {
            return RulesValidationResult.Failure("BrpPercentileCheck target value must be in range 1-100.");
        }

        return RulesValidationResult.Success();
    }

    public BrpPercentileCheckResult ResolvePercentileCheck(BrpPercentileCheckRequest request, string contextTag)
    {
        var validation = ValidatePercentileCheck(request);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Message);
        }

        var roll = _dice.RollD100(contextTag: contextTag);
        var effectiveTarget = Math.Clamp(request.TargetValue + request.Modifier, 1, 100);
        var passed = roll.Total <= effectiveTarget;

        var summary = passed
            ? $"BRP check passed for {request.ActorParticipantId} on {request.SkillOrAttributeId} (roll {roll.Total} <= target {effectiveTarget})."
            : $"BRP check failed for {request.ActorParticipantId} on {request.SkillOrAttributeId} (roll {roll.Total} > target {effectiveTarget}).";

        return new BrpPercentileCheckResult
        {
            IsSuccess = true,
            RolledValue = roll.Total,
            AppliedModifier = request.Modifier,
            EffectiveTargetValue = effectiveTarget,
            Passed = passed,
            RollDetails = roll,
            Summary = summary,
            EffectHints = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["ActorParticipantId"] = request.ActorParticipantId,
                ["CheckLabel"] = request.SkillOrAttributeId,
                ["Passed"] = passed
            }
        };
    }
}
