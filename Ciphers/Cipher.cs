namespace Ciphers;

public abstract class Cipher
{
    protected readonly char[] Alphabet;

    protected Cipher(IEnumerable<char> alphabet) => Alphabet = alphabet.Distinct().ToArray();

    public string Encrypt(string text)
    {
        EnsureIsValidText(text);

        return EncryptText(text);
    }

    public string Decrypt(string text)
    {
        EnsureIsValidText(text);

        return DecryptText(text);
    }

    protected abstract string EncryptText(string text);

    protected abstract string DecryptText(string text);

    private void EnsureIsValidText(string text)
    {
        if (text.Any(chr => !Alphabet.Contains(chr)))
            throw new ArgumentException("The text contains characters outside the current alphabet.");
    }
}
