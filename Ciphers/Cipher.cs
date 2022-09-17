namespace Ciphers;

public abstract class Cipher
{
    protected readonly char[] Alphabet;

    protected Cipher(IEnumerable<char> alphabet) => Alphabet = alphabet.Distinct().ToArray();

    public abstract string EncryptText(string text);

    public abstract string DecryptText(string text);
}
