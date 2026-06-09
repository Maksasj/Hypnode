using Hypnode.Core;
using Hypnode.Core.Connections;
using Hypnode.Core.Graph;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Runtime;

[TestFixture]
public class BoundedConnectionTests
{
    [Test]
    public void TestBoundedConnection_WithinCapacity_SendsAndReceives()
    {
        var conn = new BoundedQueueConnection<int>(3);
        conn.Send(1);
        conn.Send(2);

        Assert.That(conn.HasData, Is.True);
        Assert.That(conn.IsFull, Is.False);
        Assert.That(conn.Receive(), Is.EqualTo(1));
        Assert.That(conn.Receive(), Is.EqualTo(2));
        Assert.That(conn.HasData, Is.False);
    }

    [Test]
    public void TestBoundedConnection_AtCapacity_IsFullTrue()
    {
        var conn = new BoundedQueueConnection<int>(2);
        conn.Send(1);
        conn.Send(2);

        Assert.That(conn.IsFull, Is.True);
    }

    [Test]
    public void TestBoundedConnection_Overflow_Throws()
    {
        var conn = new BoundedQueueConnection<int>(1);
        conn.Send(42);

        Assert.Throws<InvalidOperationException>(() => conn.Send(99));
    }

    [Test]
    public void TestBoundedConnection_ZeroCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoundedQueueConnection<int>(0));
    }

    [Test]
    public void TestBoundedConnection_AfterReceive_IsFullFalse()
    {
        var conn = new BoundedQueueConnection<int>(1);
        conn.Send(10);
        Assert.That(conn.IsFull, Is.True);

        conn.Receive();
        Assert.That(conn.IsFull, Is.False);
    }

    [Test]
    public void TestBoundedConnection_InGraph_DataFlowsCorrectly()
    {
        var graph = new CoroutineNodeGraph();
        var pulse = graph.AddNode(new PulseValue<int>(7));
        var result = graph.AddNode(new Register<int>());

        graph.AddBoundedConnection<int>(pulse, Ports.Output, result, Ports.Input, capacity: 4);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(7));
    }

    [Test]
    public void TestBoundedConnection_UnboundedDefaultIsFull_AlwaysFalse()
    {
        var conn = new QueueConnection<int>();
        Assert.That(conn.IsFull, Is.False);
        for (int i = 0; i < 1000; i++) conn.Send(i);
        Assert.That(conn.IsFull, Is.False);
    }
}
