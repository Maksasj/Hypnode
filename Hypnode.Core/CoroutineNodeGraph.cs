using System.Collections;

namespace Hypnode.Core;

public class CoroutineNodeGraph : INodeGraph
{
    public List<INode> Nodes { get; set; } = [];
    public List<IConnection> Connections { get; set; } = [];

    public Connection<T> CreateConnection<T>()
    {
        var connection = new QueueConnection<T>();
        Connections.Add(connection);
        return connection;
    }

    public Connection<T> CreateBoundedConnection<T>(int capacity)
    {
        var connection = new BoundedQueueConnection<T>(capacity);
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
        var coroutines = Nodes.Select(n => n.Execute()).ToList();

        while (coroutines.Count > 0)
        {
            int countBefore = coroutines.Count;
            var next = new List<IEnumerator>();

            foreach (var coroutine in coroutines)
            {
                if (coroutine.MoveNext())
                {
                    if (coroutine.Current is INodeGraph subGraph)
                        subGraph.Evaluate();
                    next.Add(coroutine);
                }
            }

            if (next.Count == countBefore && !Connections.Any(c => c.HasData))
                throw new InvalidOperationException("Deadlock: all nodes waiting, no data in flight");

            coroutines = next;
        }
    }
}
