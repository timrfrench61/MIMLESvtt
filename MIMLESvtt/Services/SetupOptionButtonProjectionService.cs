using MIMLESvtt.Components.Shared;

namespace MIMLESvtt.Services;

public static class SetupOptionButtonProjectionService
{
    public static IReadOnlyList<SetupOptionButtonItem> Project<T>(
        IReadOnlyList<T> source,
        Func<T, string> idSelector,
        Func<T, string> nameSelector,
        Func<T, bool>? disabledSelector = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(idSelector);
        ArgumentNullException.ThrowIfNull(nameSelector);

        return source
            .Select(item => new SetupOptionButtonItem(
                idSelector(item),
                nameSelector(item),
                disabledSelector?.Invoke(item) ?? false))
            .ToList();
    }
}
