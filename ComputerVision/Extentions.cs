using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ComputerVision;

internal static class Extentions
{
    public static double NextGaussian(this Random random, double mean, double sigma)
    {
        // Box–Muller transform
        var x = random.NextDouble();
        var y = random.NextDouble();

        var z = Math.Sin(2 * Math.PI * y) * Math.Sqrt(-2 * Math.Log(x));
        var result = mean + (sigma * z);

        return result;
    }

    public static Image ForEachPixel<T>(this Image<T> image, Func<T, T> function) where T : unmanaged, IPixel<T>
    {
        foreach (var (x, y) in Enumerable
                     .Range(0, image.Width)
                     .SelectMany(x => Enumerable
                         .Range(0, image.Height)
                         .Select(y => (x, y))))
            image[x, y] = function(image[x, y]);

        return image;
    }
}
