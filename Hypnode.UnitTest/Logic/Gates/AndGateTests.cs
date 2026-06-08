using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Gates;

public abstract class AndGateTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(LogicValue.False, LogicValue.True, LogicValue.False)]
    [TestCase(LogicValue.True, LogicValue.False, LogicValue.False)]
    [TestCase(LogicValue.True, LogicValue.True, LogicValue.True)]
    public void TestAnd_CorrectValue(LogicValue a, LogicValue b, LogicValue expect)
    {
        var graph = new TGraph();
        var connection1 = graph.CreateConnection<LogicValue>();
        var connection2 = graph.CreateConnection<LogicValue>();
        var connection3 = graph.CreateConnection<LogicValue>();

        graph.AddNode(new PulseValue<LogicValue>(a))
            .SetPort(Ports.Output, connection1);

        graph.AddNode(new PulseValue<LogicValue>(b))
            .SetPort(Ports.Output, connection2);

        graph.AddNode(new AndGate())
            .SetPort(AndGate.InputA, connection1)
            .SetPort(AndGate.InputB, connection2)
            .SetPort(Ports.Output, connection3);

        var result = new Register<LogicValue>();
        graph.AddNode(result).SetPort(Ports.Input, connection3);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(expect));
    }
}

[TestFixture]
public class AsyncNodeGrap_AndGateTests : AndGateTests<CoroutineNodeGraph>
{

}

[TestFixture]
public class SequenceNodeGraph_AndGateTests : AndGateTests<SequenceNodeGraph>
{

}
