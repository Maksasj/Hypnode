using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.System.Common;
using Hypnode.System.Math;

namespace Hypnode.UnitTests.Core;

[TestFixture]
public class NodeFactoryTests
{
    private NodeFactory BuildFactory()
    {
        var factory = new NodeFactory();
        factory.Register("pulse-int", p => new PulseValue<int>(int.Parse(p["value"])));
        factory.Register("multi-int", p => new MultiPulseValue<int>(p["values"].Split(',').Select(int.Parse)));
        factory.Register("register-int", () => new Register<int>());
        factory.Register("squarer", () => new Squarer());
        factory.Register("fold-sum", () => new FoldNode<int, int>(0, (acc, x) => acc + x));
        return factory;
    }

    [Test]
    public void TestFactory_Build_EvaluatesCorrectly()
    {
        var def = new GraphDefinition();
        def.AddNode("src", "pulse-int", ("value", "7"));
        def.AddNode("sq", "squarer");
        def.AddNode("result", "register-int");
        def.Connect("int", "src", Ports.Output, "sq", Ports.Input);
        def.Connect("int", "sq", Ports.Output, "result", Ports.Input);

        var graph = BuildFactory().Build(def);
        var result = (Register<int>)graph.Nodes[2];

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(49));
    }

    [Test]
    public void TestFactory_UnknownNodeType_Throws()
    {
        var def = new GraphDefinition();
        def.AddNode("n0", "does-not-exist");

        Assert.Throws<InvalidOperationException>(() => BuildFactory().Build(def));
    }

    [Test]
    public void TestFactory_UnknownConnectionType_Throws()
    {
        var def = new GraphDefinition();
        def.AddNode("src", "pulse-int", ("value", "1"));
        def.AddNode("result", "register-int");
        def.Connect("mystery-type", "src", Ports.Output, "result", Ports.Input);

        Assert.Throws<InvalidOperationException>(() => BuildFactory().Build(def));
    }

    [Test]
    public void TestFactory_MultiplePackets_FoldSum()
    {
        var def = new GraphDefinition();
        def.AddNode("src", "multi-int", ("values", "1,2,3,4,5"));
        def.AddNode("fold", "fold-sum");
        def.AddNode("result", "register-int");
        def.Connect("int", "src", Ports.Output, "fold", Ports.Input);
        def.Connect("int", "fold", Ports.Output, "result", Ports.Input);

        var graph = BuildFactory().Build(def);
        var result = (Register<int>)graph.Nodes[2];

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(15));
    }

    [Test]
    public void TestFactory_RegisterConnectionType_CustomType()
    {
        var factory = BuildFactory();
        factory.RegisterConnectionType("int2", g => g.CreateConnection<int>());

        var def = new GraphDefinition();
        def.AddNode("src", "pulse-int", ("value", "3"));
        def.AddNode("result", "register-int");
        def.Connect("int2", "src", Ports.Output, "result", Ports.Input);

        var graph = factory.Build(def);
        var result = (Register<int>)graph.Nodes[1];

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(3));
    }
}
