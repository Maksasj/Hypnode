namespace Hypnode.Core;

public interface INodeGraph
{
    public Connection CreateConnection();
    public Connection CreateBoundedConnection(int capacity);

    public T AddNode<T>(T node) where T : INode;

    public void Evaluate();
}
