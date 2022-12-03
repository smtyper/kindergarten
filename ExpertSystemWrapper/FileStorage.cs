using System.Text.RegularExpressions;

namespace ExpertSystemWrapper;

public static class FileStorage
{
    private const string RulesFilePath = "kb.txt";
    private const string FactsFilePath = "facts.txt";
    private const string LogsFilePath = "explanation.txt";

    private const string ValueExceptBracket = @"[^\(\)]";

    private static readonly Regex RuleLineRegex = new(@$"\s*(\({ValueExceptBracket}+\)\s*)+=>\s*(\({ValueExceptBracket}+\)\s*)+",
        RegexOptions.Compiled);
    private static readonly Regex FactLineRegex = new($@"\s*\({ValueExceptBracket}+\)\s*", RegexOptions.Compiled);
    private static readonly Regex ValueBetweenBracketsRegex = new(@$"(?<=\(){ValueExceptBracket}+(?=\))",
        RegexOptions.Compiled);

    public static async ValueTask<IReadOnlyCollection<(IReadOnlyCollection<string> Conditions,
            IReadOnlyCollection<string> Conclusions)>> ReadRulesFileAsync()
    {
        var lines = await File.ReadAllLinesAsync(RulesFilePath);

        foreach (var line in lines)
            if (!RuleLineRegex.IsMatch(line))
                throw new Exception($"Invalid rule line format: {line}.");

        var conditionsConclusionsPairs = lines
            .Select(line =>
            {
                var splitted = line.Split("=>");

                var conditions = GetValuesFromBrackets(splitted[0]);
                var conclusions = GetValuesFromBrackets(splitted[1]);

                return (conditions, conclusions);
            })
            .ToArray();

        return conditionsConclusionsPairs;
    }

    public static async ValueTask<IReadOnlyCollection<string>> ReadFactsFileAsync()
    {
        var lines = await File.ReadAllLinesAsync(FactsFilePath);

        foreach (var line in lines)
            if (!FactLineRegex.IsMatch(line))
                throw new Exception($"Invalid file line format: {line}.");

        var facts = lines
            .Select(line => GetValuesFromBrackets(line).Single())
            .ToArray();

        return facts;
    }

    public static async ValueTask WriteLogsList(IReadOnlyCollection<string> logs) => await File.WriteAllLinesAsync(
        LogsFilePath, logs);

    private static IReadOnlyCollection<string> GetValuesFromBrackets(string text) => ValueBetweenBracketsRegex
        .Matches(text)
        .Select(match => match.Value.Trim())
        .ToArray();
}
