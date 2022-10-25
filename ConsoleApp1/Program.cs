﻿using System.Text;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var rnd = new Random(DateTime.Now.Millisecond);

            const string alph = "еёжзийк";
            const int n = 13;
            const int k = 8;

            var gen = new int[] { 1, 1, 1, 1, 0, 1 };

            var binArrays = GetBin(alph);
            for (var i = 0; i < alph.Length; i++)
            {
                Console.WriteLine($"Символ: {alph[i]}");
                var binArray = binArrays[i];
                Console.WriteLine($"Исходный многочлен: {string.Join("", binArray)}");

                var shift = Multi(binArray, n - k, 1);
                Mod2(shift);
                Console.WriteLine($"Информационный многочлен (со сдвигом на {n-k}): {string.Join("", shift)}");
                Devide(shift, gen, out var rem);
                Mod2(rem);
                Console.WriteLine($"Остаток от деления информационного многочлена на порождающий: {string.Join("", rem)}");
                var code = Sum(shift, rem);
                Mod2(code);

                Console.WriteLine($"Кодовое слово (сумма инфомационного многочлена и остатка от деления): {string.Join("", code)}");

                Console.WriteLine();

                var randIndex = rnd.Next(0, 13);
                code[randIndex] = code[randIndex] == 0 ? 1 : 0;
                Console.WriteLine($"Внесение одиночной ошибки на позицию {randIndex}: {string.Join("", code)}");

                Devide(code, gen, out var sindrom);
                Mod2(sindrom);
                Console.WriteLine($"Синдром кода: {string.Join("", sindrom)}");

                Console.WriteLine("\n\n");
            }
        }

        public static int[][] GetBin(string str)
        {
            var binArrays = new int[str.Length][];

            for (var i = 0; i < binArrays.Length; i++)
            {
                var bytes = Encoding.GetEncoding(1251).GetBytes(str[i].ToString());
                var stringBuilder = new StringBuilder();
                foreach (var e in bytes)
                    stringBuilder.Append(Convert.ToString(e, 2).PadLeft(8, '0'));

                var symBinArray = new int[stringBuilder.Length];
                for (var j = 0; j < symBinArray.Length; j++)
                    symBinArray[j] = int.Parse(stringBuilder[j].ToString());

                binArrays[i] = symBinArray;
            }

            return binArrays;
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

            return quotient;
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
            var lesser = bigger == x ? y : x;
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
    }
}
