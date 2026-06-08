using System.Collections;

namespace Hypnode.Core;

public abstract class CompoundNode : INode
{
    public INodeGraph NodeGraph { init; protected get; }

    public CompoundNode(INodeGraph nodeGraph)
    {
        NodeGraph = nodeGraph;
    }

    public abstract INode SetPort(string portName, IConnection connection);

    public abstract IEnumerator Execute();
}
