using Hypnode.Core;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Hypnode.System.Math;

namespace Hypnode.UnitTests.System.Math
{
    public abstract class SquarerTests<TGraph> where TGraph : INodeGraph, new()
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-2)]
        [TestCase(2)]
        [TestCase(100)]
        [TestCase(4)]
        [TestCase(3)]
        [TestCase(-25)]
        public void TestSquarer_CorrectValue(int value)
        {
            var graph = new TGraph();

            var pulse = graph.AddNode(new PulseValue<int>(value));
            var squarer = graph.AddNode(new Squarer());
            var result = graph.AddNode(new Register<int>());

            graph.AddConnection<int>(pulse, "OUT", squarer, "IN");
            graph.AddConnection<int>(squarer, "OUT", result, "IN");

            graph.Evaluate();

            Assert.That(result.GetValue(), Is.EqualTo(value * value));
        }
    }

    [TestFixture]
    public class CoroutineNodeGraph_SquarerTests : SquarerTests<CoroutineNodeGraph>
    {

    }

    [TestFixture]
    public class SequenceNodeGraph_SquarerTests : SquarerTests<SequenceNodeGraph>
    {

    }
}
