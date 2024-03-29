﻿namespace Ciphers.Transposition;

public class CaesarCipher : Cipher
{
    protected readonly int Key;

    protected readonly IReadOnlyDictionary<char, char> EncryptionCharMapping;
    protected readonly IReadOnlyDictionary<char, char> DecryptionCharMapping;

    public CaesarCipher(IReadOnlyCollection<char> alphabet, int key) : base(alphabet)
    {
        Key = key;

        var charMapping = Alphabet
            .Select((chr, index) => (chr, index))
            .ToDictionary(pair => pair.chr, pair => Alphabet[Math.Abs(pair.index + Key) % alphabet.Count]);

        EncryptionCharMapping = charMapping;
        DecryptionCharMapping = charMapping.ToDictionary(pair => pair.Value, pair => pair.Key);
    }

    public override string EncryptText(string text) => string.Concat(text
        .Select(chr => Alphabet.Contains(chr) ?
            EncryptionCharMapping[chr] :
            chr));

    public override string DecryptText(string text) => string.Concat(text
        .Select(chr => Alphabet.Contains(chr) ?
            DecryptionCharMapping[chr] :
            chr));
}
