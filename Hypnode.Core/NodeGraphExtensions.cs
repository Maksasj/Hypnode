namespace Hypnode.Core;

public static class NodeGraphExtensions
{
    public static INodeGraph AddConnection<T>(this INodeGraph graph,
        INode nodeA, string portNameA, INode nodeB, string portNameB)
    {
        var connection = graph.CreateConnection<T>();
        nodeA.SetPort(portNameA, connection);
        nodeB.SetPort(portNameB, connection);
        return graph;
    }
}
