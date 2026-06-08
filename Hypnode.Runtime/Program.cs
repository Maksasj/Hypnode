using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Compound;
using Hypnode.Logic.Gates;
using Hypnode.Logic.Utils;
using Hypnode.System.Common;
using Hypnode.System.IO;
using Hypnode.System.Math;

namespace Hypnode.Runtime;

internal static class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            PrintHelp();
            return 0;
        }

        var filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"error: file not found: {filePath}");
            return 1;
        }

        string xml;
        try { xml = File.ReadAllText(filePath); }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"error: could not read file: {ex.Message}");
            return 1;
        }

        GraphDefinition definition;
        try { definition = GraphSerializer.Deserialize(xml); }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"error: invalid graph XML: {ex.Message}");
            return 1;
        }

        CoroutineNodeGraph graph;
        try { graph = BuildFactory().Build(definition); }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"error: could not build graph: {ex.Message}");
            return 1;
        }

        try { graph.Evaluate(); }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"error: graph evaluation failed: {ex.Message}");
            return 1;
        }

        return 0;
    }

    static void PrintHelp()
    {
        Console.WriteLine("""
            Hypnode Runtime
            ===============
            Usage:
              hypnode <graph.xml>    Load and run a graph from an XML file
              hypnode --help         Show this help

            XML format:
              <Graph>
                <Nodes>
                  <Node id="n0" type="pulse-int">
                    <Parameters><Parameter name="value">42</Parameter></Parameters>
                  </Node>
                  <Node id="n1" type="printer-int" />
                </Nodes>
                <Connections>
                  <Connection type="int">
                    <From node="n0" port="OUT" />
                    <To   node="n1" port="IN"  />
                  </Connection>
                </Connections>
              </Graph>

            Connection type aliases:
              int, string, bool, byte, float, double, LogicValue

            Built-in node types:
              -- sources / sinks --
              pulse-int         Emit a single int packet        param: value
              pulse-bool        Emit a single bool packet       param: value (true/false)
              pulse-byte        Emit a single byte packet       param: value
              pulse-string      Emit a single string packet     param: value
              pulse-logic       Emit a single LogicValue        param: value (True/False)
              multi-int         Emit multiple int packets       param: values (comma-separated)
              generator         Emit incrementing ints forever
              register-int      Store last received int
              register-bool     Store last received bool
              register-string   Store last received string
              register-logic    Store last received LogicValue
              printer-int       Print each int to stdout
              printer-bool      Print each bool to stdout
              printer-string    Print each string to stdout
              printer-logic     Print each LogicValue to stdout
              void-int          Discard int packets
              void-logic        Discard LogicValue packets

              -- routing --
              splitter-int      Fan-out: one int input, multiple outputs
              splitter-logic    Fan-out: one LogicValue input, multiple outputs
              if-even           Route ints: even -> THEN, odd  -> ELSE
              if-positive       Route ints: > 0  -> THEN, else -> ELSE

              -- transform --
              squarer           Square each int packet
              fold-sum          Running sum of ints
              fold-product      Running product of ints
              fold-count        Count packets (int input)
              filter-even       Pass only even ints
              filter-odd        Pass only odd ints
              filter-positive   Pass only positive ints

              -- logic gates --
              and-gate          Ports: INA, INB, OUT  (LogicValue)
              or-gate           Ports: INA, INB, OUT  (LogicValue)
              xor-gate          Ports: INA, INB, OUT  (LogicValue)
              not-gate          Ports: IN,  OUT       (LogicValue)

              -- compound logic --
              full-adder        Ports: INA, INB, INC, OUTSUM, OUTC
              full-adder-byte   Ports: INA, INB, OUTSUM  (byte)
              byte-splitter-in  Ports: IN, 0-7  (byte in, LogicValue out)
              byte-splitter-out Ports: 0-7, OUT (LogicValue in, byte out)
            """);
    }

    static NodeFactory BuildFactory() => new NodeFactory()
        .RegisterConnectionType("LogicValue", g => g.CreateConnection<LogicValue>())

        // sources
        .Register("pulse-int", p => new PulseValue<int>(int.Parse(p["value"])))
        .Register("pulse-bool", p => new PulseValue<bool>(bool.Parse(p["value"])))
        .Register("pulse-byte", p => new PulseValue<byte>(byte.Parse(p["value"])))
        .Register("pulse-string", p => new PulseValue<string>(p["value"]))
        .Register("pulse-logic", p => new PulseValue<LogicValue>(Enum.Parse<LogicValue>(p["value"])))
        .Register("multi-int", p => new MultiPulseValue<int>(p["values"].Split(',').Select(int.Parse)))
        .Register("generator", () => new Generator())

        // sinks / stores
        .Register("register-int", () => new Register<int>())
        .Register("register-bool", () => new Register<bool>())
        .Register("register-string", () => new Register<string>())
        .Register("register-logic", () => new Register<LogicValue>())
        .Register("printer-int", () => new Printer<int>())
        .Register("printer-bool", () => new Printer<bool>())
        .Register("printer-string", () => new Printer<string>())
        .Register("printer-logic", () => new Printer<LogicValue>())
        .Register("void-int", () => new VoidSink<int>())
        .Register("void-logic", () => new VoidSink<LogicValue>())

        // routing
        .Register("splitter-int", () => new Splitter<int>())
        .Register("splitter-logic", () => new Splitter<LogicValue>())
        .Register("if-even", () => new IfNode<int>(x => x % 2 == 0))
        .Register("if-positive", () => new IfNode<int>(x => x > 0))

        // transform
        .Register("squarer", () => new Squarer())
        .Register("fold-sum", () => new FoldNode<int, int>(0, (acc, x) => acc + x))
        .Register("fold-product", () => new FoldNode<int, int>(1, (acc, x) => acc * x))
        .Register("fold-count", () => new FoldNode<int, int>(0, (acc, _) => acc + 1))
        .Register("filter-even", () => new FilterNode<int>(x => x % 2 == 0))
        .Register("filter-odd", () => new FilterNode<int>(x => x % 2 != 0))
        .Register("filter-positive", () => new FilterNode<int>(x => x > 0))

        // logic gates
        .Register("and-gate", () => new AndGate())
        .Register("or-gate", () => new OrGate())
        .Register("xor-gate", () => new XorGate())
        .Register("not-gate", () => new NotGate())

        // compound logic
        .Register("full-adder", () => new FullAdder())
        .Register("full-adder-byte", () => new FullAdderByte())
        .Register("byte-splitter-in", () => new ByteSplitterIn())
        .Register("byte-splitter-out", () => new ByteSplitterOut());
}
