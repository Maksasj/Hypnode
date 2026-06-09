using Hypnode.Core.Graph;
using Hypnode.Core.Types;

namespace Hypnode.Runtime;

public class HypnodeRuntime
{
    private readonly NodeFactory _factory = new();
    public HypnodeTypeRegistry Types { get; }

    public HypnodeRuntime()
    {
        var result = ModuleLoader.LoadAll([], _factory);
        Types = result.TypeRegistry;
    }

    public HypnodeRuntime(IEnumerable<string> dllPaths)
    {
        var result = ModuleLoader.LoadAll(dllPaths, _factory);
        Types = result.TypeRegistry;
    }

    public void Run(string xml)
    {
        var definition = GraphSerializer.Deserialize(xml);
        var graph = _factory.Build(definition);
        graph.Evaluate();
    }

    public CoroutineNodeGraph Build(string xml)
    {
        var definition = GraphSerializer.Deserialize(xml);
        return _factory.Build(definition);
    }
}
