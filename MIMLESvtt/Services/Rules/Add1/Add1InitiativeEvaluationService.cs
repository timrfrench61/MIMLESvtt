namespace MIMLESvtt.src.Domain.Rules.Add1;

public class Add1InitiativeEvaluationService
{
    private readonly IDiceRandomizationService _dice;

    public Add1InitiativeEvaluationService(IDiceRandomizationService dice)
    {
        _dice = dice ?? throw new ArgumentNullException(nameof(dice));
    }

    public Add1InitiativeResult Evaluate(Add1InitiativeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participants = request.ParticipantIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (participants.Count == 0)
        {
            throw new InvalidOperationException("Add1Initiative requires at least one participant id.");
        }

        if (request.RerollPolicy == Add1InitiativeRerollPolicy.KeepOrderUntilCombatEnds
            && request.RoundNumber > 1
            && request.PreviousRoundResult is not null)
        {
            return new Add1InitiativeResult
            {
                OrderedParticipantIds = request.PreviousRoundResult.OrderedParticipantIds.ToList(),
                InitiativeByParticipantId = new Dictionary<string, int>(request.PreviousRoundResult.InitiativeByParticipantId, StringComparer.Ordinal),
                InitiativeBySide = new Dictionary<string, int>(request.PreviousRoundResult.InitiativeBySide, StringComparer.Ordinal),
                InitiativeMode = request.InitiativeMode,
                TieBreakPolicy = request.TieBreakPolicy,
                RerollPolicy = request.RerollPolicy,
                RoundNumber = request.RoundNumber,
                PhaseContext = request.PhaseContext ?? string.Empty,
                WasRerolledThisRound = false,
                Metadata = BuildMetadata(request, wasRerolledThisRound: false)
            };
        }

        return request.InitiativeMode switch
        {
            Add1InitiativeMode.Side => EvaluateSideMode(participants, request),
            _ => EvaluateIndividualMode(participants, request)
        };
    }

    private Add1InitiativeResult EvaluateIndividualMode(List<string> participants, Add1InitiativeRequest request)
    {
        var rollsByParticipant = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var participantId in participants)
        {
            rollsByParticipant[participantId] = _dice.RollD6(contextTag: "add1:initiative").Total;
        }

        var ordered = OrderParticipants(participants, rollsByParticipant, request.TieBreakPolicy);

        return new Add1InitiativeResult
        {
            OrderedParticipantIds = ordered,
            InitiativeByParticipantId = rollsByParticipant,
            InitiativeBySide = new Dictionary<string, int>(StringComparer.Ordinal),
            InitiativeMode = Add1InitiativeMode.Individual,
            TieBreakPolicy = request.TieBreakPolicy,
            RerollPolicy = request.RerollPolicy,
            RoundNumber = request.RoundNumber,
            PhaseContext = request.PhaseContext ?? string.Empty,
            WasRerolledThisRound = true,
            Metadata = BuildMetadata(request, wasRerolledThisRound: true)
        };
    }

    private Add1InitiativeResult EvaluateSideMode(List<string> participants, Add1InitiativeRequest request)
    {
        var sideByParticipant = participants.ToDictionary(
            participant => participant,
            participant => request.SideByParticipantId.TryGetValue(participant, out var side) && !string.IsNullOrWhiteSpace(side)
                ? side.Trim()
                : participant,
            StringComparer.Ordinal);

        var uniqueSides = sideByParticipant.Values
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var sideStableIndex = participants
            .Select((participantId, index) => new { participantId, index })
            .GroupBy(x => sideByParticipant[x.participantId], StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Min(x => x.index), StringComparer.Ordinal);

        var rollsBySide = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var side in uniqueSides)
        {
            rollsBySide[side] = _dice.RollD6(contextTag: "add1:initiative:side").Total;
        }

        var orderedSides = rollsBySide
            .OrderByDescending(kvp => kvp.Value)
            .ThenBy(kvp => request.TieBreakPolicy == Add1InitiativeTieBreakPolicy.Alphabetical
                ? kvp.Key
                : sideStableIndex[kvp.Key].ToString("D6"), StringComparer.Ordinal)
            .Select(kvp => kvp.Key)
            .ToList();

        var orderedParticipants = new List<string>();
        foreach (var side in orderedSides)
        {
            var members = participants.Where(p => string.Equals(sideByParticipant[p], side, StringComparison.Ordinal)).ToList();
            if (request.TieBreakPolicy == Add1InitiativeTieBreakPolicy.Alphabetical)
            {
                members = members.OrderBy(m => m, StringComparer.Ordinal).ToList();
            }
            orderedParticipants.AddRange(members);
        }

        var rollsByParticipant = participants.ToDictionary(
            participant => participant,
            participant => rollsBySide[sideByParticipant[participant]],
            StringComparer.Ordinal);

        return new Add1InitiativeResult
        {
            OrderedParticipantIds = orderedParticipants,
            InitiativeByParticipantId = rollsByParticipant,
            InitiativeBySide = rollsBySide,
            InitiativeMode = Add1InitiativeMode.Side,
            TieBreakPolicy = request.TieBreakPolicy,
            RerollPolicy = request.RerollPolicy,
            RoundNumber = request.RoundNumber,
            PhaseContext = request.PhaseContext ?? string.Empty,
            WasRerolledThisRound = true,
            Metadata = BuildMetadata(request, wasRerolledThisRound: true)
        };
    }

    private static List<string> OrderParticipants(
        List<string> participants,
        Dictionary<string, int> rollsByParticipant,
        Add1InitiativeTieBreakPolicy tieBreakPolicy)
    {
        var indexed = participants.Select((participantId, index) => new
        {
            participantId,
            index,
            roll = rollsByParticipant[participantId]
        });

        if (tieBreakPolicy == Add1InitiativeTieBreakPolicy.Alphabetical)
        {
            return indexed
                .OrderByDescending(x => x.roll)
                .ThenBy(x => x.participantId, StringComparer.Ordinal)
                .Select(x => x.participantId)
                .ToList();
        }

        return indexed
            .OrderByDescending(x => x.roll)
            .ThenBy(x => x.index)
            .Select(x => x.participantId)
            .ToList();
    }

    private static Dictionary<string, object> BuildMetadata(Add1InitiativeRequest request, bool wasRerolledThisRound)
    {
        return new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["InitiativeMode"] = request.InitiativeMode.ToString(),
            ["TieBreakPolicy"] = request.TieBreakPolicy.ToString(),
            ["RerollPolicy"] = request.RerollPolicy.ToString(),
            ["WasRerolledThisRound"] = wasRerolledThisRound,
            ["RoundNumber"] = request.RoundNumber,
            ["PhaseContext"] = request.PhaseContext ?? string.Empty
        };
    }
}
