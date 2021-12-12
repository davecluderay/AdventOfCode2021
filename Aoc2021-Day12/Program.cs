using Aoc2021_Day12;

try
{
    var solution = new Solution();
    
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(solution.Title);
    Console.WriteLine(new string('-', solution.Title.Length));
    Console.ResetColor();
    Console.WriteLine();

    var result1 = solution.PartOne();
    Console.Write("Part One Result: ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(result1);
    Console.ResetColor();
    Console.WriteLine();

    var result2 = solution.PartTwo();
    Console.Write("Part Two Result: ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(result2);
    Console.ResetColor();
    Console.WriteLine();
}
catch (Exception exception)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine(exception);
    Console.ResetColor();
}
finally
{
    Console.WriteLine("Done.");
}