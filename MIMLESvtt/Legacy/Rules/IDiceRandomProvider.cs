namespace MIMLESvtt.src.Domain.Rules;

public interface IDiceRandomProvider
{
    int Next(int minValueInclusive, int maxValueExclusive);

    string ProviderName { get; }
}
