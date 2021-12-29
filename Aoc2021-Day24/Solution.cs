using System.Text.RegularExpressions;

namespace Aoc2021_Day24;

internal class Solution
{
    public string Title => "Day 24: Arithmetic Logic Unit";

    public object? PartOne()
    {
        var program = ReadProgramFromInputFile();
        var constraints = InferInputConstraintsFromProgram(program);
        var modelNumber = GetHighestValidModelNumber(constraints);
        if (ExecuteProgram(program, modelNumber) != 0L)
            throw new Exception($"Model number {modelNumber} is not valid.");
        return modelNumber;
    }

    public object? PartTwo()
    {
        var program = ReadProgramFromInputFile();
        var constraints = InferInputConstraintsFromProgram(program);
        var modelNumber = GetLowestValidModelNumber(constraints);
        if (ExecuteProgram(program, modelNumber) != 0L)
            throw new Exception($"Model number {modelNumber} is not valid.");
        return modelNumber;
    }

    private static string GetHighestValidModelNumber(InputConstraint[] constraints)
    {
        var digits = new char[14];
        foreach (var constraint in constraints)
        {
            var delta = constraint.Delta;
            var (_, max) = delta < 0
                               ? (-1 * delta + 1, 9)
                               : (1, 9 - delta);
            digits[constraint.RelativeToPosition] = (char)(max + '0');
            digits[constraint.AtPosition] = (char)(max + '0' + delta);
        }

        return new string(digits);
    }

    private static string GetLowestValidModelNumber(InputConstraint[] constraints)
    {
        var digits = new char[14];
        foreach (var constraint in constraints)
        {
            var delta = constraint.Delta;
            var (min, _) = delta < 0
                               ? (-1 * delta + 1, 9)
                               : (1, 9 - delta);
            digits[constraint.RelativeToPosition] = (char)(min + '0');
            digits[constraint.AtPosition] = (char)(min + '0' + delta);
        }

        return new string(digits);
    }

    private static long ExecuteProgram(ProgramInstruction[] program, string input)
    {
        var inputs = new InputProvider(input);
        var variables = new Dictionary<string, long>
                        {
                            ["w"] = 0,
                            ["x"] = 0,
                            ["y"] = 0,
                            ["z"] = 0
                        };

        foreach (var (instruction, a, b) in program)
        {
            switch (instruction)
            {
                case "inp":
                    variables[a] = inputs.Read();
                    break;
                case "add":
                    variables[a] += variables.ContainsKey(b)
                                        ? variables[b]
                                        : Convert.ToInt64(b);
                    break;
                case "mul":
                    variables[a] *= variables.ContainsKey(b)
                                        ? variables[b]
                                        : Convert.ToInt64(b);
                    break;
                case "div":
                    variables[a] /= variables.ContainsKey(b)
                                        ? variables[b]
                                        : Convert.ToInt64(b);
                    break;
                case "mod":
                    variables[a] %= variables.ContainsKey(b)
                                        ? variables[b]
                                        : Convert.ToInt64(b);
                    break;
                case "eql":
                    variables[a] = variables[a].Equals(variables.ContainsKey(b)
                                                           ? variables[b]
                                                           : Convert.ToInt64(b))
                                       ? 1L
                                       : 0L;
                    break;
            }
        }

        return variables["z"];
    }

    private InputConstraint[] InferInputConstraintsFromProgram(ProgramInstruction[] program)
    {
        if (program.Length != 18 * 14)
            throw new ArgumentException($"Unexpected program length (expected {18 * 14}, actually {program.Length}).");

        // Instructions at (18n + 4), (18n + 5) and (18n + 15) have secondary operands that represent
        // constants a, b and c for digit n.
        // Wherever b <= 0, this digit involves a pop from z. To prevent an additional push to z,
        // this digit must have a specific value relative to the corresponding earlier push to z.

        var pushes = new Stack<(int Position, int C)>();
        var constraints = new List<InputConstraint>();
        for (var position = 0; position < 14; position++)
        {
            var (b, c) = (Convert.ToInt32(program[position * 18 + 5].SecondaryOperand),
                          Convert.ToInt32(program[position * 18 + 15].SecondaryOperand));
            if (b <= 0)
            {
                var relativeTo = pushes.Pop();
                constraints.Add(new InputConstraint(position, relativeTo.Position, relativeTo.C + b));
            }
            else
            {
                pushes.Push((position, c));
            }
        }

        if (pushes.Any())
            throw new ArgumentException($"Unexpected program structure, unable to infer constraints.");

        return constraints.ToArray();
    }

    private static ProgramInstruction[] ReadProgramFromInputFile()
    {
        var inputPattern = new Regex(@"^(?<Instruction>(inp|add|mul|div|mod|eql)) (?<PrimaryOperand>[wxyz])( (?<SecondaryOperand>([wxyz]|[-]?\d+)))?$", RegexOptions.Compiled | RegexOptions.Singleline);
        return InputFile.ReadAllLines()
                        .Select(l => inputPattern.Match(l))
                        .Where(m => m.Success)
                        .Select(m => new ProgramInstruction(m.Groups["Instruction"].Value,
                                                            m.Groups["PrimaryOperand"].Value,
                                                            m.Groups["SecondaryOperand"].Value))
                        .ToArray();
    }

    private class InputProvider
    {
        private readonly IEnumerator<char> _enumerator;

        public InputProvider(string input)
        {
            _enumerator = input.GetEnumerator();
        }

        public long Read()
        {
            if (!_enumerator.MoveNext()) throw new Exception("No more inputs!");
            return _enumerator.Current - '0';
        }
    }

    private record ProgramInstruction(string Instruction, string PrimaryOperand, string SecondaryOperand);
    private record InputConstraint(int AtPosition, int RelativeToPosition, int Delta);
}
