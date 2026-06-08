using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Utils;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Utils;

public abstract class ByteSplitterInTests<TGraph> where TGraph : INodeGraph, new()
{
    [TestCase(0b00000000, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(0b10000000, LogicValue.True, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
    [TestCase(0b11111111, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True)]
    [TestCase(0b01010101, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True)]
    [TestCase(0b10101010, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False)]
    public void TestByteSplitterIn_CorrectValues(byte value, LogicValue b7e, LogicValue b6e, LogicValue b5e, LogicValue b4e, LogicValue b3e, LogicValue b2e, LogicValue b1e, LogicValue b0e)
    {
        var graph = new TGraph();
        var input = graph.CreateConnection<byte>();

        var b0c = graph.CreateConnection<LogicValue>();
        var b1c = graph.CreateConnection<LogicValue>();
        var b2c = graph.CreateConnection<LogicValue>();
        var b3c = graph.CreateConnection<LogicValue>();
        var b4c = graph.CreateConnection<LogicValue>();
        var b5c = graph.CreateConnection<LogicValue>();
        var b6c = graph.CreateConnection<LogicValue>();
        var b7c = graph.CreateConnection<LogicValue>();

        graph.AddNode(new PulseValue<byte>(value)).SetPort("OUT", input);

        graph.AddNode(new ByteSplitterIn())
            .SetPort("IN", input)
            .SetPort(0.ToString(), b0c)
            .SetPort(1.ToString(), b1c)
            .SetPort(2.ToString(), b2c)
            .SetPort(3.ToString(), b3c)
            .SetPort(4.ToString(), b4c)
            .SetPort(5.ToString(), b5c)
            .SetPort(6.ToString(), b6c)
            .SetPort(7.ToString(), b7c);

        var b0 = new Register<LogicValue>(); graph.AddNode(b0).SetPort("IN", b0c);
        var b1 = new Register<LogicValue>(); graph.AddNode(b1).SetPort("IN", b1c);
        var b2 = new Register<LogicValue>(); graph.AddNode(b2).SetPort("IN", b2c);
        var b3 = new Register<LogicValue>(); graph.AddNode(b3).SetPort("IN", b3c);
        var b4 = new Register<LogicValue>(); graph.AddNode(b4).SetPort("IN", b4c);
        var b5 = new Register<LogicValue>(); graph.AddNode(b5).SetPort("IN", b5c);
        var b6 = new Register<LogicValue>(); graph.AddNode(b6).SetPort("IN", b6c);
        var b7 = new Register<LogicValue>(); graph.AddNode(b7).SetPort("IN", b7c);

        graph.Evaluate();

        Assert.Multiple(() =>
        {
            Assert.That(b0.GetValue(), Is.EqualTo(b0e));
            Assert.That(b1.GetValue(), Is.EqualTo(b1e));
            Assert.That(b2.GetValue(), Is.EqualTo(b2e));
            Assert.That(b3.GetValue(), Is.EqualTo(b3e));
            Assert.That(b4.GetValue(), Is.EqualTo(b4e));
            Assert.That(b5.GetValue(), Is.EqualTo(b5e));
            Assert.That(b6.GetValue(), Is.EqualTo(b6e));
            Assert.That(b7.GetValue(), Is.EqualTo(b7e));
        });
    }
}

[TestFixture]
public class CoroutineNodeGraph_ByteSplitterInTests : ByteSplitterInTests<CoroutineNodeGraph>
{

}

[TestFixture]
public class SequenceNodeGraph_ByteSplitterInTests : ByteSplitterInTests<SequenceNodeGraph>
{

}
