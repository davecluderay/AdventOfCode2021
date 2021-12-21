using System.Text;

namespace Aoc2021_Day20;

internal class Solution
{
    public string Title => "Day 20: Trench Map";

    public object? PartOne()
    {
        var (algorithm, image) = ReadImageDataFromFile();
        for (var step = 1; step <= 2; step++)
        {
            Apply(algorithm, image);
        }
        return image.LitPixelCount;
    }

    public object? PartTwo()
    {
        var (algorithm, image) = ReadImageDataFromFile();
        for (var step = 1; step <= 50; step++)
        {
            Apply(algorithm, image);
        }
        return image.LitPixelCount;
    }

    private static void Apply(string algorithm, InputImage image)
    {
        var actions = new List<Action>();
        for (var y = image.Bounds.MinY - 1; y <= image.Bounds.MaxY + 1; y++)
        for (var x = image.Bounds.MinX - 1; x <= image.Bounds.MaxX + 1; x++)
        {
            int algorithmIndex = 0;
            if (image.GetPixel((x - 1, y - 1)) == '#') algorithmIndex |= 0x100;
            if (image.GetPixel((x    , y - 1)) == '#') algorithmIndex |= 0x080;
            if (image.GetPixel((x + 1, y - 1)) == '#') algorithmIndex |= 0x040;
            if (image.GetPixel((x - 1, y    )) == '#') algorithmIndex |= 0x020;
            if (image.GetPixel((x    , y    )) == '#') algorithmIndex |= 0x010;
            if (image.GetPixel((x + 1, y    )) == '#') algorithmIndex |= 0x008;
            if (image.GetPixel((x - 1, y + 1)) == '#') algorithmIndex |= 0x004;
            if (image.GetPixel((x    , y + 1)) == '#') algorithmIndex |= 0x002;
            if (image.GetPixel((x + 1, y + 1)) == '#') algorithmIndex |= 0x001;

            var pixel = algorithm[algorithmIndex];
            var position = (x, y);
            actions.Add(() => image.SetPixel(position, pixel));
        }

        foreach (var action in actions)
        {
            action();
        }

        image.OutOfBoundsChar = image.OutOfBoundsChar == '#' ? algorithm.Last() : algorithm.First();
    }

    private static (string EnhancementAlgorithm, InputImage Image) ReadImageDataFromFile()
    {
        var enhancementAlgorithm = new StringBuilder();
        using var reader = new StringReader(InputFile.ReadAllText());

        while (true)
        {
            var line = reader.ReadLine();
            if (line is null || line.Length == 0) break;
            enhancementAlgorithm.Append(line);
        }

        var image = new Dictionary<(int X, int Y), char>();
        var y = 0;
        while (true)
        {
            var line = reader.ReadLine();
            if (line is null) break;

            for (var x = 0; x < line.Length; x++)
            {
                image[(x, y)] = line[x];
            }

            y++;
        }

        return (enhancementAlgorithm.ToString(), new InputImage(image));
    }
}
