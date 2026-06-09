using Hypnode.Core.Types;

namespace Hypnode.Logic;

public sealed record LogicPacket(LogicValue V) : HypnodeValue
{
    public override HypnodeType Type => new HypnodeType.Primitive("LogicValue");
    public override string ToString() => V.ToString();
}

public static class LogicPacketExtensions
{
    public static LogicValue AsLogic(this HypnodeValue v) =>
        v is LogicPacket lp ? lp.V : throw new InvalidCastException($"Expected LogicPacket, got {v.GetType().Name}");
}
