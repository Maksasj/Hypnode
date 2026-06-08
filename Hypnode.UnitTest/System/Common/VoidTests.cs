using Hypnode.Core;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common
{
    public abstract class VoidTests<TGraph> where TGraph : INodeGraph, new()
    {
        [Test]
        public void TestVoid_SingleConnection()
        {
            var graph = new TGraph();
            var connection = graph.CreateConnection<byte>();

            graph.AddNode(new PulseValue<byte>(1)).SetPort("OUT", connection);
            graph.AddNode(new VoidSink<byte>()).SetPort("_", connection);

            graph.Evaluate();

            Assert.Pass();
        }

        [Test]
        public void TestVoid_SingleConnection_TryReceiveOnce()
        {
            var graph = new TGraph();

            var connection = new Mock<Connection<int>>();
            connection.Setup(c => c.TryReceive(out It.Ref<int>.IsAny)).Returns(false);

            var sink = graph.AddNode(new VoidSink<int>());
            sink.SetPort("_", connection.Object);

            graph.Evaluate();

            connection.Verify(c => c.TryReceive(out It.Ref<int>.IsAny), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        public void TestVoid_MultipleConnection_TryReceiveOnce(int connectionCount)
        {
            var graph = new TGraph();

            var sink = graph.AddNode(new VoidSink<int>());
            var connections = new List<Mock<Connection<int>>>();

            for (int i = 0; i < connectionCount; ++i)
            {
                var connection = new Mock<Connection<int>>();
                connections.Add(connection);
                connection.Setup(c => c.TryReceive(out It.Ref<int>.IsAny)).Returns(false);

                sink.SetPort("_", connection.Object);
            }

            graph.Evaluate();

            foreach (var mockConnection in connections)
                mockConnection.Verify(c => c.TryReceive(out It.Ref<int>.IsAny), Times.Once());
        }

        [Test]
        public void TestVoid_MultipleConnections()
        {
            var graph = new TGraph();
            var connection1 = graph.CreateConnection<byte>();
            var connection2 = graph.CreateConnection<byte>();

            graph.AddNode(new PulseValue<byte>(1)).SetPort("OUT", connection1);
            graph.AddNode(new PulseValue<byte>(1)).SetPort("OUT", connection2);

            graph.AddNode(new VoidSink<byte>())
                .SetPort("_", connection1)
                .SetPort("_", connection2);

            graph.Evaluate();

            Assert.Pass();
        }
    }

    [TestFixture]
    public class CoroutineNodeGraph_VoidTests : VoidTests<CoroutineNodeGraph>
    {

    }

    [TestFixture]
    public class SequenceNodeGraph_VoidTests : VoidTests<SequenceNodeGraph>
    {

    }
}
