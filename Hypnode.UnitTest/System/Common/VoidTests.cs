using Hypnode.Core;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

public abstract class VoidTests<TGraph> where TGraph : INodeGraph, new()
{
    [Test]
    public void TestVoid_SingleConnection()
    {
        var graph = new TGraph();
        var conn  = graph.CreateConnection<byte>();
        graph.AddNode(new PulseValue<byte>(1)).SetPort(Ports.Output, conn);
        graph.AddNode(new VoidSink<byte>()).SetPort(VoidSink<byte>.Input, conn);
        graph.Evaluate();
        Assert.Pass();
    }

    [Test]
    public void TestVoid_SingleConnection_TryReceiveOnce()
    {
        var graph = new TGraph();
        var connection = new Mock<Connection<int>>();
        connection.Setup(c => c.TryReceive(out It.Ref<int>.IsAny)).Returns(false);

        graph.AddNode(new VoidSink<int>()).SetPort(VoidSink<int>.Input, connection.Object);
        graph.Evaluate();

        connection.Verify(c => c.TryReceive(out It.Ref<int>.IsAny), Times.Once);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(10)]
    public void TestVoid_MultipleConnections_TryReceiveOnce(int count)
    {
        var graph = new TGraph();
        var sink  = graph.AddNode(new VoidSink<int>());
        var mocks = Enumerable.Range(0, count).Select(_ =>
        {
            var m = new Mock<Connection<int>>();
            m.Setup(c => c.TryReceive(out It.Ref<int>.IsAny)).Returns(false);
            sink.SetPort(VoidSink<int>.Input, m.Object);
            return m;
        }).ToList();

        graph.Evaluate();

        foreach (var m in mocks)
            m.Verify(c => c.TryReceive(out It.Ref<int>.IsAny), Times.Once());
    }

    [Test]
    public void TestVoid_MultipleConnections()
    {
        var graph = new TGraph();
        var conn1 = graph.CreateConnection<byte>();
        var conn2 = graph.CreateConnection<byte>();
        graph.AddNode(new PulseValue<byte>(1)).SetPort(Ports.Output, conn1);
        graph.AddNode(new PulseValue<byte>(2)).SetPort(Ports.Output, conn2);
        graph.AddNode(new VoidSink<byte>()).SetPort(VoidSink<byte>.Input, conn1).SetPort(VoidSink<byte>.Input, conn2);
        graph.Evaluate();
        Assert.Pass();
    }
}

[TestFixture] public class CoroutineNodeGraph_VoidTests : VoidTests<CoroutineNodeGraph> { }
