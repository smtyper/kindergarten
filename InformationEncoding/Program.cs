using System.Text;
using InformationEncoding.Codes;

Console.OutputEncoding = Encoding.UTF8;
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

const string sourceText = "ХЦЧШЩ";

var windows1251Encoding = Encoding.GetEncoding(1251);
var encoder = new CyclicEncoder(new[] { 1, 1, 1, 0, 1, 1 });
var random = new Random(DateTime.UtcNow.Millisecond);

var results = GetStringBits(sourceText, windows1251Encoding)
    .Select((bitsArray, index) =>
    {
        var sourceChar = sourceText[index];
        var (shiftedBits, remainder, encodedBits) = encoder.Encode(bitsArray);

        var errorIndex = random.Next(0, encodedBits.Length);
        var errorBits = encodedBits
            .Select((bit, bitIndex) => bitIndex == errorIndex ?
                bit is 0 ?
                    1 :
                    0 :
                bit)
            .ToArray();
        var sindrom = encoder.GetCodeSyndrome(errorBits);

        return (sourceChar, bitsArray, shiftedBits, remainder, encodedBits, errorIndex, errorBits, sindrom);
    })
    .ToArray();

foreach (var (sourceChar, bitsArray, shiftedBits, remainder, encodedBits, errorIndex, errorBits, sindrom) in results)
{
    Console.WriteLine($"{sourceChar}: {AsString(bitsArray)}");
    Console.WriteLine($"m(x): {AsString(shiftedBits)} - information bits");
    Console.WriteLine($"remainder: {AsString(remainder)}");
    Console.WriteLine($"code word: {AsString(encodedBits)}");
    Console.WriteLine($"code word with single error by the index {errorIndex}: {AsString(errorBits)}");
    Console.WriteLine($"sindrom: {AsString(sindrom)}");

    Console.WriteLine("-------------------\n");
}

IEnumerable<int[]> GetStringBits(string text, Encoding encoding) => text
    .Select(chr => encoding
        .GetBytes($"{chr}")
        .SelectMany(@byte => Convert
            .ToString(@byte, 2)
            .PadLeft(8, '0')
            .Select(bit => int.Parse($"{bit}")))
        .ToArray());

string AsString(int[] numbersArray) => string.Concat(numbersArray);
