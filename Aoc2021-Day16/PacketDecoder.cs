namespace Aoc2021_Day16;

internal static class PacketDecoder
{
    public static Packet Decode(IReadOnlyList<byte> data)
        => DecodeAt(data, 0).Packet; 

    private static (Packet Packet, int NewPosition) DecodeAt(IReadOnlyList<byte> data, int position)
    {
        (var version, position) = ReadVersion(data, position);
        (var type, position) = ReadPacketType(data, position);

        if (type == PacketType.Literal)
        {
            (var value, position) = ReadLiteral(data, position);
            return (new Packet(version, type, Array.Empty<Packet>(), value), position);
        }

        var subPackets = new List<Packet>();
        
        (var lengthType, position) = ReadSubPacketLengthType(data, position);
        switch (lengthType)
        {
            case SubPacketLengthType.TotalBitLength:
                (var totalSubPacketLength, position) = (ReadBits(data, position, 15), position + 15);
                var subPacketEndPosition = position + totalSubPacketLength;

                while (position < subPacketEndPosition)
                {
                    (var subPacket, position) = DecodeAt(data, position);
                    subPackets.Add(subPacket);
                }

                break;
            case SubPacketLengthType.Count:
                (var subPacketCount, position) = (ReadBits(data, position, 11), position + 11);

                for (var i = 0; i < subPacketCount; i++)
                {
                    (var subPacket, position) = DecodeAt(data, position);
                    subPackets.Add(subPacket);
                }

                break;
            default:
                throw new InvalidOperationException($"Unexpected sub-packet length type: {lengthType}");
                
        }
        
        return (new Packet(version, type, subPackets.ToArray(), 0), position);
    }

    private static (long Version, int newPosition) ReadVersion(IReadOnlyList<byte> data, int position)
    {
        return (ReadBits(data, position, 3), position + 3);
    }

    private static (PacketType PacketType, int newPosition) ReadPacketType(IReadOnlyList<byte> data, int position)
    {
        return ((PacketType)ReadBits(data, position, 3), position + 3);
    }

    private static (SubPacketLengthType Mode, int newPosition) ReadSubPacketLengthType(IReadOnlyList<byte> data, int position)
    {
        return ((SubPacketLengthType)ReadBits(data, position, 1), position + 1);
    }

    private static (long Value, int newPosition) ReadLiteral(IReadOnlyList<byte> data, int position)
    {
        long value = 0;
        var bitsRead = 0;
        while (true)
        {
            var chunk = ReadBits(data, position + bitsRead, 5);
            bitsRead += 5;
            value = (value << 4) | (chunk & 0xF);
            if ((chunk & 0x10) == 0) break;
        }

        return (value, position + bitsRead);
    }

    private static long ReadBits(IReadOnlyList<byte> data, int position, int count)
    {
        long result = 0;

        var bitsRead = 0;
        var byteIndex = position / 8;
        var bitOffset = position % 8;

        while (bitsRead < count)
        {
            var bitsToRead = Math.Min(count - bitsRead, 8 - bitOffset);

            var read = ((uint)data[byteIndex] >> (8 - bitOffset - bitsToRead)) & ((uint)0xFF >> (8 - bitsToRead));
        
            result = (result << bitsToRead) + read;

            bitOffset += bitsToRead;
            if (bitOffset == 8)
            {
                byteIndex++;
                bitOffset = 0;
            }
        
            bitsRead += bitsToRead;
        }

        return result;
    }
}

internal enum PacketType
{
    Sum = 0,
    Product = 1,
    Min = 2,
    Max = 3,
    Literal = 4,
    GreaterThan = 5,
    LessThan = 6,
    Equal = 7
}

internal enum SubPacketLengthType
{
    TotalBitLength = 0,
    Count = 1
}

internal record Packet(long Version, PacketType Type, Packet[] SubPackets, long Value)
{
    public long Evaluate()
        => Type switch
        {
            PacketType.Sum         => SubPackets.Aggregate(0L, (a, v) => a + v.Evaluate()),
            PacketType.Product     => SubPackets.Aggregate(1L, (a, v) => a * v.Evaluate()),
            PacketType.Min         => SubPackets.Aggregate(long.MaxValue, (a, v) => Math.Min(a, v.Evaluate())),
            PacketType.Max         => SubPackets.Aggregate(0L, (a, v) => Math.Max(a, v.Evaluate())),
            PacketType.Literal     => Value,
            PacketType.GreaterThan => SubPackets.First().Evaluate() > SubPackets.Last().Evaluate() ? 1L : 0L,
            PacketType.LessThan    => SubPackets.First().Evaluate() < SubPackets.Last().Evaluate() ? 1L : 0L,
            PacketType.Equal       => SubPackets.First().Evaluate() == SubPackets.Last().Evaluate() ? 1L : 0L,
            _                      => throw new InvalidOperationException($"Unexpected packet type: {Type}")
        };
}