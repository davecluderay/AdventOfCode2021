namespace Aoc2021_Day22;

internal record Cuboid(int X1, int X2, int Y1, int Y2, int Z1, int Z2)
{
    public long CalculateCubeCount() => 1L * (X2 - X1 + 1) * (Y2 - Y1 + 1) * (Z2 - Z1 + 1);

    public bool Overlaps(Cuboid other)
        => X1 <= other.X2 && X2 >= other.X1 &&
           Y1 <= other.Y2 && Y2 >= other.Y1 &&
           Z1 <= other.Z2 && Z2 >= other.Z1;

    public bool CompletelyContains(Cuboid other)
        => other.X1 >= X1 && other.X2 <= X2 &&
           other.Y1 >= Y1 && other.Y2 <= Y2 &&
           other.Z1 >= Z1 && other.Z2 <= Z2;

    public Cuboid[] Subtract(Cuboid other)
    {
        // Divide into as many as 27 smaller cuboids, based on where the other cube overlaps.
        // Return just those smaller cuboids that are outside of the overlapping region.

        if (other.CompletelyContains(this))
        {
            return Array.Empty<Cuboid>();
        }

        if (!Overlaps(other))
        {
            return new[] { this };
        }

        var results = from x in GetDividedExtents((X1, X2), (other.X1, other.X2))
                      from y in GetDividedExtents((Y1, Y2), (other.Y1, other.Y2))
                      from z in GetDividedExtents((Z1, Z2), (other.Z1, other.Z2))
                      let subdivision = new Cuboid(x.Low, x.High, y.Low, y.High, z.Low, z.High)
                      where !subdivision.Overlaps(other)
                      select subdivision;
        return results.ToArray();
    }

    private static IEnumerable<(int Low, int High)> GetDividedExtents((int Low, int High) target, (int Low, int High) splitPoints)
    {
        var dividedExtents = new List<(int Low, int High)>(3) { target };
    
        if (splitPoints.Low > target.Low && splitPoints.Low <= target.High)
        {
            dividedExtents.RemoveAt(dividedExtents.Count - 1);
            dividedExtents.Add((target.Low, splitPoints.Low - 1));
            dividedExtents.Add((splitPoints.Low, target.High));
        }

        if (splitPoints.High >= target.Low && splitPoints.High < target.High)
        {
            dividedExtents.RemoveAt(dividedExtents.Count - 1);
            dividedExtents.Add((dividedExtents.Any() ? dividedExtents.Last().High + 1 : target.Low, splitPoints.High));
            dividedExtents.Add((splitPoints.High + 1, target.High));
        }

        return dividedExtents;
    }
}