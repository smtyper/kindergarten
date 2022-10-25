using MoreLinq.Extensions;

namespace InformationEncoding.Codes;

public class CyclicEncoder
{
    private readonly int[] _generatingPolynomial;

    public CyclicEncoder(int[] generatingPolynomial) => _generatingPolynomial = generatingPolynomial;

    public (int[] ShiftedBits, int[] Remainder, int[] EncodedBits) Encode(int[] bitsArray)
    {
        var checkBitsCount = _generatingPolynomial.Length - 1;
        var shiftedBits = PolyMultiply(bitsArray, checkBitsCount, 1, ToBinary);
        var remainder = GetPolyDevideRemainder(shiftedBits, _generatingPolynomial, ToBinary);

        var encodedBits = PolySum(shiftedBits, remainder, ToBinary);

        return (shiftedBits, remainder, encodedBits);
    }

    public int[] GetCodeSyndrome(int[] encodedBits) => GetPolyDevideRemainder(encodedBits, _generatingPolynomial,
            ToBinary)
        .Reverse()
        .SkipWhile(number => number is 0)
        .Reverse()
        .ToArray();

    private static int[] PolyMultiply(int[] polymon, int degree, int coefficient, Func<int, int> resultselector) =>
        Enumerable
            .Range(0, degree)
            .Select(_ => 0)
            .Concat(polymon
                .Select(monomialCoefficient => monomialCoefficient * coefficient))
            .Select(resultselector)
            .ToArray();

    private static int[] PolySum(int[] firstPolynom, int[] secondPolynom, Func<int, int> resultselector) =>
        firstPolynom.ZipLongest(secondPolynom, (first, second) => first + second).Select(resultselector).ToArray();

    private static int[] GetPolyDevideRemainder(int[] firstPolynom, int[] secondPolynom, Func<int, int> resultselector)
    {
        var remainder = firstPolynom.ToArray();

        foreach (var index in Enumerable.Range(0, firstPolynom.Length - secondPolynom.Length + 1))
        {
            var coefficient = Enumerable.SkipLast(remainder, index).Last() / secondPolynom.Last();

            foreach (var remainderIndex in secondPolynom.Select((_, divisonIndex) => divisonIndex))
                remainder[remainder.Length - 1 - index - remainderIndex] -= coefficient * Enumerable
                    .SkipLast(secondPolynom, remainderIndex)
                    .Last();
        }

        var result = remainder.Select(resultselector).ToArray();

        return result;
    }

    private static int ToBinary(int number)
    {
        var absolute = Math.Abs(number);
        var result = absolute > 1 ? absolute % 2 : absolute;

        return result;
    }
}
