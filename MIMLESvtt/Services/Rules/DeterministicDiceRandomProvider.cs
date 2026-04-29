namespace MIMLESvtt.src.Domain.Rules;

public class DeterministicDiceRandomProvider : IDiceRandomProvider
{
    private readonly Queue<int> _values;

    public DeterministicDiceRandomProvider(IEnumerable<int> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = new Queue<int>(values);
    }

    public string ProviderName => "Deterministic";

    public int Next(int minValueInclusive, int maxValueExclusive)
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("No deterministic random values are available.");
        }

        var value = _values.Dequeue();
        if (value < minValueInclusive || value >= maxValueExclusive)
        {
            throw new InvalidOperationException($"Deterministic value {value} is outside requested range [{minValueInclusive}, {maxValueExclusive}).");
        }

        return value;
    }
}
