namespace Hypnode.Core;

public static class NodeGraphExtensions
{
    public static INodeGraph AddConnection(this INodeGraph graph,
        INode nodeA, string portNameA, INode nodeB, string portNameB)
    {
        var connection = graph.CreateConnection();
        nodeA.SetPort(portNameA, connection);
        nodeB.SetPort(portNameB, connection);
        return graph;
    }

    public static INodeGraph AddBoundedConnection(this INodeGraph graph,
        INode nodeA, string portNameA, INode nodeB, string portNameB, int capacity)
    {
        var connection = graph.CreateBoundedConnection(capacity);
        nodeA.SetPort(portNameA, connection);
        nodeB.SetPort(portNameB, connection);
        return graph;
    }
}
