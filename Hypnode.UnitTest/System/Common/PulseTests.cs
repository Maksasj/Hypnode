using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class PulseTests
{
    [TestCase(LogicValue.False)]
    [TestCase(LogicValue.True)]
    public void TestPulse_CorrectValue(LogicValue value)
    {
        var graph  = new CoroutineNodeGraph();
        var pulse  = graph.AddNode(new PulseValue<LogicValue>(value));
        var result = graph.AddNode(new Register<LogicValue>());
        graph.AddConnection<LogicValue>(pulse, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(value));
    }

    [TestCase(0)]
    [TestCase(100)]
    [TestCase(20000)]
    [TestCase(-100)]
    public void TestPulse_SendCloseExecuteOnce(int value)
    {
        var graph = new CoroutineNodeGraph();
        var conn  = new Mock<Connection<int>>();
        graph.AddNode(new PulseValue<int>(value)).SetPort(Ports.Output, conn.Object);

        graph.Evaluate();

        conn.Verify(c => c.Send(value), Times.Once);
        conn.Verify(c => c.Close(),     Times.Once);
    }
}
