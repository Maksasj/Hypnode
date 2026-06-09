using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Types;
using Hypnode.System.Common;
using Hypnode.System.Math;

namespace Hypnode.UnitTests.System.Math;

[TestFixture]
public class SquarerTests
{
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(-2)]
    [TestCase(2)]
    [TestCase(100)]
    [TestCase(4)]
    [TestCase(3)]
    [TestCase(-25)]
    public void TestSquarer_CorrectValue(int value)
    {
        var graph = new CoroutineNodeGraph();
        var pulse   = graph.AddNode(new PulseValue(new IntValue(value)));
        var squarer = graph.AddNode(new Squarer());
        var result  = graph.AddNode(new Register());

        graph.AddConnection(pulse, Ports.Output, squarer, Ports.Input);
        graph.AddConnection(squarer, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(value * value));
    }
}
