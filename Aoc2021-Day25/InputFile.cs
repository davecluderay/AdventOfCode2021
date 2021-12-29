using System.Reflection;

namespace Aoc2021_Day25;

internal static class InputFile
{
    public static string[] ReadAllLines(string? fileName = null)
    {
        var directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? ".";
        var filePath      = Path.Combine(directoryPath, fileName ?? "input.txt");
        if (!File.Exists(filePath)) throw new Exception($"File not found: {filePath}");

        return File.ReadAllLines(filePath);
    }
        
    public static string ReadAllText(string? fileName = null)
    {
        var directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? ".";
        var filePath      = Path.Combine(directoryPath, fileName ?? "input.txt");
        if (!File.Exists(filePath)) throw new Exception($"File not found: {filePath}");

        return File.ReadAllText(filePath);
    }
}