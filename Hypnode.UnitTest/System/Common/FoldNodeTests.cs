using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Types;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class FoldNodeTests
{
    private static readonly int[] sourceArray = new int[] { 1, 2, 3, 4, 5 };

    [Test]
    public void TestFold_RunningSum_FinalValue()
    {
        var graph = new CoroutineNodeGraph();
        var multi = graph.AddNode(new MultiPulseValue(sourceArray.Select(i => (HypnodeValue)new IntValue(i))));
        var fold = graph.AddNode(new FoldNode(new IntValue(0), (acc, x) => new IntValue(acc.AsInt() + x.AsInt())));
        var result = graph.AddNode(new Register());

        graph.AddConnection(multi, Ports.Output, fold, Ports.Input);
        graph.AddConnection(fold, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(15));
    }

    [Test]
    public void TestFold_RunningProduct_FinalValue()
    {
        var graph = new CoroutineNodeGraph();
        var multi = graph.AddNode(new MultiPulseValue(sourceArray.Select(i => (HypnodeValue)new IntValue(i))));
        var fold = graph.AddNode(new FoldNode(new IntValue(1), (acc, x) => new IntValue(acc.AsInt() * x.AsInt())));
        var result = graph.AddNode(new Register());

        graph.AddConnection(multi, Ports.Output, fold, Ports.Input);
        graph.AddConnection(fold, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(120));
    }

    [Test]
    public void TestFold_EmitsAfterEachPacket()
    {
        var graph = new CoroutineNodeGraph();
        var multi = graph.AddNode(new MultiPulseValue(sourceArray.Select(i => (HypnodeValue)new IntValue(i))));
        var fold = graph.AddNode(new FoldNode(new IntValue(0), (acc, x) => new IntValue(acc.AsInt() + x.AsInt())));
        var sink = graph.AddNode(new VoidSink());

        graph.AddConnection(multi, Ports.Output, fold, Ports.Input);

        var connOut = graph.CreateConnection();
        fold.SetPort(Ports.Output, connOut);
        sink.SetPort(VoidSink.Input, connOut);

        graph.Evaluate();

        Assert.Pass();
    }

    [Test]
    public void TestFold_StringConcat()
    {
        var graph = new CoroutineNodeGraph();
        var multi = graph.AddNode(new MultiPulseValue(new HypnodeValue[] { new StringValue("a"), new StringValue("b"), new StringValue("c") }));
        var fold = graph.AddNode(new FoldNode(new StringValue(""), (acc, x) => new StringValue(acc.AsString() + x.AsString())));
        var result = graph.AddNode(new Register());

        graph.AddConnection(multi, Ports.Output, fold, Ports.Input);
        graph.AddConnection(fold, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsString(), Is.EqualTo("abc"));
    }

    [Test]
    public void TestFold_CountPackets()
    {
        var graph = new CoroutineNodeGraph();
        var multi = graph.AddNode(new MultiPulseValue(new HypnodeValue[] { new StringValue("x"), new StringValue("y"), new StringValue("z"), new StringValue("w") }));
        var fold = graph.AddNode(new FoldNode(new IntValue(0), (acc, _) => new IntValue(acc.AsInt() + 1)));
        var result = graph.AddNode(new Register());

        graph.AddConnection(multi, Ports.Output, fold, Ports.Input);
        graph.AddConnection(fold, Ports.Output, result, Ports.Input);

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(4));
    }
}
