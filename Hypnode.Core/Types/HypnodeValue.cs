namespace Hypnode.Core.Types;

public abstract record HypnodeValue
{
    public abstract HypnodeType Type { get; }
}

public sealed record IntValue(int V) : HypnodeValue
{
    public override HypnodeType Type => new HypnodeType.Primitive("int");
    public override string ToString() => V.ToString();
}

public sealed record FloatValue(float V) : HypnodeValue
{
    public override HypnodeType Type => new HypnodeType.Primitive("float");
    public override string ToString() => V.ToString();
}

public sealed record DoubleValue(double V) : HypnodeValue
{
    public override HypnodeType Type => new HypnodeType.Primitive("double");
    public override string ToString() => V.ToString();
}

public sealed record BoolValue(bool V) : HypnodeValue
{
    public override HypnodeType Type => new HypnodeType.Primitive("bool");
    public override string ToString() => V.ToString();
}

public sealed record ByteValue(byte V) : HypnodeValue
{
    public override HypnodeType Type => new HypnodeType.Primitive("byte");
    public override string ToString() => V.ToString();
}

public sealed record StringValue(string V) : HypnodeValue
{
    public override HypnodeType Type => new HypnodeType.Primitive("string");
    public override string ToString() => V;
}

public sealed record StructValue(HypnodeType.Struct TypeDef, IReadOnlyDictionary<string, HypnodeValue> Fields) : HypnodeValue
{
    public override HypnodeType Type => TypeDef;
}

public sealed record ArrayValue(HypnodeType.Array TypeDef, IReadOnlyList<HypnodeValue> Items) : HypnodeValue
{
    public override HypnodeType Type => TypeDef;
}

public sealed record TupleValue(IReadOnlyList<HypnodeValue> Items) : HypnodeValue
{
    public override HypnodeType Type => new HypnodeType.Tuple(Items.Select(i => i.Type).ToList());
}
