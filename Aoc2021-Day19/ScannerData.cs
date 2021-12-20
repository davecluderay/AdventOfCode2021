using System.Text.RegularExpressions;

namespace Aoc2021_Day19;

internal record ScannerData(int ScannerId, (int X, int Y, int Z)[] Beacons)
{
    private static readonly Regex EmptyLinePattern = new (@"\r?\n\r?\n", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    private static readonly Regex ScannerPattern = new (@"^--- scanner (?<ID>\d+) ---$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
    private static readonly Regex BeaconPattern = new (@"^(?<X>-?\d+),(?<Y>-?\d+),(?<Z>-?\d+)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline);

    public (int X, int Y, int Z)? Position { get; set; }
    public static ScannerData[] ReadFromInputFile()
    {
        return EmptyLinePattern.Split(InputFile.ReadAllText())
                               .Select(Parse)
                               .ToArray();
    }

    public static ScannerData Parse(string section)
    {
        var scannerId = Convert.ToInt32(ScannerPattern.Match(section).Groups["ID"].Value);
        var beacons = BeaconPattern.Matches(section)
                                   .Select(m => (Convert.ToInt32(m.Groups["X"].Value),
                                                 Convert.ToInt32(m.Groups["Y"].Value),
                                                 Convert.ToInt32(m.Groups["Z"].Value)))
                                   .ToArray();
        var location = scannerId == 0 ? (0, 0, 0) : ((int,int,int)?)null;
        return new ScannerData(scannerId, beacons) { Position = location };
    }
}
