namespace Aoc2021_Day21;

internal class Solution
{
    public string Title => "Day 21: Dirac Dice";

    public object? PartOne()
    {
        var (player1StartPosition, player2StartPosition) = ReadStartingPositionsFromFile();
        var (player1Score, player2Score, numberOfRolls) = PlayGameUsingDeterministicDie(player1StartPosition, player2StartPosition);

        return Math.Min(player1Score, player2Score) * numberOfRolls;
    }

    public object? PartTwo()
    {
        var (player1StartPosition, player2StartPosition) = ReadStartingPositionsFromFile();
        var (player1Wins, player2Wins) = CalculateScoreDistributionsWithQuantumDie(player1StartPosition, player2StartPosition);
        return Math.Max(player1Wins, player2Wins);
    }

    private static (int Player1Score, int Player2Score, int numberOfRolls) PlayGameUsingDeterministicDie(int player1StartPosition, int player2StartPosition, int winningScore = 1000)
    {
        var player1 = (Score: 0, Position: player1StartPosition);
        var player2 = (Score: 0, Position: player2StartPosition);
        var numberOfRolls = 0;

        int roll() => (++numberOfRolls - 1) % 10 + 1;

        while (true)
        {
            var player1TurnTotal = roll() + roll() + roll();
            player1.Position = (player1.Position + player1TurnTotal - 1) % 10 + 1;
            player1.Score += player1.Position;
            if (player1.Score >= 1000) break;

            var player2TurnTotal = roll() + roll() + roll();
            player2.Position = (player2.Position + player2TurnTotal - 1) % 10 + 1;
            player2.Score += player2.Position;
            if (player2.Score >= 1000) break;
        }

        return (player1.Score, player2.Score, numberOfRolls);
    }

    private static (long Player1Wins, long Player2Wins) CalculateScoreDistributionsWithQuantumDie(int player1StartPosition, int player2StartPosition, int winningScore = 21)
    {
        // Pre-calculate the possible turn scores and how frequently they occur.
        var possibleTurnTotals = (from roll1 in Enumerable.Range(1, 3)
                                  from roll2 in Enumerable.Range(1, 3)
                                  from roll3 in Enumerable.Range(1, 3)
                                  select roll1 + roll2 + roll3).GroupBy(x => x)
                                                               .ToDictionary(g => g.Key, g => g.Count());

        var calculateCache = new Dictionary<((int, int), (int, int)), (long, long)>();
        return Calculate((0, player1StartPosition), (0, player2StartPosition));

        (long Player1Wins, long Player2Wins) Calculate((int Score, int Position) player1, (int Score, int Position) player2)
        {
            if (!calculateCache.ContainsKey((player1, player2)))
            {
                var (player1Wins, player2Wins) = (0L, 0L);

                foreach (var (player1TurnTotal, player1TurnTotalFrequency) in possibleTurnTotals)
                {
                    var player1NewPosition = (player1.Position + player1TurnTotal - 1) % 10 + 1;
                    var player1NewScore = player1.Score + player1NewPosition;
                    if (player1NewScore >= winningScore)
                    {
                        player1Wins += player1TurnTotalFrequency;
                        continue;
                    }

                    foreach (var (player2TurnTotal, player2TurnTotalFrequency) in possibleTurnTotals)
                    {
                        var player2NewPosition = (player2.Position + player2TurnTotal - 1) % 10 + 1;
                        var player2NewScore = player2.Score + player2NewPosition;
                        if (player2NewScore >= winningScore)
                        {
                            player2Wins += player1TurnTotalFrequency * player2TurnTotalFrequency;
                            continue;
                        }

                        var result = Calculate((player1NewScore, player1NewPosition), (player2NewScore, player2NewPosition));
                        player1Wins += result.Player1Wins * player1TurnTotalFrequency * player2TurnTotalFrequency;
                        player2Wins += result.Player2Wins * player1TurnTotalFrequency * player2TurnTotalFrequency;
                    }
                }

                calculateCache[(player1, player2)] = (player1Wins, player2Wins);
            }

            return calculateCache[(player1, player2)];;
        }
    }

    private static (int Player1, int Player2) ReadStartingPositionsFromFile()
    {
        var positions = InputFile.ReadAllLines()
                                 .Select(line => Convert.ToInt32(line.Split(":", StringSplitOptions.TrimEntries).Last()));
        return (positions.First(), positions.Last());
    }
}
