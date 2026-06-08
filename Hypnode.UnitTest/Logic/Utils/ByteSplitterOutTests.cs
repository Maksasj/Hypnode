using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Utils;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Utils;

[TestFixture]
public class ByteSplitterOutTests
{
    [TestCase(0b00000000, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(0b10000000, LogicValue.True,  LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(0b11111111, LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True,  LogicValue.True)]
    [TestCase(0b01010101, LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True)]
    [TestCase(0b10101010, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False, LogicValue.True,  LogicValue.False)]
    public void TestByteSplitterOut_CorrectValues(byte expected,
        LogicValue b7, LogicValue b6, LogicValue b5, LogicValue b4,
        LogicValue b3, LogicValue b2, LogicValue b1, LogicValue b0)
    {
        var graph = new CoroutineNodeGraph();
        LogicValue[] bits = [b0, b1, b2, b3, b4, b5, b6, b7];

        var conns = Enumerable.Range(0, 8).Select(_ => graph.CreateConnection<LogicValue>()).ToArray();
        for (int i = 0; i < 8; i++)
            graph.AddNode(new PulseValue<LogicValue>(bits[i])).SetPort(Ports.Output, conns[i]);

        var splitter = graph.AddNode(new ByteSplitterOut());
        for (int i = 0; i < 8; i++) splitter.SetPort(i.ToString(), conns[i]);

        var resultConn = graph.CreateConnection<byte>();
        splitter.SetPort(Ports.Output, resultConn);

        var result = new Register<byte>();
        graph.AddNode(result).SetPort(Ports.Input, resultConn);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(expected));
    }
}
