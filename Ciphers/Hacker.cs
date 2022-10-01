namespace Ciphers;

public abstract class Hacker
{
    protected readonly char[] Alphabet;

    protected Hacker(IEnumerable<char> alphabet) => Alphabet = alphabet.Distinct().ToArray();

    public abstract string Hack(string text);
}
