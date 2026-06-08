using Hypnode.Core;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Moq;

namespace Hypnode.UnitTests.System.Common;

[TestFixture]
public class IfNodeTests
{
    [Test]
    public void TestIf_TruePacket_RoutedToThen()
    {
        var graph   = new CoroutineNodeGraph();
        var connIn  = graph.CreateConnection<int>();
        var connThen = graph.CreateConnection<int>();
        var connElse = graph.CreateConnection<int>();

        graph.AddNode(new PulseValue<int>(4)).SetPort(Ports.Output, connIn);
        graph.AddNode(new IfNode<int>(x => x % 2 == 0))
            .SetPort(Ports.Input,  connIn)
            .SetPort(IfNode<int>.Then, connThen)
            .SetPort(IfNode<int>.Else, connElse);

        var thenResult = graph.AddNode(new Register<int>());
        thenResult.SetPort(Ports.Input, connThen);
        graph.AddNode(new VoidSink<int>()).SetPort(VoidSink<int>.Input, connElse);

        graph.Evaluate();

        Assert.That(thenResult.GetValue(), Is.EqualTo(4));
    }

    [Test]
    public void TestIf_FalsePacket_RoutedToElse()
    {
        var graph    = new CoroutineNodeGraph();
        var connIn   = graph.CreateConnection<int>();
        var connThen = graph.CreateConnection<int>();
        var connElse = graph.CreateConnection<int>();

        graph.AddNode(new PulseValue<int>(3)).SetPort(Ports.Output, connIn);
        graph.AddNode(new IfNode<int>(x => x % 2 == 0))
            .SetPort(Ports.Input,  connIn)
            .SetPort(IfNode<int>.Then, connThen)
            .SetPort(IfNode<int>.Else, connElse);

        graph.AddNode(new VoidSink<int>()).SetPort(VoidSink<int>.Input, connThen);
        var elseResult = graph.AddNode(new Register<int>());
        elseResult.SetPort(Ports.Input, connElse);

        graph.Evaluate();

        Assert.That(elseResult.GetValue(), Is.EqualTo(3));
    }

    [Test]
    public void TestIf_TruePacket_NotSentToElse()
    {
        var graph    = new CoroutineNodeGraph();
        var connIn   = graph.CreateConnection<int>();
        var connThen = new Mock<Connection<int>>();
        var connElse = new Mock<Connection<int>>();

        graph.AddNode(new PulseValue<int>(10)).SetPort(Ports.Output, connIn);
        graph.AddNode(new IfNode<int>(x => x > 5))
            .SetPort(Ports.Input,      connIn)
            .SetPort(IfNode<int>.Then, connThen.Object)
            .SetPort(IfNode<int>.Else, connElse.Object);

        graph.Evaluate();

        connThen.Verify(c => c.Send(10),           Times.Once);
        connElse.Verify(c => c.Send(It.IsAny<int>()), Times.Never);
        connThen.Verify(c => c.Close(), Times.Once);
        connElse.Verify(c => c.Close(), Times.Once);
    }

    [Test]
    public void TestIf_MultiplePackets_SplitCorrectly()
    {
        var graph    = new CoroutineNodeGraph();
        var connIn   = graph.CreateConnection<int>();
        var connThen = graph.CreateConnection<int>();
        var connElse = graph.CreateConnection<int>();

        graph.AddNode(new MultiPulseValue<int>([1, 2, 3, 4, 5, 6])).SetPort(Ports.Output, connIn);
        graph.AddNode(new IfNode<int>(x => x % 2 == 0))
            .SetPort(Ports.Input,      connIn)
            .SetPort(IfNode<int>.Then, connThen)
            .SetPort(IfNode<int>.Else, connElse);

        var evenFold = graph.AddNode(new FoldNode<int, int>(0, (acc, x) => acc + x));
        var oddFold  = graph.AddNode(new FoldNode<int, int>(0, (acc, x) => acc + x));
        evenFold.SetPort(Ports.Input, connThen);
        oddFold.SetPort(Ports.Input,  connElse);

        var evenResult = graph.AddNode(new Register<int>());
        var oddResult  = graph.AddNode(new Register<int>());
        graph.AddConnection<int>(evenFold, Ports.Output, evenResult, Ports.Input);
        graph.AddConnection<int>(oddFold,  Ports.Output, oddResult,  Ports.Input);

        graph.Evaluate();

        Assert.That(evenResult.GetValue(), Is.EqualTo(12));
        Assert.That(oddResult.GetValue(),  Is.EqualTo(9));
    }
}
