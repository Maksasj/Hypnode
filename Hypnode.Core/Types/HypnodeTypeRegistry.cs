namespace Hypnode.Core.Types;

public class HypnodeTypeRegistry
{
    private readonly Dictionary<string, HypnodeType> _types = new();

    public IReadOnlyDictionary<string, HypnodeType> Types => _types;

    public HypnodeTypeRegistry Register(string name, HypnodeType type)
    {
        _types[name] = type;
        return this;
    }

    public HypnodeType Resolve(string name)
    {
        if (_types.TryGetValue(name, out var type)) return type;
        throw new InvalidOperationException($"Unknown type '{name}'");
    }

    public bool TryResolve(string name, out HypnodeType type) =>
        _types.TryGetValue(name, out type!);
}
