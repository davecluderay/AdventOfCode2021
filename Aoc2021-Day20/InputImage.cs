using System.Text;

namespace Aoc2021_Day20;

internal class InputImage
{
    private readonly Dictionary<(int X, int Y), char> _image = new();
    public (int MinX, int MinY, int MaxX, int MaxY) Bounds { get; private set; } = (0, 0, 0, 0);

    public char OutOfBoundsChar { get; set; } = '.';

    public int LitPixelCount => OutOfBoundsChar == '#' ? int.MaxValue : _image.Values.Count(v => v == '#');

    public InputImage(Dictionary<(int X, int Y), char> data)
    {
        _image = data;
        Bounds = (_image.Keys.Min(k => k.X),
                   _image.Keys.Min(k => k.Y),
                   _image.Keys.Max(k => k.X),
                   _image.Keys.Max(k => k.Y));
    }

    public char GetPixel((int X, int Y) position)
    {
        if (position.X < Bounds.MinX || position.X > Bounds.MaxX || position.Y < Bounds.MinY || position.Y > Bounds.MaxY)
            return OutOfBoundsChar;
        return _image.GetValueOrDefault(position, '.');
    }

    public void SetPixel((int X, int Y) position, char value)
    {
        _image[position] = value;

        Bounds = (Math.Min(Bounds.MinX, position.X),
                  Math.Min(Bounds.MinY, position.Y),
                  Math.Max(Bounds.MaxX, position.X),
                  Math.Max(Bounds.MaxY, position.Y));
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        for (var y = Bounds.MinY; y <= Bounds.MaxY; y++)
        {
            for (var x = Bounds.MinX; x <= Bounds.MaxX; x++)
            {
                result.Append(GetPixel((x, y)));
            }
            result.AppendLine();
        }

        return result.ToString();
    }
}
