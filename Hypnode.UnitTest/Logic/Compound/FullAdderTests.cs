using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Compound;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Compound;

[TestFixture]
public class FullAdderTests
{
    [TestCase(LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(LogicValue.False, LogicValue.False, LogicValue.True,  LogicValue.True,  LogicValue.False)]
    [TestCase(LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False)]
    [TestCase(LogicValue.False, LogicValue.True,  LogicValue.True,  LogicValue.False, LogicValue.True)]
    [TestCase(LogicValue.True,  LogicValue.False, LogicValue.False, LogicValue.True,  LogicValue.False)]
    [TestCase(LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True)]
    [TestCase(LogicValue.True,  LogicValue.True,  LogicValue.False, LogicValue.False, LogicValue.True)]
    [TestCase(LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True)]
    public void TestFullAdder_CorrectValues(LogicValue a, LogicValue b, LogicValue cIn,
        LogicValue expectedSum, LogicValue expectedCarry)
    {
        var graph  = new CoroutineNodeGraph();
        var ain    = graph.CreateConnection<LogicValue>();
        var bin    = graph.CreateConnection<LogicValue>();
        var cin    = graph.CreateConnection<LogicValue>();
        var outSum = graph.CreateConnection<LogicValue>();
        var outC   = graph.CreateConnection<LogicValue>();

        graph.AddNode(new PulseValue<LogicValue>(a)).SetPort(Ports.Output, ain);
        graph.AddNode(new PulseValue<LogicValue>(b)).SetPort(Ports.Output, bin);
        graph.AddNode(new PulseValue<LogicValue>(cIn)).SetPort(Ports.Output, cin);

        graph.AddNode(new FullAdder())
            .SetPort(FullAdder.InputA,      ain)
            .SetPort(FullAdder.InputB,      bin)
            .SetPort(FullAdder.InputC,      cin)
            .SetPort(FullAdder.OutputSum,   outSum)
            .SetPort(FullAdder.OutputCarry, outC);

        var sumCell   = new Register<LogicValue>();
        var carryCell = new Register<LogicValue>();
        graph.AddNode(sumCell).SetPort(Ports.Input, outSum);
        graph.AddNode(carryCell).SetPort(Ports.Input, outC);

        graph.Evaluate();

        Assert.That(sumCell.GetValue(),   Is.EqualTo(expectedSum));
        Assert.That(carryCell.GetValue(), Is.EqualTo(expectedCarry));
    }
}
