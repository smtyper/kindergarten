using ExpertSystemWrapper;

var rulesPairs = await FileStorage.ReadRulesFileAsync();
var factsList = (await FileStorage.ReadFactsFileAsync()).ToList();

var rulesMapper = new DynamicRulesMapper(rulesPairs);
var logsList = new List<string>();

var iterationIndex = 0;

while (true)
{
    var conflictSet = rulesMapper.GetConflictSet(factsList);
    var (selectedRuleIndex, selectedRule) = rulesMapper.SelectRule(factsList);

    if (selectedRule is null)
        break;

    factsList.AddRange(selectedRule.Conclusions);
    LogSelection(conflictSet, selectedRuleIndex!.Value);
    iterationIndex++;
}

await FileStorage.WriteLogsList(logsList);

void LogSelection(IReadOnlyCollection<(int Index, DynamicRulesMapper.Rule Rule)> conflictSet, int selectedRuleIndex)
{
    var logMessage =
        $"{iterationIndex}. ({string.Join(", ", conflictSet.Select(pair => pair.Index))}); ({selectedRuleIndex})";

    logsList.Add(logMessage);
    Console.WriteLine($"[{DateTime.UtcNow}]: {logMessage}");
}


