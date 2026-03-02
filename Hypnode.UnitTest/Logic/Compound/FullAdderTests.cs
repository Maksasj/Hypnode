using Hypnode.Async;
using Hypnode.Core;
using Hypnode.Logic;
using Hypnode.Logic.Compound;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Compound
{
    public abstract class FullAdderTests<TGraph> where TGraph : INodeGraph, new()
    {
        [TestCase(LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False, LogicValue.False)]
        [TestCase(LogicValue.False, LogicValue.False, LogicValue.True, LogicValue.True, LogicValue.False)]
        [TestCase(LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False)]
        [TestCase(LogicValue.False, LogicValue.True, LogicValue.True, LogicValue.False, LogicValue.True)]
        [TestCase(LogicValue.True, LogicValue.False, LogicValue.False, LogicValue.True, LogicValue.False)]
        [TestCase(LogicValue.True, LogicValue.False, LogicValue.True, LogicValue.False, LogicValue.True)]
        [TestCase(LogicValue.True, LogicValue.True, LogicValue.False, LogicValue.False, LogicValue.True)]
        [TestCase(LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True, LogicValue.True)]
        public async Task TestAdderCompound_CorrectValues(LogicValue a, LogicValue b, LogicValue cIn, LogicValue sum, LogicValue cOut)
        {
            var graph = new TGraph();
            var ain = graph.CreateConnection<LogicValue>();
            var bin = graph.CreateConnection<LogicValue>();
            var cin = graph.CreateConnection<LogicValue>();

            var outsum = graph.CreateConnection<LogicValue>();
            var outc = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<LogicValue>(a))
                .SetPort("OUT", ain);

            graph.AddNode(new PulseValue<LogicValue>(b))
                .SetPort("OUT", bin);

            graph.AddNode(new PulseValue<LogicValue>(cIn))
                .SetPort("OUT", cin);

            graph.AddNode(new FullAdder(new AsyncNodeGraph()))
                .SetPort("INA", ain)
                .SetPort("INB", bin)
                .SetPort("INC", cin)
                .SetPort("OUTSUM", outsum)
                .SetPort("OUTC", outc);

            var sumCell = new Register<LogicValue>();
            graph.AddNode(sumCell).SetPort("IN", outsum);

            var carryCell = new Register<LogicValue>();
            graph.AddNode(carryCell).SetPort("IN", outc);

            await graph.EvaluateAsync();

            Assert.That(sumCell.GetValue(), Is.EqualTo(sum));
            Assert.That(carryCell.GetValue(), Is.EqualTo(cOut));
        }

        [TestCase(0b00000000, 0b00000000)]
        [TestCase(0b00001010, 0b00000000)]
        [TestCase(0b00000000, 0b00001010)]
        [TestCase(0b00000001, 0b00000001)]
        [TestCase(0b00001010, 0b00110010)]
        [TestCase(0b01100100, 0b01100100)]
        [TestCase(0b10000000, 0b01111111)]
        [TestCase(0b11001000, 0b00110111)]
        [TestCase(0b11111010, 0b00000101)]
        [TestCase(0b00000000, 0b11111111)]
        [TestCase(0b11111111, 0b00000000)]
        public async Task TestAdderByteCompound_CorrectValues(byte a, byte b)
        {
            var graph = new TGraph();
            var ain = graph.CreateConnection<byte>();
            var bin = graph.CreateConnection<byte>();

            var outsum = graph.CreateConnection<byte>();

            graph.AddNode(new PulseValue<byte>(a))
                .SetPort("OUT", ain);

            graph.AddNode(new PulseValue<byte>(b))
               .SetPort("OUT", bin);

            graph.AddNode(new FullAdderByte(new TGraph()))
                .SetPort("INA", ain)
                .SetPort("INB", bin)
                .SetPort("OUTSUM", outsum);

            var sumCell = new Register<byte>();
            graph.AddNode(sumCell).SetPort("IN", outsum);

            await graph.EvaluateAsync();

            Assert.That(sumCell.GetValue(), Is.EqualTo(a + b));
        }
    }

    [TestFixture]
    public class AsyncNodeGrap_FullAdderTests : FullAdderTests<AsyncNodeGraph>
    {

    }

    [TestFixture]
    public class SequenceNodeGraph_FullAdderTests : FullAdderTests<SequenceNodeGraph>
    {

    }
}
