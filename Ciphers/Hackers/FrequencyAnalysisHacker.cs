namespace Ciphers.Hackers;

public class FrequencyAnalysisHacker : Hacker
{
    private readonly IReadOnlyCollection<(char Character, double Frequency)> _characterFrequencies;

    public FrequencyAnalysisHacker(IEnumerable<char> alphabet, string frequencyAnalysisText) : base(alphabet) =>
        _characterFrequencies = GetCharacterFrequencies(frequencyAnalysisText);

    public override string Hack(string text)
    {
        var textFrequencies = GetCharacterFrequencies(text);

        var charactersMap = textFrequencies
            .Zip(_characterFrequencies)
            .ToDictionary(pair => pair.First.Character, pair => pair.Second.Character);

        var resultText = string.Concat(text.Select(chr => IsSuitableCharacter(chr) && charactersMap.ContainsKey(chr) ?
            charactersMap[chr] :
            chr));

        return resultText;
    }

    private IReadOnlyCollection<(char Character, double Frequency)> GetCharacterFrequencies(string text)
    {
        var suitableCharacters = text.Where(IsSuitableCharacter).ToArray();

        var characterFrequencies = suitableCharacters
            .GroupBy(chr => chr)
            .Select(group => (Character: group.Key, Frequency: (double)group.Count() / suitableCharacters.Length))
            .OrderByDescending(pair => pair.Frequency)
            .ToArray();

        return characterFrequencies;
    }

    private bool IsSuitableCharacter(char chr) => Alphabet.Contains(chr);
}
