using Hypnode.Core.Graph;

namespace Hypnode.Core.Modules;

public interface IHypnodeModule
{
    void Register(NodeFactory factory);
}
