using Hypnode.Core;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class ZipNodeTests
{
    [Test]
    public void TestZip_SinglePair_CorrectTuple()
    {
        var graph = new CoroutineNodeGraph();
        var conn1 = graph.CreateConnection<int>();
        var conn2 = graph.CreateConnection<string>();
        var connOut = graph.CreateConnection<(int, string)>();

        graph.AddNode(new PulseValue<int>(42)).SetPort(Ports.Output, conn1);
        graph.AddNode(new PulseValue<string>("hello")).SetPort(Ports.Output, conn2);
        graph.AddNode(new ZipNode<int, string>())
            .SetPort(ZipNode<int, string>.Input1, conn1)
            .SetPort(ZipNode<int, string>.Input2, conn2)
            .SetPort(Ports.Output, connOut);

        var result = graph.AddNode(new Register<(int, string)>());
        result.SetPort(Ports.Input, connOut);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo((42, "hello")));
    }

    [Test]
    public void TestZip_MultiplePackets_LastPairReceived()
    {
        var graph = new CoroutineNodeGraph();
        var conn1 = graph.CreateConnection<int>();
        var conn2 = graph.CreateConnection<int>();
        var connOut = graph.CreateConnection<(int, int)>();

        graph.AddNode(new MultiPulseValue<int>([1, 2, 3])).SetPort(Ports.Output, conn1);
        graph.AddNode(new MultiPulseValue<int>([10, 20, 30])).SetPort(Ports.Output, conn2);
        graph.AddNode(new ZipNode<int, int>())
            .SetPort(ZipNode<int, int>.Input1, conn1)
            .SetPort(ZipNode<int, int>.Input2, conn2)
            .SetPort(Ports.Output, connOut);

        var result = graph.AddNode(new Register<(int, int)>());
        result.SetPort(Ports.Input, connOut);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo((3, 30)));
    }

    [Test]
    public void TestZip_ClosesOutput()
    {
        var graph = new CoroutineNodeGraph();
        var conn1 = graph.CreateConnection<int>();
        var conn2 = graph.CreateConnection<int>();
        var connOut = new Mock<Connection<(int, int)>>();

        graph.AddNode(new PulseValue<int>(1)).SetPort(Ports.Output, conn1);
        graph.AddNode(new PulseValue<int>(2)).SetPort(Ports.Output, conn2);
        graph.AddNode(new ZipNode<int, int>())
            .SetPort(ZipNode<int, int>.Input1, conn1)
            .SetPort(ZipNode<int, int>.Input2, conn2)
            .SetPort(Ports.Output, connOut.Object);

        graph.Evaluate();

        connOut.Verify(c => c.Close(), Times.Once);
    }

    [Test]
    public void TestZip_SumOfPairs()
    {
        var graph = new CoroutineNodeGraph();
        var conn1 = graph.CreateConnection<int>();
        var conn2 = graph.CreateConnection<int>();
        var connZip = graph.CreateConnection<(int, int)>();
        var connOut = graph.CreateConnection<int>();

        graph.AddNode(new MultiPulseValue<int>([1, 2, 3])).SetPort(Ports.Output, conn1);
        graph.AddNode(new MultiPulseValue<int>([4, 5, 6])).SetPort(Ports.Output, conn2);
        graph.AddNode(new ZipNode<int, int>())
            .SetPort(ZipNode<int, int>.Input1, conn1)
            .SetPort(ZipNode<int, int>.Input2, conn2)
            .SetPort(Ports.Output, connZip);
        graph.AddNode(new MapNode<(int, int), int>(t => t.Item1 + t.Item2)).SetPort(Ports.Input, connZip).SetPort(Ports.Output, connOut);

        var result = graph.AddNode(new Register<int>());
        result.SetPort(Ports.Input, connOut);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(9));
    }
}
