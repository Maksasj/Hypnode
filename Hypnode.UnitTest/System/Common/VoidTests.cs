using Hypnode.Async;
using Hypnode.Core;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common
{
    public abstract class VoidTests<TGraph> where TGraph : INodeGraph, new()
    {
        [Test]
        public async Task TestVoid_SingleConnection()
        {
            var graph = new TGraph();
            var connection = graph.CreateConnection<byte>();

            graph.AddNode(new PulseValue<byte>(1))
                .SetPort("OUT", connection);

            graph.AddNode(new VoidSink<byte>())
                .SetPort("_", connection);

            await graph.EvaluateAsync();

            Assert.Pass();
        }

        [Test]
        public async Task TestVoid_SingleConnection_TryReceiveOnce()
        {
            var graph = new TGraph();

            var connection = new Mock<Connection<int>>();
            connection.Setup(c => c.TryReceive(out It.Ref<int>.IsAny)).Returns(false);

            var sink = graph.AddNode(new VoidSink<int>());
            sink.SetPort("_", connection.Object);

            await graph.EvaluateAsync();

            connection.Verify(c => c.TryReceive(out It.Ref<int>.IsAny), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        public async Task TestVoid_MultipleConnection_TryReceiveOnce(int connectionCount)
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

            await graph.EvaluateAsync();

            foreach (var mockConnection in connections)
                mockConnection.Verify(c => c.TryReceive(out It.Ref<int>.IsAny), Times.Once());
        }

        [Test]
        public async Task TestVoid_MultipleConnections()
        {
            var graph = new TGraph();
            var connection1 = graph.CreateConnection<byte>();
            var connection2 = graph.CreateConnection<byte>();

            graph.AddNode(new PulseValue<byte>(1))
                .SetPort("OUT", connection1);

            graph.AddNode(new PulseValue<byte>(1))
                .SetPort("OUT", connection2);

            graph.AddNode(new VoidSink<byte>())
                .SetPort("_", connection1)
                .SetPort("_", connection2);

            await graph.EvaluateAsync();

            Assert.Pass();
        }
    }

    [TestFixture]
    public class AsyncNodeGraph_VoidTests : VoidTests<AsyncNodeGraph>
    {

    }

    [TestFixture]
    public class SequenceNodeGraph_VoidTests : VoidTests<SequenceNodeGraph>
    {

    }
}
