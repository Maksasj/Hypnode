using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Utils;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Utils;

public abstract class ByteSplitterOutTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(0b00000000, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(0b10000000, LogicValue.True, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(0b11111111, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True)]
    [TestCase(0b01010101, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True)]
    [TestCase(0b10101010, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False)]
    public void TestByteSplitterOut_CorrectValues(byte expected, LogicValue b7, LogicValue b6, LogicValue b5, LogicValue b4, LogicValue b3, LogicValue b2, LogicValue b1, LogicValue b0)
    {
        var graph = new TGraph();

        var b0n = graph.AddNode(new PulseValue<LogicValue>(b0));
        var b1n = graph.AddNode(new PulseValue<LogicValue>(b1));
        var b2n = graph.AddNode(new PulseValue<LogicValue>(b2));
        var b3n = graph.AddNode(new PulseValue<LogicValue>(b3));
        var b4n = graph.AddNode(new PulseValue<LogicValue>(b4));
        var b5n = graph.AddNode(new PulseValue<LogicValue>(b5));
        var b6n = graph.AddNode(new PulseValue<LogicValue>(b6));
        var b7n = graph.AddNode(new PulseValue<LogicValue>(b7));

        var splitter = graph.AddNode(new ByteSplitterOut());

        graph.AddConnection<LogicValue>(b0n, "OUT", splitter, "0");
        graph.AddConnection<LogicValue>(b1n, "OUT", splitter, "1");
        graph.AddConnection<LogicValue>(b2n, "OUT", splitter, "2");
        graph.AddConnection<LogicValue>(b3n, "OUT", splitter, "3");
        graph.AddConnection<LogicValue>(b4n, "OUT", splitter, "4");
        graph.AddConnection<LogicValue>(b5n, "OUT", splitter, "5");
        graph.AddConnection<LogicValue>(b6n, "OUT", splitter, "6");
        graph.AddConnection<LogicValue>(b7n, "OUT", splitter, "7");

        var result = graph.AddNode(new Register<byte>());
        graph.AddConnection<byte>(splitter, "OUT", result, "IN");

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(expected));
    }
}

[TestFixture]
public class CoroutineNodeGraph_ByteSplitterOutTests : ByteSplitterOutTests<CoroutineNodeGraph>
{

}

[TestFixture]
public class SequenceNodeGraph_ByteSplitterOutTests : ByteSplitterOutTests<SequenceNodeGraph>
{

}
