using Hypnode.Core;
using Hypnode.Logic.Gates;
using Hypnode.System.Common;
using Hypnode.System.IO;

namespace Hypnode.Runtime;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hypnode Runtime");
        Console.WriteLine("---------------");

        RunExample();
    }

    static void RunExample()
    {
        // Example: filter even numbers, square them, print results
        var graph  = new CoroutineNodeGraph();
        var source = graph.AddNode(new MultiPulseValue<int>([1, 2, 3, 4, 5, 6, 7, 8]));
        var filter = graph.AddNode(new FilterNode<int>(x => x % 2 == 0));
        var map    = graph.AddNode(new MapNode<int, int>(x => x * x));
        var print  = graph.AddNode(new Printer<int>());

        graph.AddConnection<int>(source, Ports.Output, filter, Ports.Input);
        graph.AddConnection<int>(filter, Ports.Output, map,    Ports.Input);
        graph.AddConnection<int>(map,    Ports.Output, print,  Ports.Input);

        graph.Evaluate();
    }
}
