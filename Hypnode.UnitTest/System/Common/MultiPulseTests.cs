using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Types;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class MultiPulseTests
{
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(-7)]
    public void TestMultiPulse_SingleElement_CorrectValue(int value)
    {
        var graph = new CoroutineNodeGraph();
        var multiPulse = graph.AddNode(new MultiPulseValue([new IntValue(value)]));
        var result = graph.AddNode(new Register());
        graph.AddConnection(multiPulse, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(value));
    }

    [TestCase(0)]
    [TestCase(100)]
    [TestCase(20000)]
    [TestCase(-100)]
    public void TestMultiPulse_SingleElement_SendCloseExecuteOnce(int value)
    {
        var graph = new CoroutineNodeGraph();
        var conn = new Mock<Connection<HypnodeValue>>();
        graph.AddNode(new MultiPulseValue([new IntValue(value)])).SetPort(Ports.Output, conn.Object);

        graph.Evaluate();

        conn.Verify(c => c.Send(new IntValue(value)), Times.Once);
        conn.Verify(c => c.Close(), Times.Once);
    }
}
