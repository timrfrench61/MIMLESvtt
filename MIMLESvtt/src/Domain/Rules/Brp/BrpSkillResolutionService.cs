namespace MIMLESvtt.src.Domain.Rules.Brp;

public class BrpSkillResolutionService
{
    private readonly BrpPercentileResolutionService _percentileResolutionService;
    private readonly BrpOpposedCheckService _opposedCheckService;

    public BrpSkillResolutionService(
        BrpPercentileResolutionService percentileResolutionService,
        BrpOpposedCheckService opposedCheckService)
    {
        _percentileResolutionService = percentileResolutionService ?? throw new ArgumentNullException(nameof(percentileResolutionService));
        _opposedCheckService = opposedCheckService ?? throw new ArgumentNullException(nameof(opposedCheckService));
    }

    public RulesValidationResult ValidateSkillResolution(BrpSkillResolutionRequest request)
    {
        if (request is null)
        {
            return RulesValidationResult.Failure("BrpSkillResolution payload is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ActorParticipantId))
        {
            return RulesValidationResult.Failure("BrpSkillResolution actor participant id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.SkillId))
        {
            return RulesValidationResult.Failure("BrpSkillResolution skill id is required.");
        }

        if (request.TargetValue < 1 || request.TargetValue > 100)
        {
            return RulesValidationResult.Failure("BrpSkillResolution target value must be in range 1-100.");
        }

        return RulesValidationResult.Success();
    }

    public BrpPercentileCheckResult ResolveSkillResolution(BrpSkillResolutionRequest request)
    {
        var validation = ValidateSkillResolution(request);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Message);
        }

        var contextTag = string.IsNullOrWhiteSpace(request.ContextTag)
            ? "brp:skill"
            : request.ContextTag.Trim();

        return _percentileResolutionService.ResolvePercentileCheck(new BrpPercentileCheckRequest
        {
            ActorParticipantId = request.ActorParticipantId,
            SkillOrAttributeId = request.SkillId,
            TargetValue = request.TargetValue,
            Modifier = request.Modifier
        }, contextTag);
    }

    public RulesValidationResult ValidateOpposedCheck(BrpOpposedCheckRequest request)
    {
        if (request is null)
        {
            return RulesValidationResult.Failure("BrpOpposedCheck payload is required.");
        }

        if (string.IsNullOrWhiteSpace(request.AttackerParticipantId))
        {
            return RulesValidationResult.Failure("BrpOpposedCheck attacker participant id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DefenderParticipantId))
        {
            return RulesValidationResult.Failure("BrpOpposedCheck defender participant id is required.");
        }

        if (request.AttackerTargetValue < 1 || request.AttackerTargetValue > 100)
        {
            return RulesValidationResult.Failure("BrpOpposedCheck attacker target value must be in range 1-100.");
        }

        if (request.DefenderTargetValue < 1 || request.DefenderTargetValue > 100)
        {
            return RulesValidationResult.Failure("BrpOpposedCheck defender target value must be in range 1-100.");
        }

        return RulesValidationResult.Success();
    }

    public BrpOpposedCheckResult ResolveOpposedCheck(BrpOpposedCheckRequest request)
    {
        var validation = ValidateOpposedCheck(request);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Message);
        }

        var attacker = _percentileResolutionService.ResolvePercentileCheck(new BrpPercentileCheckRequest
        {
            ActorParticipantId = request.AttackerParticipantId,
            SkillOrAttributeId = "OpposedAttacker",
            TargetValue = request.AttackerTargetValue,
            Modifier = request.AttackerModifier
        }, "brp:opposed:attacker");

        var defender = _percentileResolutionService.ResolvePercentileCheck(new BrpPercentileCheckRequest
        {
            ActorParticipantId = request.DefenderParticipantId,
            SkillOrAttributeId = "OpposedDefender",
            TargetValue = request.DefenderTargetValue,
            Modifier = request.DefenderModifier
        }, "brp:opposed:defender");

        return _opposedCheckService.Resolve(attacker, request.AttackerParticipantId, defender, request.DefenderParticipantId);
    }
}
