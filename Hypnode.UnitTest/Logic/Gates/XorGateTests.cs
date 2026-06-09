using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Gates;

[TestFixture]
public class XorGateTests
{
    [TestCase(LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(LogicValue.False, LogicValue.True, LogicValue.True)]
    [TestCase(LogicValue.True, LogicValue.False, LogicValue.True)]
    [TestCase(LogicValue.True, LogicValue.True, LogicValue.False)]
    public void TestXor_CorrectValue(LogicValue a, LogicValue b, LogicValue expected)
    {
        var graph = new CoroutineNodeGraph();
        var connA = graph.CreateConnection<LogicValue>();
        var connB = graph.CreateConnection<LogicValue>();
        var connOut = graph.CreateConnection<LogicValue>();

        graph.AddNode(new PulseValue<LogicValue>(a)).SetPort(Ports.Output, connA);
        graph.AddNode(new PulseValue<LogicValue>(b)).SetPort(Ports.Output, connB);
        graph.AddNode(new XorGate())
            .SetPort(XorGate.InputA, connA)
            .SetPort(XorGate.InputB, connB)
            .SetPort(XorGate.Output, connOut);

        var result = new Register<LogicValue>();
        graph.AddNode(result).SetPort(Ports.Input, connOut);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(expected));
    }
}
