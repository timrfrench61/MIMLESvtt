namespace VttMvuView.Tabletop;

public static class TabletopUpdate
{
    public static TabletopState Reduce(TabletopState state, ITabletopAction action)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return action switch
        {
            SelectTokenAction select => SelectToken(state, select),
            MoveSelectedTokenAction move => MoveSelectedToken(state, move),
            RotateSelectedTokenAction rotate => RotateSelectedToken(state, rotate),
            EndTurnAction => state with
            {
                TurnNumber = state.TurnNumber + 1,
                TurnPhase = "Main",
                StatusMessage = $"Turn advanced to {state.TurnNumber + 1}."
            },
            SetActiveToolAction setTool => state with
            {
                ActiveToolId = string.IsNullOrWhiteSpace(setTool.ToolId) ? state.ActiveToolId : setTool.ToolId,
                StatusMessage = string.IsNullOrWhiteSpace(setTool.ToolId) ? state.StatusMessage : $"Active tool set to {setTool.ToolId}."
            },
            RollDiceAction => state with
            {
                LastDiceRoll = NextDiceRoll(state.LastDiceRoll),
                StatusMessage = $"Rolled d6: {NextDiceRoll(state.LastDiceRoll)}"
            },
            _ => state
        };
    }

    private static TabletopState SelectToken(TabletopState state, SelectTokenAction action)
    {
        if (string.IsNullOrWhiteSpace(action.TokenId)
            || !state.Tokens.Any(t => string.Equals(t.TokenId, action.TokenId, StringComparison.OrdinalIgnoreCase)))
        {
            return state with { StatusMessage = "Selected token was not found." };
        }

        return state with
        {
            SelectedTokenId = action.TokenId,
            StatusMessage = $"Selected token {action.TokenId}."
        };
    }

    private static TabletopState MoveSelectedToken(TabletopState state, MoveSelectedTokenAction action)
    {
        if (string.IsNullOrWhiteSpace(state.SelectedTokenId))
        {
            return state with { StatusMessage = "Select a token before moving." };
        }

        var selected = state.Tokens.FirstOrDefault(t => string.Equals(t.TokenId, state.SelectedTokenId, StringComparison.OrdinalIgnoreCase));
        if (selected is null)
        {
            return state with { StatusMessage = "Selected token was not found." };
        }

        var nextX = Math.Clamp(selected.X + action.DeltaX, 0, Math.Max(0, state.BoardWidth - 1));
        var nextY = Math.Clamp(selected.Y + action.DeltaY, 0, Math.Max(0, state.BoardHeight - 1));

        var updated = selected with { X = nextX, Y = nextY };
        var tokens = state.Tokens
            .Select(t => string.Equals(t.TokenId, selected.TokenId, StringComparison.OrdinalIgnoreCase) ? updated : t)
            .ToList();

        return state with
        {
            Tokens = tokens,
            StatusMessage = $"Moved {selected.TokenId} to ({nextX}, {nextY})."
        };
    }

    private static TabletopState RotateSelectedToken(TabletopState state, RotateSelectedTokenAction action)
    {
        if (string.IsNullOrWhiteSpace(state.SelectedTokenId))
        {
            return state with { StatusMessage = "Select a token before rotating." };
        }

        var selected = state.Tokens.FirstOrDefault(t => string.Equals(t.TokenId, state.SelectedTokenId, StringComparison.OrdinalIgnoreCase));
        if (selected is null)
        {
            return state with { StatusMessage = "Selected token was not found." };
        }

        var normalizedRotation = ((selected.RotationDegrees + action.DeltaDegrees) % 360 + 360) % 360;
        var updated = selected with { RotationDegrees = normalizedRotation };
        var tokens = state.Tokens
            .Select(t => string.Equals(t.TokenId, selected.TokenId, StringComparison.OrdinalIgnoreCase) ? updated : t)
            .ToList();

        return state with
        {
            Tokens = tokens,
            StatusMessage = $"Rotated {selected.TokenId} to {normalizedRotation}°."
        };
    }

    private static int NextDiceRoll(int? lastDiceRoll)
    {
        if (!lastDiceRoll.HasValue || lastDiceRoll.Value < 1 || lastDiceRoll.Value > 6)
        {
            return 1;
        }

        return lastDiceRoll.Value == 6 ? 1 : lastDiceRoll.Value + 1;
    }
}
