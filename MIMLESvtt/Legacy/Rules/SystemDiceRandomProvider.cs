namespace MIMLESvtt.src.Domain.Rules;

public class SystemDiceRandomProvider : IDiceRandomProvider
{
    private readonly Random _random;

    public SystemDiceRandomProvider()
        : this(new Random())
    {
    }

    public SystemDiceRandomProvider(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public string ProviderName => "SystemRandom";

    public int Next(int minValueInclusive, int maxValueExclusive)
    {
        return _random.Next(minValueInclusive, maxValueExclusive);
    }
}
