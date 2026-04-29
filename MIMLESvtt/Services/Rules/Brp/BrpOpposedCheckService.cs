namespace MIMLESvtt.src.Domain.Rules.Brp;

public class BrpOpposedCheckService
{
    public BrpOpposedCheckResult Resolve(
        BrpPercentileCheckResult attacker,
        string attackerParticipantId,
        BrpPercentileCheckResult defender,
        string defenderParticipantId)
    {
        ArgumentNullException.ThrowIfNull(attacker);
        ArgumentException.ThrowIfNullOrWhiteSpace(attackerParticipantId);
        ArgumentNullException.ThrowIfNull(defender);
        ArgumentException.ThrowIfNullOrWhiteSpace(defenderParticipantId);

        var attackerScore = BuildScore(attacker);
        var defenderScore = BuildScore(defender);

        string winner;
        var wasTie = false;

        if (attackerScore > defenderScore)
        {
            winner = attackerParticipantId;
        }
        else if (defenderScore > attackerScore)
        {
            winner = defenderParticipantId;
        }
        else
        {
            winner = string.CompareOrdinal(attackerParticipantId, defenderParticipantId) <= 0
                ? attackerParticipantId
                : defenderParticipantId;
            wasTie = true;
        }

        var summary = wasTie
            ? $"BRP opposed check tie resolved by deterministic participant-id ordering. Winner: {winner}."
            : $"BRP opposed check winner: {winner}.";

        return new BrpOpposedCheckResult
        {
            IsSuccess = true,
            WinnerParticipantId = winner,
            WasTie = wasTie,
            TieBreakPolicy = "DeterministicParticipantIdOrder",
            AttackerResult = attacker,
            DefenderResult = defender,
            Summary = summary
        };
    }

    private static int BuildScore(BrpPercentileCheckResult result)
    {
        var successWeight = result.Passed ? 1000 : 0;
        var margin = result.EffectiveTargetValue - result.RolledValue;
        return successWeight + margin;
    }
}
