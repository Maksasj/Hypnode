using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Utils;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Utils;

public abstract class ByteSplitterInTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(0b00000000, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(0b10000000, LogicValue.True,  LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(0b11111111, LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True)]
    [TestCase(0b01010101, LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True)]
    [TestCase(0b10101010, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False)]
    public void TestByteSplitterIn_CorrectValues(byte value,
        LogicValue b7e, LogicValue b6e, LogicValue b5e, LogicValue b4e,
        LogicValue b3e, LogicValue b2e, LogicValue b1e, LogicValue b0e)
    {
        var graph = new TGraph();
        var input = graph.CreateConnection<byte>();
        var bits  = Enumerable.Range(0, 8).Select(_ => graph.CreateConnection<LogicValue>()).ToArray();

        graph.AddNode(new PulseValue<byte>(value)).SetPort(Ports.Output, input);

        var splitter = graph.AddNode(new ByteSplitterIn()).SetPort(Ports.Input, input);
        for (int i = 0; i < 8; i++) splitter.SetPort(i.ToString(), bits[i]);

        var registers = bits.Select(b => { var r = new Register<LogicValue>(); graph.AddNode(r).SetPort(Ports.Input, b); return r; }).ToArray();

        graph.Evaluate();

        LogicValue[] expected = [b0e, b1e, b2e, b3e, b4e, b5e, b6e, b7e];
        for (int i = 0; i < 8; i++)
            Assert.That(registers[i].GetValue(), Is.EqualTo(expected[i]));
    }
}

[TestFixture] public class CoroutineNodeGraph_ByteSplitterInTests : ByteSplitterInTests<CoroutineNodeGraph> { }
