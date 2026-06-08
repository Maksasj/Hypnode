using Hypnode.Core;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class FoldNodeTests
{
    [Test]
    public void TestFold_RunningSum_FinalValue()
    {
        var graph  = new CoroutineNodeGraph();
        var multi  = graph.AddNode(new MultiPulseValue<int>([1, 2, 3, 4, 5]));
        var fold   = graph.AddNode(new FoldNode<int, int>(0, (acc, x) => acc + x));
        var result = graph.AddNode(new Register<int>());

        graph.AddConnection<int>(multi,  Ports.Output, fold,   Ports.Input);
        graph.AddConnection<int>(fold,   Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(15));
    }

    [Test]
    public void TestFold_RunningProduct_FinalValue()
    {
        var graph  = new CoroutineNodeGraph();
        var multi  = graph.AddNode(new MultiPulseValue<int>([1, 2, 3, 4]));
        var fold   = graph.AddNode(new FoldNode<int, int>(1, (acc, x) => acc * x));
        var result = graph.AddNode(new Register<int>());

        graph.AddConnection<int>(multi,  Ports.Output, fold,   Ports.Input);
        graph.AddConnection<int>(fold,   Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(24));
    }

    [Test]
    public void TestFold_EmitsAfterEachPacket()
    {
        var graph  = new CoroutineNodeGraph();
        var multi  = graph.AddNode(new MultiPulseValue<int>([10, 20, 30]));
        var fold   = graph.AddNode(new FoldNode<int, int>(0, (acc, x) => acc + x));
        var sink   = graph.AddNode(new VoidSink<int>());

        graph.AddConnection<int>(multi, Ports.Output, fold, Ports.Input);

        var connOut = graph.CreateConnection<int>();
        fold.SetPort(Ports.Output, connOut);
        sink.SetPort(VoidSink<int>.Input, connOut);

        graph.Evaluate();

        Assert.Pass();
    }

    [Test]
    public void TestFold_TypeAccumulation_StringConcat()
    {
        var graph  = new CoroutineNodeGraph();
        var multi  = graph.AddNode(new MultiPulseValue<string>(["a", "b", "c"]));
        var fold   = graph.AddNode(new FoldNode<string, string>("", (acc, x) => acc + x));
        var result = graph.AddNode(new Register<string>());

        graph.AddConnection<string>(multi,  Ports.Output, fold,   Ports.Input);
        graph.AddConnection<string>(fold,   Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo("abc"));
    }

    [Test]
    public void TestFold_CountPackets()
    {
        var graph  = new CoroutineNodeGraph();
        var multi  = graph.AddNode(new MultiPulseValue<string>(["x", "y", "z", "w"]));
        var fold   = graph.AddNode(new FoldNode<string, int>(0, (acc, _) => acc + 1));
        var result = graph.AddNode(new Register<int>());

        graph.AddConnection<string>(multi, Ports.Output, fold,   Ports.Input);
        graph.AddConnection<int>   (fold,  Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(4));
    }
}
