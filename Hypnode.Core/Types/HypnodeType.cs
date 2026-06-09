namespace Hypnode.Core.Types;

public abstract record HypnodeType
{
    public sealed record Primitive(string Name) : HypnodeType;
    public sealed record Struct(string Name, IReadOnlyList<Field> Fields) : HypnodeType;
    public sealed record Array(HypnodeType Element) : HypnodeType;
    public sealed record Tuple(IReadOnlyList<HypnodeType> Elements) : HypnodeType;
    public sealed record Union(string Name, IReadOnlyList<HypnodeType> Variants) : HypnodeType;

    public record Field(string Name, HypnodeType Type);
}
