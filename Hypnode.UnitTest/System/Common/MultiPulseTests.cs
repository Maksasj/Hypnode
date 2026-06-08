using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

public abstract class MultiPulseTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(LogicValue.False)]
    [TestCase(LogicValue.True)]
    public void TestMultiPulse_SingleElement_CorrectValue(LogicValue value)
    {
        var graph      = new TGraph();
        var multiPulse = graph.AddNode(new MultiPulseValue<LogicValue>([value]));
        var result     = graph.AddNode(new Register<LogicValue>());
        graph.AddConnection<LogicValue>(multiPulse, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(value));
    }

    [TestCase(0)]
    [TestCase(100)]
    [TestCase(20000)]
    [TestCase(-100)]
    public void TestMultiPulse_SingleElement_SendCloseExecuteOnce(int value)
    {
        var graph      = new TGraph();
        var connection = new Mock<Connection<int>>();
        var multiPulse = graph.AddNode(new MultiPulseValue<int>([value]));
        multiPulse.SetPort(Ports.Output, connection.Object);

        graph.Evaluate();

        connection.Verify(c => c.Send(value), Times.Once);
        connection.Verify(c => c.Close(),     Times.Once);
    }
}

[TestFixture] public class CoroutineNodeGraph_MultiPulseTests : MultiPulseTests<CoroutineNodeGraph> { }
