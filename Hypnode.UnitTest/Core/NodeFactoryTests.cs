using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Types;
using Hypnode.System.Common;
using Hypnode.System.Math;

namespace Hypnode.UnitTests.Core;

[TestFixture]
public class NodeFactoryTests
{
    private static NodeFactory BuildFactory()
    {
        var factory = new NodeFactory();
        factory.Register("pulse", p => new PulseValue(new IntValue(int.Parse(p["value"]))));
        factory.Register("multi", p => new MultiPulseValue(p["values"].Split(',').Select(v => (HypnodeValue)new IntValue(int.Parse(v)))));
        factory.Register("register", () => new Register());
        factory.Register("squarer", () => new Squarer());
        return factory;
    }

    [Test]
    public void TestFactory_Build_EvaluatesCorrectly()
    {
        var def = new GraphDefinition();
        def.AddNode("src", "pulse", ("value", "7"));
        def.AddNode("sq", "squarer");
        def.AddNode("result", "register");
        def.Connect("int", "src", Ports.Output, "sq", Ports.Input);
        def.Connect("int", "sq", Ports.Output, "result", Ports.Input);

        var graph = BuildFactory().Build(def);
        var result = (Register)graph.Nodes[2];

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(49));
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
        def.AddNode("src", "pulse", ("value", "1"));
        def.AddNode("result", "register");
        def.Connect("mystery-type", "src", Ports.Output, "result", Ports.Input);

        Assert.Throws<InvalidOperationException>(() => BuildFactory().Build(def));
    }

    [Test]
    public void TestFactory_RegisterConnectionType_CustomAlias()
    {
        var factory = BuildFactory();
        factory.RegisterConnectionType("int2", g => g.CreateConnection());

        var def = new GraphDefinition();
        def.AddNode("src", "pulse", ("value", "3"));
        def.AddNode("result", "register");
        def.Connect("int2", "src", Ports.Output, "result", Ports.Input);

        var graph = factory.Build(def);
        var result = (Register)graph.Nodes[1];

        graph.Evaluate();

        Assert.That(result.GetValue()!.AsInt(), Is.EqualTo(3));
    }
}
