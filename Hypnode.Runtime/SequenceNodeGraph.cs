using Hypnode.Core;

namespace Hypnode.Runtime;

public class SequenceNodeGraph : INodeGraph
{
    public List<INode> Nodes { get; set; }
    public List<IConnection> Connections { get; set; }

    public SequenceNodeGraph()
    {
        Nodes = [];
        Connections = [];
    }

    public Connection<T> CreateConnection<T>()
    {
        var connection = new SequenceConnection<T>();
        Connections.Add(connection);
        return connection;
    }

    public T AddNode<T>(T node) where T : INode
    {
        Nodes.Add(node);
        return node;
    }

    public void Evaluate()
    {
        foreach (var node in Nodes)
        {
            var coroutine = node.Execute();
            while (coroutine.MoveNext())
            {
                if (coroutine.Current is INodeGraph subGraph)
                    subGraph.Evaluate();
            }
        }
    }
}
