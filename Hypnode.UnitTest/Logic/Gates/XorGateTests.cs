using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Hypnode.System.Math;

namespace Hypnode.UnitTests.Logic.Gates;

public abstract class XorGateTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(LogicValue.False, LogicValue.True, LogicValue.True)]
    [TestCase(LogicValue.True, LogicValue.False, LogicValue.True)]
    [TestCase(LogicValue.True, LogicValue.True, LogicValue.False)]
    public void TestXor_CorrectValue(LogicValue a, LogicValue b, LogicValue expect)
    {
        var graph = new TGraph();
        var connection1 = graph.CreateConnection<LogicValue>();
        var connection2 = graph.CreateConnection<LogicValue>();
        var connection3 = graph.CreateConnection<LogicValue>();

        graph.AddNode(new PulseValue<LogicValue>(a))
            .SetPort(Ports.Output, connection1);

        graph.AddNode(new PulseValue<LogicValue>(b))
            .SetPort(Ports.Output, connection2);

        graph.AddNode(new XorGate())
            .SetPort(AndGate.InputA, connection1)
            .SetPort(AndGate.InputB, connection2)
            .SetPort(Ports.Output, connection3);

        var result = new Register<LogicValue>();
        graph.AddNode(result).SetPort(Ports.Input, connection3);

        graph.Evaluate();

        Assert.That(expect, Is.EqualTo(result.GetValue()));
    }
}

[TestFixture]
public class AsyncNodeGrap_XorGateTests : XorGateTests<CoroutineNodeGraph>
{

}

[TestFixture]
public class SequenceNodeGraph_XorGateTests : XorGateTests<SequenceNodeGraph>
{

}
