namespace Hypnode.Core;

public interface INodeGraph
{
    public Connection<T> CreateConnection<T>();
    public Connection<T> CreateBoundedConnection<T>(int capacity);

    public T AddNode<T>(T node) where T : INode;

    public void Evaluate();
}
