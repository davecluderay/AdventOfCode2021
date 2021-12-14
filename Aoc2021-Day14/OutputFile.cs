using System.Reflection;

namespace Aoc2021_Day14;

internal static class OutputFile
{
    public static void WriteAllLines(IEnumerable<string> lines, string? fileName = null)
    {
        var directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? ".";
        var filePath      = Path.Combine(directoryPath, fileName ?? "output.txt");

        File.WriteAllLines(filePath, lines);
    }
        
    public static void WriteAllText(string content, string? fileName = null)
    {
        var directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? ".";
        var filePath      = Path.Combine(directoryPath, fileName ?? "output.txt");

        File.WriteAllText(filePath, content);
    }
}