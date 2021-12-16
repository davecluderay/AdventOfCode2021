using System.Globalization;

namespace Aoc2021_Day16;

internal class Solution
{
    public string Title => "Day 16: Packet Decoder";

    public object? PartOne()
    {
        var data = ReadDataFromFile();

        var packet = PacketDecoder.Decode(data);

        return CalculateVersionTotal(packet);
    }

    public object? PartTwo()
    {
        var data = ReadDataFromFile();

        var packet = PacketDecoder.Decode(data);
        
        return packet.Evaluate();
    }

    private byte[] ReadDataFromFile()
    {
        var text = InputFile.ReadAllText();

        var data = new byte[text.Length / 2];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = byte.Parse(text.Substring(i * 2, 2), NumberStyles.HexNumber);
        }

        return data;
    }

    private static long CalculateVersionTotal(Packet packet)
    {
        var versionTotal = 0L;
        var next = new Stack<Packet>(new[] { packet });
        while (next.Count > 0)
        {
            var current = next.Pop();
            versionTotal += current.Version;
           
            foreach (var subPacket in current.SubPackets)
            {
                next.Push(subPacket);
            }
        }

        return versionTotal;
    }
}