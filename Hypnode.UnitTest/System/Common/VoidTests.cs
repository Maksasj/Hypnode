using Hypnode.Core;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class VoidTests
{
    [Test]
    public void TestVoid_SingleConnection()
    {
        var graph = new CoroutineNodeGraph();
        var conn = graph.CreateConnection<byte>();
        graph.AddNode(new PulseValue<byte>(1)).SetPort(Ports.Output, conn);
        graph.AddNode(new VoidSink<byte>()).SetPort(VoidSink<byte>.Input, conn);
        graph.Evaluate();
        Assert.Pass();
    }

    [Test]
    public void TestVoid_SingleConnection_TryReceiveOnce()
    {
        var graph = new CoroutineNodeGraph();
        var conn = new Mock<Connection<int>>();
        conn.Setup(c => c.TryReceive(out It.Ref<int>.IsAny)).Returns(false);
        graph.AddNode(new VoidSink<int>()).SetPort(VoidSink<int>.Input, conn.Object);

        graph.Evaluate();

        conn.Verify(c => c.TryReceive(out It.Ref<int>.IsAny), Times.Once);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(10)]
    public void TestVoid_MultipleConnections_TryReceiveOnce(int count)
    {
        var graph = new CoroutineNodeGraph();
        var sink = graph.AddNode(new VoidSink<int>());
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
        var graph = new CoroutineNodeGraph();
        var conn1 = graph.CreateConnection<byte>();
        var conn2 = graph.CreateConnection<byte>();
        graph.AddNode(new PulseValue<byte>(1)).SetPort(Ports.Output, conn1);
        graph.AddNode(new PulseValue<byte>(2)).SetPort(Ports.Output, conn2);
        graph.AddNode(new VoidSink<byte>())
            .SetPort(VoidSink<byte>.Input, conn1)
            .SetPort(VoidSink<byte>.Input, conn2);
        graph.Evaluate();
        Assert.Pass();
    }
}
