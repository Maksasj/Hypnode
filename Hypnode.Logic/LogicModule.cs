using Hypnode.Core.Graph;
using Hypnode.Core.Modules;
using Hypnode.Logic.Compound;
using Hypnode.Logic.Gates;
using Hypnode.Logic.Utils;
using Hypnode.System.Common;
using Hypnode.System.IO;

namespace Hypnode.Logic;

[HypnodeModule("Hypnode.Logic", "Logic gates, compound circuits, and LogicValue node types", "1.0.0")]
public sealed class LogicModule : IHypnodeModule
{
    public void Register(NodeFactory factory) => factory
        // connection type
        .RegisterConnectionType("LogicValue", g => g.CreateConnection<LogicValue>())
        // LogicValue sources / sinks
        .Register("pulse-logic",    "Emits a single LogicValue packet on the first tick", p => new PulseValue<LogicValue>(Enum.Parse<LogicValue>(p["value"])))
        .Register("register-logic", "Stores the last received LogicValue packet",         () => new Register<LogicValue>())
        .Register("printer-logic",  "Prints each received LogicValue to stdout",          () => new Printer<LogicValue>())
        .Register("void-logic",     "Discards all received LogicValue packets",           () => new VoidSink<LogicValue>())
        .Register("splitter-logic", "Fans out one LogicValue input to multiple outputs",  () => new Splitter<LogicValue>())
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
