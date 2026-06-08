using Hypnode.Core;
using Hypnode.System.Common;
using Hypnode.System.IO;
using Hypnode.System.Math;

namespace Hypnode.Runtime;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hypnode Runtime");
        Console.WriteLine("---------------");

        var factory = BuildFactory();

        var definition = BuildExampleDefinition();

        Console.WriteLine("Graph XML:");
        var xml = GraphSerializer.Serialize(definition);
        Console.WriteLine(xml);
        Console.WriteLine();

        Console.WriteLine("Evaluating (loaded from XML):");
        var loaded = GraphSerializer.Deserialize(xml);
        var graph  = factory.Build(loaded);
        graph.Evaluate();
    }

    static NodeFactory BuildFactory() =>
        new NodeFactory()
            .Register("pulse-int",       p  => new PulseValue<int>(int.Parse(p["value"])))
            .Register("multi-int",       p  => new MultiPulseValue<int>(p["values"].Split(',').Select(int.Parse)))
            .Register("filter-even",     () => new FilterNode<int>(x => x % 2 == 0))
            .Register("squarer",         () => new Squarer())
            .Register("printer-int",     () => new Printer<int>())
            .Register("register-int",    () => new Register<int>())
            .Register("fold-sum",        () => new FoldNode<int, int>(0, (acc, x) => acc + x))
            .Register("generator",       () => new Generator());

    static GraphDefinition BuildExampleDefinition()
    {
        var def = new GraphDefinition();
        def.AddNode("source", "multi-int",   ("values", "1,2,3,4,5,6,7,8"));
        def.AddNode("filter", "filter-even");
        def.AddNode("sq",     "squarer");
        def.AddNode("print",  "printer-int");
        def.Connect("int", "source", Ports.Output, "filter", Ports.Input);
        def.Connect("int", "filter", Ports.Output, "sq",     Ports.Input);
        def.Connect("int", "sq",     Ports.Output, "print",  Ports.Input);
        return def;
    }
}
