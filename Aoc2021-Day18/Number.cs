using System.Diagnostics;

namespace Aoc2021_Day18;

internal sealed class NumberPair : Number
{
    public Number Left { get; set; }
    public Number Right { get; set; }
    public override long Magnitude => Left.Magnitude * 3 + Right.Magnitude * 2;

    public NumberPair(Number left, Number right) => (Left, Right) = (left, right);
    public override Number Copy() => new NumberPair(Left.Copy(), Right.Copy());
    public override string ToString() => $"[{Left},{Right}]";
}

internal sealed class RegularNumber : Number
{
    public int Value { get; set; }
    public override long Magnitude => Value;

    public RegularNumber(int value) => Value = value;
    public override Number Copy() => new RegularNumber(Value);
    public override string ToString() => Value.ToString();
}

internal abstract class Number
{
    public abstract long Magnitude { get; }
    public abstract Number Copy();

    public static Number operator +(Number left, Number right)
        => Reduce(new NumberPair(left, right));

    private static Number Reduce(Number number)
    {
        number = number.Copy();

        while (true)
        {
            var allElements = GetAllElements(number).ToArray();

            var (explodeAtIndex, splitAtIndex) = (-1, -1);
            for (var index = 0; index < allElements.Length; index++)
            {
                var (num, depth) = allElements[index];
                if (num is NumberPair pair && depth >= 4 && pair.Left is RegularNumber && pair.Right is RegularNumber)
                {
                    explodeAtIndex = index;
                    break;
                }

                if (splitAtIndex < 0 && num is RegularNumber { Value: >= 10 })
                {
                    splitAtIndex = index;
                }
            }

            if (explodeAtIndex >= 0)
            {
                Explode(allElements, explodeAtIndex);
                continue;
            }

            if (splitAtIndex >= 0)
            {
                Split(allElements, splitAtIndex);
                continue;
            }

            break;
        }

        return number;
    }

    private static IEnumerable<(Number Number, int Depth)> GetAllElements(Number number)
    {
        yield return (number, 0);

        if (number is RegularNumber) yield break;

        var pair = (NumberPair)number;

        foreach (var (num, depth) in GetAllElements(pair.Left))
            yield return (num, depth + 1);

        foreach (var (num, depth) in GetAllElements(pair.Right))
            yield return (num, depth + 1);
    }

    private static void Explode(IReadOnlyList<(Number Number, int Depth)> allElements, int atIndex)
    {
        var pair = (NumberPair)allElements[atIndex].Number;

        // Update the next regular number to the left (if any).
        for (var index = atIndex - 1; index >= 0; index--)
        {
            if (allElements[index].Number is RegularNumber r)
            {
                r.Value += ((RegularNumber)pair.Left).Value;
                break;
            }
        }

        // Update the next regular number to the right (if any).
        for (var index = atIndex + 3; index < allElements.Count; index++)
        {
            if (allElements[index].Number is RegularNumber r)
            {
                r.Value += ((RegularNumber)pair.Right).Value;
                break;
            }
        }

        // Replace the pair itself with a zero.
        var depth = allElements[atIndex].Depth;
        if (allElements[atIndex - 1].Depth < depth)
            ((NumberPair)allElements[atIndex - 1].Number).Left = new RegularNumber(0);
        else if (allElements[atIndex - 2].Depth < depth)
            ((NumberPair)allElements[atIndex - 2].Number).Right = new RegularNumber(0);
    }

    private static void Split(IReadOnlyList<(Number Number, int Depth)> allElements, int atIndex)
    {
        // Split the regular number into a pair.
        var value = ((RegularNumber)allElements[atIndex].Number).Value;
        var left = new RegularNumber((value - value % 2) / 2);
        var right = new RegularNumber((value + value % 2) / 2);
        var newPair = new NumberPair(left, right);

        // Replace the regular number with the new pair.
        var depth = allElements[atIndex].Depth;
        for (var index = atIndex - 1; index >= 0; index--)
        {
            if (allElements[index].Depth < depth)
            {
                ((NumberPair)allElements[index].Number).Left = newPair;
                break;
            }

            if (allElements[index].Depth == depth)
            {
                ((NumberPair)allElements[index - 1].Number).Right = newPair;
                break;
            }
        }
    }

    public static Number Parse(string input)
    {
        var (result, _) = ReadAt(input, 0);
        return result;

        (Number Number, int NextIndex) ReadAt(string input, int atIndex)
        {
            if (atIndex >= input.Length) throw new ArgumentException("Input is empty.");

            if (input[atIndex] == '[')
            {
                (var left, atIndex) = ReadAt(input, atIndex + 1);
                if (input[atIndex] != ',') throw new ArgumentException($"Expected ',' at index {atIndex}. Actual: '{input[atIndex]}");

                (var right, atIndex) = ReadAt(input, atIndex + 1);
                if (input[atIndex] != ']') throw new ArgumentException($"Expected ']' at index {atIndex}. Actual: '{input[atIndex]}");

                return (new NumberPair(left, right), atIndex + 1);
            }

            if (char.IsNumber(input[atIndex]))
            {
                var length = 1;
                while (atIndex + length < input.Length && char.IsNumber(input[atIndex + length]))
                    length++;

                var value = Convert.ToInt32(input.Substring(atIndex, length));
                return (new RegularNumber(value), atIndex + length);
            }

            throw new ArgumentException($"Unexpected character '{input[atIndex]}' at index {atIndex}.");
        }
    }
}
