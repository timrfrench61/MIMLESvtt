using MIMLESvtt.src.Domain.Rules;

namespace MIMLESvtt.Tests.Harness;

public static class RulesDeterministicRandomSupport
{
    public static IDiceRandomizationService CreateDice(params int[] deterministicRolls)
    {
        return new DiceRandomizationService(new DeterministicDiceRandomProvider(deterministicRolls));
    }

    public sealed class StubRandomProvider : IDiceRandomProvider
    {
        private readonly Queue<int> _values;

        public StubRandomProvider(IEnumerable<int> values)
        {
            _values = new Queue<int>(values ?? []);
        }

        public string ProviderName => "HarnessStub";

        public int Next(int minValueInclusive, int maxValueExclusive)
        {
            if (_values.Count == 0)
            {
                return minValueInclusive;
            }

            return _values.Dequeue();
        }
    }
}
