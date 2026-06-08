namespace Hypnode.Core;

public sealed class NodeDefinition
{
    public string Id { get; init; } = "";
    public string TypeName { get; init; } = "";
    public IReadOnlyDictionary<string, string> Parameters { get; init; } = new Dictionary<string, string>();
}

public sealed class ConnectionDefinition
{
    public string TypeAlias { get; init; } = "";
    public string FromNodeId { get; init; } = "";
    public string FromPort { get; init; } = "";
    public string ToNodeId { get; init; } = "";
    public string ToPort { get; init; } = "";
}

public class GraphDefinition
{
    public List<NodeDefinition> Nodes { get; } = [];
    public List<ConnectionDefinition> Connections { get; } = [];

    public GraphDefinition AddNode(string id, string typeName, params (string Key, string Value)[] parameters)
    {
        Nodes.Add(new NodeDefinition
        {
            Id = id,
            TypeName = typeName,
            Parameters = parameters.ToDictionary(p => p.Key, p => p.Value),
        });
        return this;
    }

    public GraphDefinition Connect(string typeAlias, string fromNodeId, string fromPort, string toNodeId, string toPort)
    {
        Connections.Add(new ConnectionDefinition
        {
            TypeAlias = typeAlias,
            FromNodeId = fromNodeId,
            FromPort = fromPort,
            ToNodeId = toNodeId,
            ToPort = toPort,
        });
        return this;
    }
}
