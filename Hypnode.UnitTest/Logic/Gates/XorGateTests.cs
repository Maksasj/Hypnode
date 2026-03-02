using Hypnode.Async;
using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Gates
{
    public abstract class XorGateTests<TGraph> where TGraph : INodeGraph, new()
    {
        [TestCase(LogicValue.False, LogicValue.False, LogicValue.False)]
        [TestCase(LogicValue.False, LogicValue.True, LogicValue.True)]
        [TestCase(LogicValue.True, LogicValue.False, LogicValue.True)]
        [TestCase(LogicValue.True, LogicValue.True, LogicValue.False)]
        public async Task TestXor_CorrectValue(LogicValue a, LogicValue b, LogicValue expect)
        {
            var graph = new TGraph();
            var connection1 = graph.CreateConnection<LogicValue>();
            var connection2 = graph.CreateConnection<LogicValue>();
            var connection3 = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<LogicValue>(a))
                .SetPort("OUT", connection1);

            graph.AddNode(new PulseValue<LogicValue>(b))
                .SetPort("OUT", connection2);

            graph.AddNode(new XorGate())
                .SetPort("INA", connection1)
                .SetPort("INB", connection2)
                .SetPort("OUT", connection3);

            var result = new Register<LogicValue>();
            graph.AddNode(result).SetPort("IN", connection3);

            await graph.EvaluateAsync();

            Assert.That(expect, Is.EqualTo(result.GetValue()));
        }
    }

    [TestFixture]
    public class AsyncNodeGrap_XorGateTests : XorGateTests<AsyncNodeGraph>
    {

    }

    [TestFixture]
    public class SequenceNodeGraph_XorGateTests : XorGateTests<SequenceNodeGraph>
    {

    }
}
