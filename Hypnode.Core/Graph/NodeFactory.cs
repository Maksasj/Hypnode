using System.Reflection;
using Hypnode.Core.Modules;

namespace Hypnode.Core.Graph;

public class NodeFactory
{
    private readonly Dictionary<string, Func<IReadOnlyDictionary<string, string>, INode>> _nodeCreators = new();
    private readonly Dictionary<string, string> _nodeDescriptions = new();

    private readonly Dictionary<string, Func<INodeGraph, IConnection>> _connectionCreators = new()
    {
        ["int"]    = g => g.CreateConnection<int>(),
        ["string"] = g => g.CreateConnection<string>(),
        ["bool"]   = g => g.CreateConnection<bool>(),
        ["byte"]   = g => g.CreateConnection<byte>(),
        ["float"]  = g => g.CreateConnection<float>(),
        ["double"] = g => g.CreateConnection<double>(),
    };

    public IReadOnlyCollection<string> NodeTypes => _nodeCreators.Keys;
    public IReadOnlyCollection<string> ConnectionTypes => _connectionCreators.Keys;
    public IReadOnlyDictionary<string, string> NodeDescriptions => _nodeDescriptions;

    public NodeFactory Register(string typeName, string description, Func<IReadOnlyDictionary<string, string>, INode> creator)
    {
        _nodeCreators[typeName] = creator;
        _nodeDescriptions[typeName] = description;
        return this;
    }

    public NodeFactory Register(string typeName, string description, Func<INode> creator)
        => Register(typeName, description, _ => creator());

    public NodeFactory Register(string typeName, Func<IReadOnlyDictionary<string, string>, INode> creator)
        => Register(typeName, "", creator);

    public NodeFactory Register(string typeName, Func<INode> creator)
        => Register(typeName, "", _ => creator());

    public NodeFactory Register<T>() where T : INode, new()
    {
        var attr = typeof(T).GetCustomAttribute<HypnodeNodeAttribute>()
            ?? throw new InvalidOperationException($"{typeof(T).Name} is missing a [HypnodeNode] attribute");
        return Register(attr.Name, attr.Description, _ => new T());
    }

    public NodeFactory RegisterConnectionType(string alias, Func<INodeGraph, IConnection> creator)
    {
        _connectionCreators[alias] = creator;
        return this;
    }

    public CoroutineNodeGraph Build(GraphDefinition definition)
    {
        var graph = new CoroutineNodeGraph();
        var nodeById = new Dictionary<string, INode>();

        foreach (var nd in definition.Nodes)
        {
            if (!_nodeCreators.TryGetValue(nd.TypeName, out var creator))
                throw new InvalidOperationException($"No creator registered for node type '{nd.TypeName}'");

            var node = creator(nd.Parameters);
            graph.AddNode(node);
            nodeById[nd.Id] = node;
        }

        foreach (var cd in definition.Connections)
        {
            if (!_connectionCreators.TryGetValue(cd.TypeAlias, out var connCreator))
                throw new InvalidOperationException($"Unknown connection type alias '{cd.TypeAlias}'. Register it with RegisterConnectionType.");

            var connection = connCreator(graph);
            nodeById[cd.FromNodeId].SetPort(cd.FromPort, connection);
            nodeById[cd.ToNodeId].SetPort(cd.ToPort, connection);
        }

        return graph;
    }
}
