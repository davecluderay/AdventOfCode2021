using System.Numerics;

namespace Aoc2021_Day19;

internal class Solution
{
    public string Title => "Day 19: Beacon Scanner";

    public object? PartOne()
    {
        var scannerData = ScannerData.ReadFromInputFile()
                                     .OrderBy(s => s.ScannerId)
                                     .ToArray();
        var map = BuildBeaconMap(scannerData);
        return map.Count;
    }

    public object? PartTwo()
    {
        var scannerData = ScannerData.ReadFromInputFile()
                                     .OrderBy(s => s.ScannerId)
                                     .ToArray();
        BuildBeaconMap(scannerData);
        return scannerData.InPairs()
                          .Max(s => CalculateDistance(s.First.Position,
                                                      s.Second.Position));
    }

    private static HashSet<(int X, int Y, int Z)> BuildBeaconMap(ScannerData[] scannerData)
    {
        var map = new HashSet<(int X, int Y, int Z)>(scannerData.First().Beacons);
        var remainingScanners = scannerData.Skip(1).ToList();

        while (remainingScanners.Any())
        {
            PlaceNextScanner(map, remainingScanners);
        }

        return map;
    }

    private static void PlaceNextScanner(HashSet<(int X, int Y, int Z)> map, List<ScannerData> remainingScanners)
    {
        var mapBeaconPairsByDifference = map.ToList()
                                            .InPairs()
                                            .OrderBy(p => p)
                                            .ToLookup(pair => CalculateDifference(pair.First, pair.Second));
        foreach (var scanner in remainingScanners)
        foreach (var transform in OrientationTransforms)
        {
            var transformedScannerBeacons = scanner.Beacons.Select(b => ApplyTransform(transform, b)).ToArray();

            // Look for pairs of beacons in the map that seem to be in the same orientation as pairs in the
            // scanner data. Use these matches to decide which positions to try.
            var positionsToTry = new List<(int X, int Y, int Z)>();
            foreach (var pair in transformedScannerBeacons.InPairs())
            {
                var difference = CalculateDifference(pair.First, pair.Second);
                positionsToTry.AddRange(mapBeaconPairsByDifference[difference].Select(m => CalculateDifference(pair.First, m.First)));
            }

            // Look for beacons that line up between the map and the scanner data when aligned to this position.
            // If there are more than twelve, add the scanner's data to the map at this position.
            foreach (var positionToTry in positionsToTry)
            {
                var translate = Matrix4x4.CreateTranslation(positionToTry.X, positionToTry.Y, positionToTry.Z);
                var beacons = transformedScannerBeacons.Select(b => ApplyTransform(translate, b)).ToArray();
                if (beacons.Count(map.Contains) >= 12)
                {
                    remainingScanners.Remove(scanner);
                    scanner.Position = positionToTry;
                    foreach (var beacon in beacons)
                    {
                        map.Add(beacon);
                    }
                    return;
                }
            }
        }

        throw new Exception("No scanner's data could be placed on the map!");
    }

    private static int CalculateDistance((int X, int Y, int Z) first, (int X, int Y, int Z) second)
        => Math.Abs(second.X - first.X) + Math.Abs(second.Y - first.Y) + Math.Abs(second.Z - first.Z);

    private static (int X, int Y, int Z) CalculateDifference((int X, int Y, int Z) first, (int X, int Y, int Z) second)
        => (second.X - first.X, second.Y - first.Y, second.Z - first.Z);

    private static (int X, int Y, int Z) ApplyTransform(Matrix4x4 transform, (int X, int Y, int Z) position)
    {
        var vector = Vector3.Transform(new Vector3(position.X, position.Y, position.Z), transform);
        return ((int)vector.X, (int)vector.Y, (int)vector.Z);
    }

    private static readonly Matrix4x4[] OrientationTransforms =
        {
            new (1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1),
            new (0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1),
            new (-1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1),
            new (0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1),
            new (0, 0, -1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1),
            new (0, 0, -1, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1),
            new (0, 0, -1, 0, 0, -1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1),
            new (0, 0, -1, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 1),
            new (-1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1),
            new (0, -1, 0, 0, -1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1),
            new (1, 0, 0, 0, 0, -1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1),
            new (0, 1, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1),
            new (0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1),
            new (0, 0, 1, 0, -1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 1),
            new (0, 0, 1, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1),
            new (0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1),
            new (1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1),
            new (0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1),
            new (-1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1),
            new (0, -1, 0, 0, 0, 0, 1, 0, -1, 0, 0, 0, 0, 0, 0, 1),
            new (-1, 0, 0, 0, 0, 0, -1, 0, 0, -1, 0, 0, 0, 0, 0, 1),
            new (0, -1, 0, 0, 0, 0, -1, 0, 1, 0, 0, 0, 0, 0, 0, 1),
            new (1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1),
            new (0, 1, 0, 0, 0, 0, -1, 0, -1, 0, 0, 0, 0, 0, 0, 1)
        };
}
