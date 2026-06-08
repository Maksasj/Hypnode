namespace Hypnode.Core;

public interface INodeGraph
{
    public Connection<T> CreateConnection<T>();

    public T AddNode<T>(T node) where T : INode;

    public void Evaluate();
}
