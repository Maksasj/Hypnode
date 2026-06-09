using Hypnode.Core.Graph;
using Hypnode.Core.Types;

namespace Hypnode.Core.Modules;

public interface IHypnodeModule
{
    void Register(NodeFactory factory, HypnodeTypeRegistry types);
}
