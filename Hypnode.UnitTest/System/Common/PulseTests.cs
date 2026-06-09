using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Types;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class PulseTests
{
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(20000)]
    [TestCase(-100)]
    public void TestPulse_CorrectValue(int value)
    {
        var graph = new CoroutineNodeGraph();
        var pulse = graph.AddNode(new PulseValue(new IntValue(value)));
        var result = graph.AddNode(new Register());
        graph.AddConnection(pulse, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(value));
    }

    [TestCase(0)]
    [TestCase(100)]
    [TestCase(20000)]
    [TestCase(-100)]
    public void TestPulse_SendCloseExecuteOnce(int value)
    {
        var graph = new CoroutineNodeGraph();
        var conn = new Mock<Connection>();
        graph.AddNode(new PulseValue(new IntValue(value))).SetPort(Ports.Output, conn.Object);

        graph.Evaluate();

        conn.Verify(c => c.Send(new IntValue(value)), Times.Once);
        conn.Verify(c => c.Close(), Times.Once);
    }
}
