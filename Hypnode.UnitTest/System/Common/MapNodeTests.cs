using Hypnode.Core;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class MapNodeTests
{
    [TestCase(0,    0)]
    [TestCase(1,    2)]
    [TestCase(-5,  -10)]
    [TestCase(100, 200)]
    public void TestMap_CorrectValue(int input, int expected)
    {
        var graph  = new CoroutineNodeGraph();
        var pulse  = graph.AddNode(new PulseValue<int>(input));
        var map    = graph.AddNode(new MapNode<int, int>(x => x * 2));
        var result = graph.AddNode(new Register<int>());

        graph.AddConnection<int>(pulse,  Ports.Output, map,    Ports.Input);
        graph.AddConnection<int>(map,    Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(expected));
    }

    [TestCase(0)]
    [TestCase(42)]
    [TestCase(-7)]
    public void TestMap_TypeConversion(int input)
    {
        var graph   = new CoroutineNodeGraph();
        var connIn  = graph.CreateConnection<int>();
        var connOut = graph.CreateConnection<string>();

        graph.AddNode(new PulseValue<int>(input)).SetPort(Ports.Output, connIn);
        graph.AddNode(new MapNode<int, string>(x => x.ToString())).SetPort(Ports.Input, connIn).SetPort(Ports.Output, connOut);

        var result = graph.AddNode(new Register<string>());
        result.SetPort(Ports.Input, connOut);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(input.ToString()));
    }

    [Test]
    public void TestMap_ClosesOutput()
    {
        var graph   = new CoroutineNodeGraph();
        var connIn  = graph.CreateConnection<int>();
        var connOut = new Mock<Connection<int>>();

        graph.AddNode(new PulseValue<int>(1)).SetPort(Ports.Output, connIn);
        graph.AddNode(new MapNode<int, int>(x => x)).SetPort(Ports.Input, connIn).SetPort(Ports.Output, connOut.Object);

        graph.Evaluate();

        connOut.Verify(c => c.Close(), Times.Once);
    }

    [Test]
    public void TestMap_MultiplePackets()
    {
        var graph  = new CoroutineNodeGraph();
        var multi  = graph.AddNode(new MultiPulseValue<int>([1, 2, 3, 4, 5]));
        var map    = graph.AddNode(new MapNode<int, int>(x => x * x));
        var result = graph.AddNode(new Register<int>());

        graph.AddConnection<int>(multi,  Ports.Output, map,    Ports.Input);
        graph.AddConnection<int>(map,    Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(25));
    }
}
