using Hypnode.Core;

namespace Hypnode.Async
{
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

        public async Task EvaluateAsync()
        {
            // Execute nodes sequentially (one at a time) in the order they were added
            foreach (var node in Nodes)
            {
                await node.ExecuteAsync();
            }
        }
    }
}
