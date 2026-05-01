using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules.Add1;

public class Add1RulesModuleScaffold : IRulesActionModule
{
    public const string ActionTypeInitiative = "Add1Initiative";
    public const string ActionTypeAttack = "Add1Attack";
    public const string ActionTypeSavingThrow = "Add1SavingThrow";

    private static readonly Dictionary<string, Add1DeferredMechanicTag> DeferredActionTypeTags = new(StringComparer.Ordinal)
    {
        ["Add1SpellCast"] = Add1DeferredMechanicTag.SpellSubsystem,
        ["Add1WeaponVsArmor"] = Add1DeferredMechanicTag.WeaponVsArmorMatrix,
        ["Add1MoraleCheck"] = Add1DeferredMechanicTag.MoraleReactionDepth,
        ["Add1EncumbranceTick"] = Add1DeferredMechanicTag.EncumbranceTravelTimekeeping,
        ["Add1ConditionStacking"] = Add1DeferredMechanicTag.AdvancedConditionStackingExpiry,
        ["Add1MonsterEdgeCase"] = Add1DeferredMechanicTag.FullMonsterManualEdgeCases
    };

    private readonly IDiceRandomizationService _dice;
    private readonly Add1InitiativeEvaluationService _initiativeService;

    public Add1RulesModuleScaffold()
        : this(new DiceRandomizationService())
    {
    }

    public Add1RulesModuleScaffold(IDiceRandomizationService dice)
    {
        _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        _initiativeService = new Add1InitiativeEvaluationService(_dice);
    }

    public IReadOnlyDictionary<string, Add1DeferredMechanicTag> GetDeferredActionTypeTags()
    {
        return DeferredActionTypeTags;
    }

    public RulesValidationResult ValidateAction(RulesContext context, RulesActionRequest request)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(request);

        if (DeferredActionTypeTags.ContainsKey(request.ActionType))
        {
            return RulesValidationResult.Success("Deferred mechanic action accepted as no-op path.");
        }

        return request.ActionType switch
        {
            ActionTypeInitiative => ValidateInitiativeRequest(request),
            ActionTypeAttack => ValidateAttackRequest(request),
            ActionTypeSavingThrow => ValidateSavingThrowRequest(request),
            _ => RulesValidationResult.Failure("AD&D1 scaffold does not support requested action type.")
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
            ActionTypeInitiative => ResolveInitiative(request),
            ActionTypeAttack => ResolveAttack(request),
            ActionTypeSavingThrow => ResolveSavingThrow(request),
            _ => RulesActionResolutionResult.Failure("AD&D1 scaffold cannot resolve requested action type.")
        };
    }

    private RulesValidationResult ValidateInitiativeRequest(RulesActionRequest request)
    {
        if (request.Payload is not Add1InitiativeRequest payload)
        {
            return RulesValidationResult.Failure("Add1Initiative payload is required.");
        }

        if (payload.ParticipantIds.Count == 0)
        {
            return RulesValidationResult.Failure("Add1Initiative requires at least one participant id.");
        }

        return RulesValidationResult.Success();
    }

    private RulesValidationResult ValidateAttackRequest(RulesActionRequest request)
    {
        if (request.Payload is not Add1AttackRequest payload)
        {
            return RulesValidationResult.Failure("Add1Attack payload is required.");
        }

        if (string.IsNullOrWhiteSpace(payload.AttackerParticipantId))
        {
            return RulesValidationResult.Failure("Add1Attack attacker participant id is required.");
        }

        if (string.IsNullOrWhiteSpace(payload.AttackerPieceId))
        {
            return RulesValidationResult.Failure("Add1Attack attacker piece id is required.");
        }

        if (string.IsNullOrWhiteSpace(payload.TargetPieceId))
        {
            return RulesValidationResult.Failure("Add1Attack target piece id is required.");
        }

        if (string.IsNullOrWhiteSpace(payload.AttackType))
        {
            return RulesValidationResult.Failure("Add1Attack attack type is required.");
        }

        if (payload.ToHitThreshold <= 1 || payload.ToHitThreshold > 20)
        {
            return RulesValidationResult.Failure("Add1Attack to-hit threshold must be in range 2-20.");
        }

        return RulesValidationResult.Success();
    }

    private RulesValidationResult ValidateSavingThrowRequest(RulesActionRequest request)
    {
        if (request.Payload is not Add1SavingThrowRequest payload)
        {
            return RulesValidationResult.Failure("Add1SavingThrow payload is required.");
        }

        if (string.IsNullOrWhiteSpace(payload.ActorParticipantId))
        {
            return RulesValidationResult.Failure("Add1SavingThrow actor participant id is required.");
        }

        if (string.IsNullOrWhiteSpace(payload.SaveType))
        {
            return RulesValidationResult.Failure("Add1SavingThrow save type is required.");
        }

        if (payload.SaveTarget <= 1 || payload.SaveTarget > 20)
        {
            return RulesValidationResult.Failure("Add1SavingThrow target must be in range 2-20.");
        }

        return RulesValidationResult.Success();
    }

    private RulesActionResolutionResult ResolveInitiative(RulesActionRequest request)
    {
        var payload = (Add1InitiativeRequest)request.Payload!;
        var result = _initiativeService.Evaluate(payload);

        return BuildEngineNoOpResult(request, result, "Initiative baseline resolved.");
    }

    private RulesActionResolutionResult ResolveAttack(RulesActionRequest request)
    {
        var payload = (Add1AttackRequest)request.Payload!;
        var attackRollDetails = _dice.RollD20(contextTag: "add1:attack");
        var attackRoll = attackRollDetails.Total + payload.AttackModifier;
        var isHit = attackRoll >= payload.ToHitThreshold;

        var result = new Add1AttackResult
        {
            IsSuccess = true,
            AttackRoll = attackRoll,
            IsHit = isHit,
            RollDetails = attackRollDetails,
            AppliedModifier = payload.AttackModifier,
            DamageEffect = new Add1DamageEffect
            {
                TargetPieceId = payload.TargetPieceId,
                DamageAmount = isHit ? Math.Max(0, payload.DamageAmountOnHit) : 0,
                ShouldApply = isHit
            },
            StatusEffect = new Add1StatusConditionEffect
            {
                TargetPieceId = payload.TargetPieceId,
                ConditionMarkerId = isHit ? (payload.StatusOnHit?.Trim() ?? string.Empty) : string.Empty,
                ShouldApply = isHit && !string.IsNullOrWhiteSpace(payload.StatusOnHit)
            },
            Summary = isHit
                ? $"AD&D1 attack hit (roll {attackRoll} vs threshold {payload.ToHitThreshold})."
                : $"AD&D1 attack miss (roll {attackRoll} vs threshold {payload.ToHitThreshold})."
        };

        return BuildEngineNoOpResult(request, result, isHit ? "Attack hit." : "Attack miss.");
    }

    private RulesActionResolutionResult ResolveSavingThrow(RulesActionRequest request)
    {
        var payload = (Add1SavingThrowRequest)request.Payload!;
        var saveRollDetails = _dice.RollD20(contextTag: "add1:save");
        var saveRoll = saveRollDetails.Total;
        var savePassed = saveRoll >= payload.SaveTarget;

        var result = new Add1SavingThrowResult
        {
            IsSuccess = true,
            SavePassed = savePassed,
            SaveRoll = saveRoll,
            SaveTarget = payload.SaveTarget,
            RollDetails = saveRollDetails,
            AppliedModifier = 0,
            Summary = savePassed
                ? $"AD&D1 saving throw passed (roll {saveRoll} vs target {payload.SaveTarget})."
                : $"AD&D1 saving throw failed (roll {saveRoll} vs target {payload.SaveTarget})."
        };

        return BuildEngineNoOpResult(request, result, result.SavePassed ? "Saving throw succeeded." : "Saving throw failed.");
    }

    private static RulesActionResolutionResult BuildDeferredNoOpResponse(RulesActionRequest request, Add1DeferredMechanicTag deferredTag)
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
        }, $"Deferred AD&D1 mechanic: {deferredTag}");
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
