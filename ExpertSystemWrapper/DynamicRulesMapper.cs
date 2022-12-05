namespace ExpertSystemWrapper;

public class DynamicRulesMapper
{
    private readonly Dictionary<int, Rule> _mappingDictionary;

    public DynamicRulesMapper(IReadOnlyCollection<(IReadOnlyCollection<string> Conditions,
        IReadOnlyCollection<string> Conclusions)> conditionsConclusionsPairs) =>
        _mappingDictionary = conditionsConclusionsPairs
            .DistinctBy(pair => string.Join(" ", pair.Conditions))
            .Select((pair, index) => (pair, index: index + 1))
            .ToDictionary(item => item.index, item => new Rule(item.pair.Conditions, item.pair.Conclusions, false));

    public (int? Index, Rule? Rule) SelectRule(IReadOnlyCollection<string> facts)
    {
        var conflictSet = GetConflictSet(facts);

        if (!conflictSet.Any())
            return (null, null);

        var (selectedRuleIndex, selectedRule) = conflictSet
            .OrderByDescending(pair => pair.Rule.Conditions.Count)
            .ThenByDescending(pair => pair.Rule.Conclusions.Count)
            .First();

        _mappingDictionary[selectedRuleIndex] = selectedRule with { IsUsed = true };

        return (selectedRuleIndex, selectedRule);
    }

    public IReadOnlyCollection<(int Index, Rule Rule)> GetConflictSet(IReadOnlyCollection<string> facts) =>
        _mappingDictionary
            .Where(pair => !pair.Value.IsUsed && pair.Value.Conditions.All(facts.Contains))
            .Select(pair => (pair.Key, pair.Value))
            .ToArray();

    public record Rule(IReadOnlyCollection<string> Conditions, IReadOnlyCollection<string> Conclusions, bool IsUsed);
}
