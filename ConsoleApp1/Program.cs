namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {

        }

        public static int[] Multi(int[] x, int d, int c)
        {
            var size = d + x.Length - 1;
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
                sum[i] = bigger[i] + i < lesser.Length ? lesser[i] : 0;

            return sum;
        }
    }
}
