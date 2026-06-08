using Hypnode.Core;
using Hypnode.System.Common;
using Hypnode.System.Math;

namespace Hypnode.UnitTests.Core;

[TestFixture]
public class GraphSerializerTests
{
    [Test]
    public void TestSerialize_ProducesValidXml()
    {
        var def = new GraphDefinition();
        def.AddNode("src",    "pulse-int",    ("value", "5"));
        def.AddNode("result", "register-int");
        def.Connect("int", "src", Ports.Output, "result", Ports.Input);

        var xml = GraphSerializer.Serialize(def);

        Assert.That(xml, Does.Contain("<Graph>"));
        Assert.That(xml, Does.Contain("pulse-int"));
        Assert.That(xml, Does.Contain("register-int"));
        Assert.That(xml, Does.Contain("<Parameter name=\"value\">5</Parameter>"));
    }

    [Test]
    public void TestDeserialize_RestoresNodes()
    {
        var def = new GraphDefinition();
        def.AddNode("src",    "pulse-int",    ("value", "42"));
        def.AddNode("result", "register-int");
        def.Connect("int", "src", Ports.Output, "result", Ports.Input);

        var xml     = GraphSerializer.Serialize(def);
        var loaded  = GraphSerializer.Deserialize(xml);

        Assert.That(loaded.Nodes.Count,       Is.EqualTo(2));
        Assert.That(loaded.Nodes[0].Id,       Is.EqualTo("src"));
        Assert.That(loaded.Nodes[0].TypeName, Is.EqualTo("pulse-int"));
        Assert.That(loaded.Nodes[0].Parameters["value"], Is.EqualTo("42"));
        Assert.That(loaded.Nodes[1].Id,       Is.EqualTo("result"));
    }

    [Test]
    public void TestDeserialize_RestoresConnections()
    {
        var def = new GraphDefinition();
        def.AddNode("src",    "pulse-int",    ("value", "1"));
        def.AddNode("result", "register-int");
        def.Connect("int", "src", Ports.Output, "result", Ports.Input);

        var loaded = GraphSerializer.Deserialize(GraphSerializer.Serialize(def));

        Assert.That(loaded.Connections.Count,          Is.EqualTo(1));
        Assert.That(loaded.Connections[0].TypeAlias,   Is.EqualTo("int"));
        Assert.That(loaded.Connections[0].FromNodeId,  Is.EqualTo("src"));
        Assert.That(loaded.Connections[0].FromPort,    Is.EqualTo(Ports.Output));
        Assert.That(loaded.Connections[0].ToNodeId,    Is.EqualTo("result"));
        Assert.That(loaded.Connections[0].ToPort,      Is.EqualTo(Ports.Input));
    }

    [Test]
    public void TestRoundTrip_BuildAndEvaluate()
    {
        var factory = new NodeFactory();
        factory.Register("pulse-int",    p  => new PulseValue<int>(int.Parse(p["value"])));
        factory.Register("register-int", () => new Register<int>());
        factory.Register("squarer",      () => new Squarer());

        var def = new GraphDefinition();
        def.AddNode("src",    "pulse-int",    ("value", "9"));
        def.AddNode("sq",     "squarer");
        def.AddNode("result", "register-int");
        def.Connect("int", "src", Ports.Output, "sq",     Ports.Input);
        def.Connect("int", "sq",  Ports.Output, "result", Ports.Input);

        var xml    = GraphSerializer.Serialize(def);
        var loaded = GraphSerializer.Deserialize(xml);
        var graph  = factory.Build(loaded);
        var result = (Register<int>)graph.Nodes[2];

        graph.Evaluate();

        Assert.That(result.GetValue(), Is.EqualTo(81));
    }

    [Test]
    public void TestDeserialize_NodeWithNoParameters_HasEmptyDict()
    {
        var def = new GraphDefinition();
        def.AddNode("sq", "squarer");

        var loaded = GraphSerializer.Deserialize(GraphSerializer.Serialize(def));

        Assert.That(loaded.Nodes[0].Parameters.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestDeserialize_MultipleParameters_AllRestored()
    {
        var def = new GraphDefinition();
        def.AddNode("n0", "some-node", ("a", "1"), ("b", "2"), ("c", "hello"));

        var loaded = GraphSerializer.Deserialize(GraphSerializer.Serialize(def));

        Assert.That(loaded.Nodes[0].Parameters["a"], Is.EqualTo("1"));
        Assert.That(loaded.Nodes[0].Parameters["b"], Is.EqualTo("2"));
        Assert.That(loaded.Nodes[0].Parameters["c"], Is.EqualTo("hello"));
    }
}
