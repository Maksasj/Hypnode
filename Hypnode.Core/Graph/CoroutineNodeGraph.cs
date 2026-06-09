using Hypnode.Core.Connections;
using System.Collections;

namespace Hypnode.Core.Graph;

public class CoroutineNodeGraph : INodeGraph
{
    public List<INode> Nodes { get; set; } = [];
    public List<IConnection> Connections { get; set; } = [];

    public Connection CreateConnection()
    {
        var connection = new QueueConnection();
        Connections.Add(connection);
        return connection;
    }

    public Connection CreateBoundedConnection(int capacity)
    {
        var connection = new BoundedQueueConnection(capacity);
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

            foreach (var c in Connections) c.ResetActivity();

            foreach (var coroutine in coroutines)
            {
                if (coroutine.MoveNext())
                {
                    if (coroutine.Current is INodeGraph subGraph)
                        subGraph.Evaluate();
                    next.Add(coroutine);
                }
            }

            if (next.Count == countBefore && !Connections.Any(c => c.HasData || c.HadActivity))
                throw new InvalidOperationException("Deadlock: all nodes waiting, no data in flight");

            coroutines = next;
        }
    }
}
