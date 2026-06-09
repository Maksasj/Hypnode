using Hypnode.Core.Graph;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using Hypnode.Logic.Compound;
using Hypnode.Logic.Gates;
using Hypnode.Logic.Utils;

namespace Hypnode.Logic;

[HypnodeModule("Hypnode.Logic", "Logic gates, compound circuits, and LogicValue node types", "1.0.0")]
public sealed class LogicModule : IHypnodeModule
{
    public void Register(NodeFactory factory, HypnodeTypeRegistry types)
    {
        types
            .Register("LogicValue", new HypnodeType.Primitive("LogicValue"));

        factory
            .RegisterConnectionType("LogicValue", g => g.CreateConnection())
            // gates
            .Register<AndGate>()
            .Register<OrGate>()
            .Register<XorGate>()
            .Register<NotGate>()
            // compound
            .Register<FullAdder>()
            .Register<FullAdderByte>()
            .Register<ByteSplitterIn>()
            .Register<ByteSplitterOut>();
    }
}
