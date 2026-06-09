namespace Hypnode.Core.Types;

public static class HypnodeValueExtensions
{
    public static int AsInt(this HypnodeValue v) =>
        v is IntValue(var n) ? n : throw new InvalidCastException($"Expected IntValue, got {v.GetType().Name}");

    public static float AsFloat(this HypnodeValue v) =>
        v is FloatValue(var n) ? n : throw new InvalidCastException($"Expected FloatValue, got {v.GetType().Name}");

    public static double AsDouble(this HypnodeValue v) =>
        v is DoubleValue(var n) ? n : throw new InvalidCastException($"Expected DoubleValue, got {v.GetType().Name}");

    public static bool AsBool(this HypnodeValue v) =>
        v is BoolValue(var n) ? n : throw new InvalidCastException($"Expected BoolValue, got {v.GetType().Name}");

    public static byte AsByte(this HypnodeValue v) =>
        v is ByteValue(var n) ? n : throw new InvalidCastException($"Expected ByteValue, got {v.GetType().Name}");

    public static string AsString(this HypnodeValue v) =>
        v is StringValue(var n) ? n : throw new InvalidCastException($"Expected StringValue, got {v.GetType().Name}");
}
