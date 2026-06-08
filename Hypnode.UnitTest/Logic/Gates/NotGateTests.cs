using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Gates;

[TestFixture]
public class NotGateTests
{
    [TestCase(LogicValue.False, LogicValue.True)]
    [TestCase(LogicValue.True,  LogicValue.False)]
    public void TestNot_CorrectValue(LogicValue value, LogicValue expected)
    {
        var graph  = new CoroutineNodeGraph();
        var connIn  = graph.CreateConnection<LogicValue>();
        var connOut = graph.CreateConnection<LogicValue>();

        graph.AddNode(new PulseValue<LogicValue>(value)).SetPort(Ports.Output, connIn);
        graph.AddNode(new NotGate()).SetPort(Ports.Input, connIn).SetPort(Ports.Output, connOut);

        var result = new Register<LogicValue>();
        graph.AddNode(result).SetPort(Ports.Input, connOut);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(expected));
    }
}
