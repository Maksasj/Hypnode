using Hypnode.Core.Types;
using Hypnode.Runtime;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Runtime;

[TestFixture]
public class RuntimeTests
{
    private static string CaptureOutput(Action action)
    {
        var sw = new StringWriter();
        var original = Console.Out;
        Console.SetOut(sw);
        try { action(); }
        finally { Console.SetOut(original); }
        return sw.ToString();
    }

    [Test]
    public void Pulse_Printer_PrintsValue()
    {
        const string xml = """
            <Graph>
              <Nodes>
                <Node id="src" type="pulse">
                  <Parameters>
                    <Parameter name="type">int</Parameter>
                    <Parameter name="value">42</Parameter>
                  </Parameters>
                </Node>
                <Node id="out" type="printer" />
              </Nodes>
              <Connections>
                <Connection type="int">
                  <From node="src" port="OUT" />
                  <To   node="out" port="IN"  />
                </Connection>
              </Connections>
            </Graph>
            """;

        var output = CaptureOutput(() => new HypnodeRuntime().Run(xml));
        Assert.That(output.Trim(), Is.EqualTo("42"));
    }

    [Test]
    public void Pulse_Squarer_Printer_Prints49()
    {
        const string xml = """
            <Graph>
              <Nodes>
                <Node id="src" type="pulse">
                  <Parameters>
                    <Parameter name="type">int</Parameter>
                    <Parameter name="value">7</Parameter>
                  </Parameters>
                </Node>
                <Node id="sq"  type="squarer"  />
                <Node id="out" type="printer"  />
              </Nodes>
              <Connections>
                <Connection type="int">
                  <From node="src" port="OUT" />
                  <To   node="sq"  port="IN"  />
                </Connection>
                <Connection type="int">
                  <From node="sq"  port="OUT" />
                  <To   node="out" port="IN"  />
                </Connection>
              </Connections>
            </Graph>
            """;

        var output = CaptureOutput(() => new HypnodeRuntime().Run(xml));
        Assert.That(output.Trim(), Is.EqualTo("49"));
    }

    [Test]
    public void Build_PulseToRegister_ValueAccessible()
    {
        const string xml = """
            <Graph>
              <Nodes>
                <Node id="src"    type="pulse">
                  <Parameters>
                    <Parameter name="type">int</Parameter>
                    <Parameter name="value">99</Parameter>
                  </Parameters>
                </Node>
                <Node id="result" type="register" />
              </Nodes>
              <Connections>
                <Connection type="int">
                  <From node="src"    port="OUT" />
                  <To   node="result" port="IN"  />
                </Connection>
              </Connections>
            </Graph>
            """;

        var runtime = new HypnodeRuntime();
        var graph = runtime.Build(xml);
        graph.Evaluate();

        var register = (Register)graph.Nodes.Last();
        Assert.That(register.GetValue()!.AsInt(), Is.EqualTo(99));
    }

    [Test]
    public void UnknownNodeType_Throws()
    {
        const string xml = """
            <Graph>
              <Nodes>
                <Node id="n" type="does-not-exist" />
              </Nodes>
              <Connections />
            </Graph>
            """;

        Assert.Throws<InvalidOperationException>(() => new HypnodeRuntime().Run(xml));
    }
}
