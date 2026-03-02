using Hypnode.Async;
using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common
{
    public abstract class PulseTests<TGraph> where TGraph : INodeGraph, new()
    {
        [TestCase(LogicValue.False)]
        [TestCase(LogicValue.True)]
        public async Task TestPulse_CorrectValue(LogicValue value)
        {
            var graph = new TGraph();

            var pulse = graph.AddNode(new PulseValue<LogicValue>(value));
            var result = graph.AddNode(new Register<LogicValue>());

            graph.AddConnection<LogicValue>(pulse, "OUT", result, "IN");

            await graph.EvaluateAsync();

            Assert.That(result.GetValue(), Is.EqualTo(value));
        }

        [TestCase(0)]
        [TestCase(100)]
        [TestCase(20000)]
        [TestCase(-100)]
        public async Task TestPulse_SendCloseExecuteOnce(int value)
        {
            var graph = new TGraph();

            var connection = new Mock<Connection<int>>();
            var pulse = graph.AddNode(new PulseValue<int>(value));

            pulse.SetPort("OUT", connection.Object);

            await graph.EvaluateAsync();

            connection.Verify(c => c.Send(value), Times.Once);
            connection.Verify(c => c.Close(), Times.Once);
        }
    }

    [TestFixture]
    public class AsyncNodeGraph_PulseTests : PulseTests<AsyncNodeGraph>
    {

    }

    [TestFixture]
    public class SequenceNodeGraph_PulseTests : PulseTests<SequenceNodeGraph>
    {

    }
}
