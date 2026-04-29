using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules.Brp;

public class BrpRulesModuleScaffold : IRulesActionModule
{
    public const string ActionTypePercentileCheck = "BrpPercentileCheck";
    public const string ActionTypeSkillResolution = "BrpSkillResolution";
    public const string ActionTypeOpposedCheck = "BrpOpposedCheck";

    private static readonly Dictionary<string, BrpDeferredMechanicTag> DeferredActionTypeTags = new(StringComparer.Ordinal)
    {
        ["BrpHitLocation"] = BrpDeferredMechanicTag.HitLocationWoundSeverity,
        ["BrpSubsystemVariant"] = BrpDeferredMechanicTag.FullSubsystemVariants,
        ["BrpAdvancedOpposedTier"] = BrpDeferredMechanicTag.AdvancedOpposedMatrixSpecialTiers,
        ["BrpDowntimeProgression"] = BrpDeferredMechanicTag.DowntimeImprovementProgression,
        ["BrpConditionStacking"] = BrpDeferredMechanicTag.FullConditionCatalogStackingExpiry
    };

    private readonly IDiceRandomizationService _dice;
    private readonly BrpPercentileResolutionService _percentileResolutionService;
    private readonly BrpSkillResolutionService _skillResolutionService;
    private readonly BrpOpposedCheckService _opposedCheckService;

    public BrpRulesModuleScaffold()
        : this(new DiceRandomizationService(), new BrpOpposedCheckService())
    {
    }

    public BrpRulesModuleScaffold(IDiceRandomizationService dice, BrpOpposedCheckService opposedCheckService)
    {
        _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        _opposedCheckService = opposedCheckService ?? throw new ArgumentNullException(nameof(opposedCheckService));
        _percentileResolutionService = new BrpPercentileResolutionService(_dice);
        _skillResolutionService = new BrpSkillResolutionService(_percentileResolutionService, _opposedCheckService);
    }

    public IReadOnlyDictionary<string, BrpDeferredMechanicTag> GetDeferredActionTypeTags()
    {
        return DeferredActionTypeTags;
    }

    public RulesValidationResult ValidateAction(RulesContext context, RulesActionRequest request)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(request);

        if (DeferredActionTypeTags.ContainsKey(request.ActionType))
        {
            return RulesValidationResult.Success("Deferred BRP mechanic accepted as no-op path.");
        }

        return request.ActionType switch
        {
            ActionTypePercentileCheck => ValidatePercentileRequest(request),
            ActionTypeSkillResolution => ValidateSkillResolutionRequest(request),
            ActionTypeOpposedCheck => ValidateOpposedCheckRequest(request),
            _ => RulesValidationResult.Failure("BRP scaffold does not support requested action type.")
        };
    }

    public RulesActionResolutionResult ResolveAction(RulesContext context, RulesActionRequest request)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(request);

        if (DeferredActionTypeTags.TryGetValue(request.ActionType, out var deferredTag))
        {
            return BuildDeferredNoOpResponse(request, deferredTag);
        }

        return request.ActionType switch
        {
            ActionTypePercentileCheck => ResolvePercentileCheck(request),
            ActionTypeSkillResolution => ResolveSkillResolution(request),
            ActionTypeOpposedCheck => ResolveOpposedCheck(request),
            _ => RulesActionResolutionResult.Failure("BRP scaffold cannot resolve requested action type.")
        };
    }

    private static RulesValidationResult ValidatePercentileRequest(RulesActionRequest request)
    {
        if (request.Payload is not BrpPercentileCheckRequest payload)
        {
            return RulesValidationResult.Failure("BrpPercentileCheck payload is required.");
        }

        var validator = new BrpPercentileResolutionService(new DiceRandomizationService(new DeterministicDiceRandomProvider([1])));
        return validator.ValidatePercentileCheck(payload);
    }

    private static RulesValidationResult ValidateSkillResolutionRequest(RulesActionRequest request)
    {
        if (request.Payload is not BrpSkillResolutionRequest payload)
        {
            return RulesValidationResult.Failure("BrpSkillResolution payload is required.");
        }

        var validator = new BrpSkillResolutionService(
            new BrpPercentileResolutionService(new DiceRandomizationService(new DeterministicDiceRandomProvider([1]))),
            new BrpOpposedCheckService());
        return validator.ValidateSkillResolution(payload);
    }

    private static RulesValidationResult ValidateOpposedCheckRequest(RulesActionRequest request)
    {
        if (request.Payload is not BrpOpposedCheckRequest payload)
        {
            return RulesValidationResult.Failure("BrpOpposedCheck payload is required.");
        }

        var validator = new BrpSkillResolutionService(
            new BrpPercentileResolutionService(new DiceRandomizationService(new DeterministicDiceRandomProvider([1]))),
            new BrpOpposedCheckService());
        return validator.ValidateOpposedCheck(payload);
    }

    private RulesActionResolutionResult ResolvePercentileCheck(RulesActionRequest request)
    {
        var payload = (BrpPercentileCheckRequest)request.Payload!;
        var result = _percentileResolutionService.ResolvePercentileCheck(payload, "brp:percentile");

        return BuildEngineNoOpResult(request, result, result.Summary);
    }

    private RulesActionResolutionResult ResolveSkillResolution(RulesActionRequest request)
    {
        var payload = (BrpSkillResolutionRequest)request.Payload!;
        var result = _skillResolutionService.ResolveSkillResolution(payload);

        return BuildEngineNoOpResult(request, result, result.Summary);
    }

    private RulesActionResolutionResult ResolveOpposedCheck(RulesActionRequest request)
    {
        var payload = (BrpOpposedCheckRequest)request.Payload!;
        var opposed = _skillResolutionService.ResolveOpposedCheck(payload);

        return BuildEngineNoOpResult(request, opposed, opposed.Summary);
    }

    private static RulesActionResolutionResult BuildDeferredNoOpResponse(RulesActionRequest request, BrpDeferredMechanicTag deferredTag)
    {
        var payload = new
        {
            DeferredTag = deferredTag.ToString(),
            RequestActionType = request.ActionType,
            Status = "DeferredNoOp"
        };

        return RulesActionResolutionResult.Success(new ActionRequest
        {
            ActionType = "NoOpAction",
            ActorParticipantId = request.ActorParticipantId,
            Payload = payload
        }, $"Deferred BRP mechanic: {deferredTag}");
    }

    private static RulesActionResolutionResult BuildEngineNoOpResult(RulesActionRequest request, object resultPayload, string message)
    {
        return RulesActionResolutionResult.Success(new ActionRequest
        {
            ActionType = "NoOpAction",
            ActorParticipantId = request.ActorParticipantId,
            Payload = resultPayload
        }, message);
    }
}
