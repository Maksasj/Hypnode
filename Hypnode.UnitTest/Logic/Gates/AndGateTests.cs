using Hypnode.Async;
using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Gates
{
    public abstract class AndGateTests<TGraph> where TGraph : INodeGraph, new()
    {

        [TestCase(LogicValue.False, LogicValue.False, LogicValue.False)]
        [TestCase(LogicValue.False, LogicValue.True, LogicValue.False)]
        [TestCase(LogicValue.True, LogicValue.False, LogicValue.False)]
        [TestCase(LogicValue.True, LogicValue.True, LogicValue.True)]
        public async Task TestAnd_CorrectValue(LogicValue a, LogicValue b, LogicValue expect)
        {
            var graph = new TGraph();
            var connection1 = graph.CreateConnection<LogicValue>();
            var connection2 = graph.CreateConnection<LogicValue>();
            var connection3 = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<LogicValue>(a))
                .SetPort("OUT", connection1);

            graph.AddNode(new PulseValue<LogicValue>(b))
                .SetPort("OUT", connection2);

            graph.AddNode(new AndGate())
                .SetPort("INA", connection1)
                .SetPort("INB", connection2)
                .SetPort("OUT", connection3);


            var result = new Register<LogicValue>();
            graph.AddNode(result).SetPort("IN", connection3);

            await graph.EvaluateAsync();

            Assert.That(result.GetValue(), Is.EqualTo(expect));
        }
    }

    [TestFixture]
    public class AsyncNodeGrap_AndGateTests : AndGateTests<AsyncNodeGraph>
    {

    }

    [TestFixture]
    public class SequenceNodeGraph_AndGateTests : AndGateTests<SequenceNodeGraph>
    {

    }
}
