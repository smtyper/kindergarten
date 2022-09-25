using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace ComputerVision;

public class ImageOperations
{
    public static async ValueTask<Stream> ToGrayscaleAsync(Stream imageStream)
    {
        using var sourceImage = await Image.LoadAsync<Rgba32>(Configuration.Default, imageStream);

        using var grayscaleImage = sourceImage
            .Clone()
            .ForEachPixel(pixel =>
            {
                var average = (byte)((pixel.R + pixel.G + pixel.B) / 3);
                var updatedPixel = new Rgba32(average, average, average, pixel.A);

                return updatedPixel;
            });

        var resultStream = new MemoryStream();
        await grayscaleImage.SaveAsync(resultStream, new JpegEncoder());
        resultStream.Position = 0;

        return resultStream;
    }

    public static async ValueTask<Stream> ToNoisedAsync(Stream imageStream, double intensity = 0.5, byte mean = 25,
        byte sigma = 5)
    {
        using var sourceImage = await Image.LoadAsync<Rgba32>(Configuration.Default, imageStream);

        var random = new Random(DateTime.UtcNow.Millisecond);
        using var noisedImage = sourceImage
            .Clone()
            .ForEachPixel(pixel =>
            {
                if (random.NextDouble() < intensity)
                    return pixel;

                var gaussian = random.NextGaussian(mean, sigma);
                var r = (byte)(pixel.R + gaussian);
                var g = (byte)(pixel.G + gaussian);
                var b = (byte)(pixel.B + gaussian);

                var updatedPixel = new Rgba32(r, g, b, pixel.A);

                return updatedPixel;

            });

        var resultStream = new MemoryStream();
        await noisedImage.SaveAsync(resultStream, new JpegEncoder());
        resultStream.Position = 0;

        return resultStream;
    }
}
