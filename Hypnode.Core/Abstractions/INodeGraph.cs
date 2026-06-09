using Hypnode.Core.Types;

namespace Hypnode.Core;

public interface INodeGraph
{
    public Connection<HypnodeValue> CreateConnection();
    public Connection<HypnodeValue> CreateBoundedConnection(int capacity);

    public T AddNode<T>(T node) where T : INode;

    public void Evaluate();
}
