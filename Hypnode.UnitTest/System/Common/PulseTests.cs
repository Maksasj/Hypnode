using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Moq;
using Hypnode.System.Math;

namespace Hypnode.UnitTests.System.Common;

public abstract class PulseTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(LogicValue.False)]
    [TestCase(LogicValue.True)]
    public void TestPulse_CorrectValue(LogicValue value)
    {
        var graph = new TGraph();

        var pulse = graph.AddNode(new PulseValue<LogicValue>(value));
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
        var graph = new TGraph();

        var connection = new Mock<Connection<int>>();
        var pulse = graph.AddNode(new PulseValue<int>(value));

        pulse.SetPort(Ports.Output, connection.Object);

        graph.Evaluate();

        connection.Verify(c => c.Send(value), Times.Once);
        connection.Verify(c => c.Close(), Times.Once);
    }
}

[TestFixture]
public class CoroutineNodeGraph_PulseTests : PulseTests<CoroutineNodeGraph>
{

}

[TestFixture]
public class SequenceNodeGraph_PulseTests : PulseTests<SequenceNodeGraph>
{

}
