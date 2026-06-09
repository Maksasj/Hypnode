using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Types;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class IfNodeTests
{
    [Test]
    public void TestIf_TruePacket_RoutedToThen()
    {
        var graph = new CoroutineNodeGraph();
        var connIn = graph.CreateConnection();
        var connThen = graph.CreateConnection();
        var connElse = graph.CreateConnection();

        graph.AddNode(new PulseValue(new IntValue(4))).SetPort(Ports.Output, connIn);
        graph.AddNode(new IfNode(v => v.AsInt() % 2 == 0))
            .SetPort(Ports.Input, connIn)
            .SetPort(IfNode.Then, connThen)
            .SetPort(IfNode.Else, connElse);

        var thenResult = graph.AddNode(new Register());
        thenResult.SetPort(Ports.Input, connThen);
        graph.AddNode(new VoidSink()).SetPort(VoidSink.Input, connElse);

        graph.Evaluate();

        Assert.That(thenResult.GetValue()!.AsInt(), Is.EqualTo(4));
    }

    [Test]
    public void TestIf_FalsePacket_RoutedToElse()
    {
        var graph = new CoroutineNodeGraph();
        var connIn = graph.CreateConnection();
        var connThen = graph.CreateConnection();
        var connElse = graph.CreateConnection();

        graph.AddNode(new PulseValue(new IntValue(3))).SetPort(Ports.Output, connIn);
        graph.AddNode(new IfNode(v => v.AsInt() % 2 == 0))
            .SetPort(Ports.Input, connIn)
            .SetPort(IfNode.Then, connThen)
            .SetPort(IfNode.Else, connElse);

        graph.AddNode(new VoidSink()).SetPort(VoidSink.Input, connThen);
        var elseResult = graph.AddNode(new Register());
        elseResult.SetPort(Ports.Input, connElse);

        graph.Evaluate();

        Assert.That(elseResult.GetValue()!.AsInt(), Is.EqualTo(3));
    }

    [Test]
    public void TestIf_TruePacket_NotSentToElse()
    {
        var graph = new CoroutineNodeGraph();
        var connIn = graph.CreateConnection();
        var connThen = new Mock<Connection>();
        var connElse = new Mock<Connection>();

        graph.AddNode(new PulseValue(new IntValue(10))).SetPort(Ports.Output, connIn);
        graph.AddNode(new IfNode(v => v.AsInt() > 5))
            .SetPort(Ports.Input, connIn)
            .SetPort(IfNode.Then, connThen.Object)
            .SetPort(IfNode.Else, connElse.Object);

        graph.Evaluate();

        connThen.Verify(c => c.Send(new IntValue(10)), Times.Once);
        connElse.Verify(c => c.Send(It.IsAny<HypnodeValue>()), Times.Never);
        connThen.Verify(c => c.Close(), Times.Once);
        connElse.Verify(c => c.Close(), Times.Once);
    }

    [Test]
    public void TestIf_MultiplePackets_SplitCorrectly()
    {
        var graph = new CoroutineNodeGraph();
        var connIn = graph.CreateConnection();
        var connThen = graph.CreateConnection();
        var connElse = graph.CreateConnection();

        graph.AddNode(new MultiPulseValue(new int[] { 1, 2, 3, 4, 5, 6 }.Select(i => (HypnodeValue)new IntValue(i)))).SetPort(Ports.Output, connIn);
        graph.AddNode(new IfNode(v => v.AsInt() % 2 == 0))
            .SetPort(Ports.Input, connIn)
            .SetPort(IfNode.Then, connThen)
            .SetPort(IfNode.Else, connElse);

        var evenResult = graph.AddNode(new Register());
        var oddResult = graph.AddNode(new Register());
        evenResult.SetPort(Ports.Input, connThen);
        oddResult.SetPort(Ports.Input, connElse);

        graph.Evaluate();

        Assert.That(evenResult.GetValue()!.AsInt(), Is.EqualTo(6));
        Assert.That(oddResult.GetValue()!.AsInt(), Is.EqualTo(5));
    }
}
