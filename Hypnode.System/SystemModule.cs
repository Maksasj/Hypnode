using Hypnode.Core.Graph;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using Hypnode.System.Common;
using Hypnode.System.IO;
using Hypnode.System.Math;

namespace Hypnode.System;

[HypnodeModule("Hypnode.System", "Built-in system nodes: sources, sinks, routing, and transforms", "1.0.0")]
public sealed class SystemModule : IHypnodeModule
{
    public void Register(NodeFactory factory, HypnodeTypeRegistry types)
    {
        types
            .Register("int",    new HypnodeType.Primitive("int"))
            .Register("float",  new HypnodeType.Primitive("float"))
            .Register("double", new HypnodeType.Primitive("double"))
            .Register("bool",   new HypnodeType.Primitive("bool"))
            .Register("byte",   new HypnodeType.Primitive("byte"))
            .Register("string", new HypnodeType.Primitive("string"));

        factory
            // sources
            .Register("pulse", "Emits a single packet on the first tick", p =>
                new PulseValue(HypnodeValueParser.Parse(types, p["type"], p["value"])))
            .Register("multi", "Emits multiple packets on the first tick", p =>
            {
                var t = p["type"];
                return new MultiPulseValue(p["values"].Split(',').Select(v => HypnodeValueParser.Parse(types, t, v.Trim())));
            })
            .Register<Generator>()
            // sinks / stores
            .Register("register", "Stores the last received packet",          () => new Register())
            .Register<Printer>()
            .Register("void", "Discards all received packets",                () => new VoidSink())
            // routing
            .Register("splitter", "Fans out one input to multiple outputs",   () => new Splitter())
            .Register("if-even",     "Routes even ints to THEN, odds to ELSE",   () => new IfNode(v => v.AsInt() % 2 == 0))
            .Register("if-positive", "Routes positive ints to THEN, rest to ELSE", () => new IfNode(v => v.AsInt() > 0))
            // feedback / cycles
            .Register("delay", "Emits seed on first tick, then echoes each input next tick", p =>
                new DelayNode(HypnodeValueParser.Parse(types, p["type"], p["value"])))
            // transforms
            .Register<AddIntNode>()
            .Register<Squarer>()
            .Register("fold-sum",     "Running sum of int packets",     () => new FoldNode(new IntValue(0), (acc, x) => new IntValue(acc.AsInt() + x.AsInt())))
            .Register("fold-product", "Running product of int packets", () => new FoldNode(new IntValue(1), (acc, x) => new IntValue(acc.AsInt() * x.AsInt())))
            .Register("fold-count",   "Counts received packets",        () => new FoldNode(new IntValue(0), (acc, _) => new IntValue(acc.AsInt() + 1)))
            .Register("filter-even",     "Passes only even int packets",     () => new FilterNode(v => v.AsInt() % 2 == 0))
            .Register("filter-odd",      "Passes only odd int packets",      () => new FilterNode(v => v.AsInt() % 2 != 0))
            .Register("filter-positive", "Passes only positive int packets", () => new FilterNode(v => v.AsInt() > 0));
    }
}
