using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules.Add1;

public class Add1InitiativeTurnProjectionAdapter
{
    public void ApplyToTurnState(VttSession session, Add1InitiativeResult initiativeResult)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(initiativeResult);

        session.TurnOrder = initiativeResult.OrderedParticipantIds.ToList();
        session.CurrentTurnIndex = 0;
        session.CurrentPhase = string.IsNullOrWhiteSpace(initiativeResult.PhaseContext)
            ? "Combat"
            : initiativeResult.PhaseContext;

        if (session.TurnState is null)
        {
            session.TurnState = new TurnState();
        }

        session.TurnState.RoundNumber = initiativeResult.RoundNumber;
        session.TurnState.TurnNumber = session.TurnNumber;
        session.TurnState.CurrentPhase = session.CurrentPhase;
        session.TurnState.CurrentPhaseIndex = 0;
        session.TurnState.CurrentSeatId = session.TurnOrder.Count > 0 ? session.TurnOrder[0] : string.Empty;

        session.TurnState.Metadata["InitiativeMode"] = initiativeResult.InitiativeMode.ToString();
        session.TurnState.Metadata["TieBreakPolicy"] = initiativeResult.TieBreakPolicy.ToString();
        session.TurnState.Metadata["RerollPolicy"] = initiativeResult.RerollPolicy.ToString();
        session.TurnState.Metadata["InitiativeRolls"] = initiativeResult.InitiativeByParticipantId.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.Ordinal);
        session.TurnState.Metadata["InitiativeSideRolls"] = initiativeResult.InitiativeBySide.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.Ordinal);
    }
}
