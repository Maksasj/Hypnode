using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Types;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class FilterNodeTests
{
    [Test]
    public void TestFilter_PassingPacket_Forwarded()
    {
        var graph = new CoroutineNodeGraph();
        var pulse = graph.AddNode(new PulseValue(new IntValue(4)));
        var filter = graph.AddNode(new FilterNode(v => v.AsInt() % 2 == 0));
        var result = graph.AddNode(new Register());

        graph.AddConnection(pulse, Ports.Output, filter, Ports.Input);
        graph.AddConnection(filter, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(4));
    }

    [Test]
    public void TestFilter_BlockedPacket_NotForwarded()
    {
        var graph = new CoroutineNodeGraph();
        var connIn = graph.CreateConnection();
        var connOut = new Mock<Connection<HypnodeValue>>();

        graph.AddNode(new PulseValue(new IntValue(3))).SetPort(Ports.Output, connIn);
        graph.AddNode(new FilterNode(v => v.AsInt() % 2 == 0)).SetPort(Ports.Input, connIn).SetPort(Ports.Output, connOut.Object);

        graph.Evaluate();

        connOut.Verify(c => c.Send(It.IsAny<HypnodeValue>()), Times.Never);
        connOut.Verify(c => c.Close(), Times.Once);
    }

    [Test]
    public void TestFilter_MultiplePackets_OnlyPassingForwarded()
    {
        var graph = new CoroutineNodeGraph();
        var multi = graph.AddNode(new MultiPulseValue(new int[] { 1, 2, 3, 4, 5, 6 }.Select(i => (HypnodeValue)new IntValue(i))));
        var filter = graph.AddNode(new FilterNode(v => v.AsInt() % 2 == 0));
        var result = graph.AddNode(new Register());

        graph.AddConnection(multi, Ports.Output, filter, Ports.Input);
        graph.AddConnection(filter, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(6));
    }

    [Test]
    public void TestFilter_AllBlocked_OutputClosedWithNoSend()
    {
        var graph = new CoroutineNodeGraph();
        var connIn = graph.CreateConnection();
        var connOut = new Mock<Connection<HypnodeValue>>();

        graph.AddNode(new MultiPulseValue(new int[] { 1, 3, 5 }.Select(i => (HypnodeValue)new IntValue(i)))).SetPort(Ports.Output, connIn);
        graph.AddNode(new FilterNode(v => v.AsInt() % 2 == 0)).SetPort(Ports.Input, connIn).SetPort(Ports.Output, connOut.Object);

        graph.Evaluate();

        connOut.Verify(c => c.Send(It.IsAny<HypnodeValue>()), Times.Never);
        connOut.Verify(c => c.Close(), Times.Once);
    }
}
