using Hypnode.Core.Graph;
using Hypnode.Core.Modules;
using Hypnode.System.Common;
using Hypnode.System.IO;
using Hypnode.System.Math;

namespace Hypnode.System;

[HypnodeModule("Hypnode.System", "Built-in system nodes: sources, sinks, routing, and transforms", "1.0.0")]
public sealed class SystemModule : IHypnodeModule
{
    public void Register(NodeFactory factory) => factory
        // sources
        .Register("pulse-int",       "Emits a single int packet on the first tick",    p => new PulseValue<int>(int.Parse(p["value"])))
        .Register("pulse-bool",      "Emits a single bool packet on the first tick",   p => new PulseValue<bool>(bool.Parse(p["value"])))
        .Register("pulse-byte",      "Emits a single byte packet on the first tick",   p => new PulseValue<byte>(byte.Parse(p["value"])))
        .Register("pulse-string",    "Emits a single string packet on the first tick", p => new PulseValue<string>(p["value"]))
        .Register("multi-int",       "Emits multiple int packets on the first tick",   p => new MultiPulseValue<int>(p["values"].Split(',').Select(int.Parse)))
        .Register<Generator>()
        // sinks / stores
        .Register("register-int",    "Stores the last received int packet",    () => new Register<int>())
        .Register("register-bool",   "Stores the last received bool packet",   () => new Register<bool>())
        .Register("register-string", "Stores the last received string packet", () => new Register<string>())
        .Register("printer-int",     "Prints each received int to stdout",     () => new Printer<int>())
        .Register("printer-bool",    "Prints each received bool to stdout",    () => new Printer<bool>())
        .Register("printer-string",  "Prints each received string to stdout",  () => new Printer<string>())
        .Register("void-int",        "Discards all received int packets",      () => new VoidSink<int>())
        // routing
        .Register("splitter-int",    "Fans out one int input to multiple outputs",  () => new Splitter<int>())
        .Register("if-even",         "Routes even ints to THEN, odd ints to ELSE",  () => new IfNode<int>(x => x % 2 == 0))
        .Register("if-positive",     "Routes positive ints to THEN, others to ELSE", () => new IfNode<int>(x => x > 0))
        // feedback / cycles
        .Register("delay-int",       "Emits seed on first tick, then echoes each input next tick", p => new DelayNode<int>(int.Parse(p["value"])))
        // transform
        .Register<AddIntNode>()
        .Register<Squarer>()
        .Register("fold-sum",        "Running sum of all int packets",         () => new FoldNode<int, int>(0, (acc, x) => acc + x))
        .Register("fold-product",    "Running product of all int packets",     () => new FoldNode<int, int>(1, (acc, x) => acc * x))
        .Register("fold-count",      "Counts all received int packets",        () => new FoldNode<int, int>(0, (acc, _) => acc + 1))
        .Register("filter-even",     "Passes only even int packets",           () => new FilterNode<int>(x => x % 2 == 0))
        .Register("filter-odd",      "Passes only odd int packets",            () => new FilterNode<int>(x => x % 2 != 0))
        .Register("filter-positive", "Passes only positive int packets",       () => new FilterNode<int>(x => x > 0));
}
