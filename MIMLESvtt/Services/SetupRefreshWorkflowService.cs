namespace MIMLESvtt.Services;

public static class SetupRefreshWorkflowService
{
    public static SetupRefreshSelectionResult<TOption> BuildSelection<TSource, TOption>(
        IReadOnlyList<TSource> sourceItems,
        Func<IReadOnlyList<TSource>, IReadOnlyList<TOption>> map,
        Func<TOption, string> idSelector,
        string? currentSelectedId,
        Func<IReadOnlyList<TOption>>? fallbackOptionsFactory = null)
    {
        ArgumentNullException.ThrowIfNull(sourceItems);
        ArgumentNullException.ThrowIfNull(map);
        ArgumentNullException.ThrowIfNull(idSelector);

        var options = map(sourceItems).ToList();

        if (options.Count == 0 && fallbackOptionsFactory is not null)
        {
            options = fallbackOptionsFactory().ToList();
        }

        var selectedId = SetupSelectionDefaultsService.ResolveSelectedId(
            currentSelectedId,
            options.Select(idSelector).ToList());

        return new SetupRefreshSelectionResult<TOption>(options, selectedId);
    }
}

public sealed record SetupRefreshSelectionResult<TOption>(
    IReadOnlyList<TOption> Options,
    string? SelectedId);
