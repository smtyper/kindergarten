using System.Text;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string alph = "еёжзийк";
            const int n = 13;
            const int k = 8;

            var text = "t";
            var gen = new int[] { 1, 1, 1, 1, 0, 1 };

            var binaryArray = GetBin(text);
            var shift = Multi(binaryArray, n - k, 1);
            Mod2(shift);
            var dev = Devide(shift, gen, out var rem);
            Mod2(rem);
            var code = Sum(shift, rem);
            Mod2(code);
        }

        public static int[] GetBin(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var stringBuilder = new StringBuilder();
            foreach (var e in bytes)
                stringBuilder.Append(Convert.ToString(e, 2).PadLeft(8, '0'));

            var binArray = new int[stringBuilder.Length];
            for (var i = 0; i < binArray.Length; i++)
                binArray[i] = int.Parse(stringBuilder[i].ToString());

            return binArray;
        }

        public static int[] Devide(int[] dividend, int[] divisor, out int[] remainder)
        {
            remainder = (int[])dividend.Clone();
            var quotient = new int[remainder.Length - divisor.Length + 1];
            for (var i = 0; i < quotient.Length; i++)
            {
                var c = remainder[remainder.Length - i - 1] / divisor[divisor.Length - 1];
                quotient[quotient.Length - i - 1] = c;
                for (var j = 0; j < divisor.Length; j++)
                    remainder[remainder.Length - i - j - 1] -= c * divisor[divisor.Length - j - 1];
            }

            remainder = Trim(remainder);

            return Trim(quotient);
        }

        public static int[] Multi(int[] x, int[] y)
        {
            var multi = new int[0];

            for (var i = 0; i < y.Length; i++)
                multi = Sum(multi, Multi(x, i, y[i]));

            return multi;
        }

        public static int[] Multi(int[] x, int d, int c)
        {
            var size = d + x.Length;
            var multi = new int[size];

            for (var i = 0; i < x.Length; i++)
                multi[d + i] = x[i] * c;

            return multi;
        }

        public static int[] Sum(int[] x, int[] y)
        {
            var bigger = x.Length > y.Length ? x : y;
            var lesser = x.Length < y.Length ? x : y;
            var sum = new int[bigger.Length];
            for (var i = 0; i < bigger.Length; i++)
                sum[i] = bigger[i] + (i < lesser.Length ? lesser[i] : 0);

            return sum;
        }

        public static void Mod2(int[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                var abs = Math.Abs(array[i]);
                array[i] = abs;

                if (abs > 1)
                    array[i] = abs % 2;
            }
        }

        public static int[] Trim(int[] array) => array.Reverse().SkipWhile(num => num == 0).Reverse().ToArray();
    }
}
