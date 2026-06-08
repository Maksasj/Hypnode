using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Gates;

public abstract class NotGateTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(LogicValue.False, LogicValue.True)]
    [TestCase(LogicValue.True, LogicValue.False)]
    public void TestNot_CorrectValue(LogicValue value, LogicValue expect)
    {
        var graph = new TGraph();
        var connection1 = graph.CreateConnection<LogicValue>();
        var connection2 = graph.CreateConnection<LogicValue>();

        graph.AddNode(new PulseValue<LogicValue>(value))
            .SetPort(Ports.Output, connection1);

        graph.AddNode(new NotGate())
            .SetPort(Ports.Input, connection1)
            .SetPort(Ports.Output, connection2);

        var result = new Register<LogicValue>();
        graph.AddNode(result).SetPort(Ports.Input, connection2);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(expect));
    }
}

[TestFixture]
public class AsyncNodeGrap_NotGateTests : NotGateTests<CoroutineNodeGraph>
{

}

[TestFixture]
public class SequenceNodeGraph_NotGateTests : NotGateTests<SequenceNodeGraph>
{

}
