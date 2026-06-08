using System.Collections;

namespace Hypnode.Core;

public abstract class CompoundNode : INode
{
    protected readonly INodeGraph NodeGraph;

    protected CompoundNode(INodeGraph nodeGraph)
    {
        NodeGraph = nodeGraph;
    }

    public abstract INode SetPort(string portName, IConnection connection);
    public abstract IEnumerator Execute();
}
