using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Gates;

public abstract class AndGateTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(LogicValue.False, LogicValue.True,  LogicValue.False)]
    [TestCase(LogicValue.True,  LogicValue.False, LogicValue.False)]
    [TestCase(LogicValue.True,  LogicValue.True,  LogicValue.True)]
    public void TestAnd_CorrectValue(LogicValue a, LogicValue b, LogicValue expected)
    {
        var graph = new TGraph();
        var connA = graph.CreateConnection<LogicValue>();
        var connB = graph.CreateConnection<LogicValue>();
        var connOut = graph.CreateConnection<LogicValue>();

        graph.AddNode(new PulseValue<LogicValue>(a)).SetPort(Ports.Output, connA);
        graph.AddNode(new PulseValue<LogicValue>(b)).SetPort(Ports.Output, connB);
        graph.AddNode(new AndGate()).SetPort(AndGate.InputA, connA).SetPort(AndGate.InputB, connB).SetPort(AndGate.Output, connOut);

        var result = new Register<LogicValue>();
        graph.AddNode(result).SetPort(Ports.Input, connOut);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(expected));
    }
}

[TestFixture] public class CoroutineNodeGraph_AndGateTests : AndGateTests<CoroutineNodeGraph> { }
