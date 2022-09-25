using System.Reflection;
using ComputerVision;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;

namespace Tests;

public static class ImageOperationsTest
{
    [Test]
    public static async Task ToGrayscaleTest()
    {
        var assembly = Assembly.GetExecutingAssembly();
        await using var imageStream = assembly.GetManifestResourceStream(
                                          $"{nameof(Tests)}.{nameof(Resources)}.C'Thulhu.jpg") ??
                                      throw new NullReferenceException();

        await using var grayscaleStream = await ImageOperations.ToGrayscaleAsync(imageStream);

        var resultFilePath = Path.Combine("data", "grayscale.jpg");

        Directory.CreateDirectory(Path.GetDirectoryName(resultFilePath) ?? throw new NullReferenceException());
        await using var fileStream = File.Create(resultFilePath);
        await grayscaleStream.CopyToAsync(fileStream);
    }

    [Test]
    public static async Task ToNoisedTest()
    {
        var assembly = Assembly.GetExecutingAssembly();
        await using var imageStream = assembly.GetManifestResourceStream(
                                          $"{nameof(Tests)}.{nameof(Resources)}.C'Thulhu.jpg") ??
                                      throw new NullReferenceException();

        await using var noisedStream = await ImageOperations.ToNoisedAsync(imageStream);

        var resultFilePath = Path.Combine("data", "noised.jpg");

        Directory.CreateDirectory(Path.GetDirectoryName(resultFilePath) ?? throw new NullReferenceException());
        await using var fileStream = File.Create(resultFilePath);
        await noisedStream.CopyToAsync(fileStream);
    }
}
