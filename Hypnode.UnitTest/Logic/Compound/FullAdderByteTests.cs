using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Logic.Compound;
using Hypnode.System.Common;

namespace Hypnode.UnitTests.Logic.Compound;

[TestFixture]
public class FullAdderByteTests
{
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
    public void TestFullAdderByte_CorrectValues(byte a, byte b)
    {
        var graph = new CoroutineNodeGraph();
        var ain = graph.CreateConnection<byte>();
        var bin = graph.CreateConnection<byte>();
        var outSum = graph.CreateConnection<byte>();

        graph.AddNode(new PulseValue<byte>(a)).SetPort(Ports.Output, ain);
        graph.AddNode(new PulseValue<byte>(b)).SetPort(Ports.Output, bin);

        graph.AddNode(new FullAdderByte())
            .SetPort(FullAdderByte.InputA, ain)
            .SetPort(FullAdderByte.InputB, bin)
            .SetPort(FullAdderByte.OutputSum, outSum);

        var sumCell = new Register<byte>();
        graph.AddNode(sumCell).SetPort(Ports.Input, outSum);

        graph.Evaluate();

        Assert.That(sumCell.GetValue(), Is.EqualTo((byte)(a + b)));
    }
}
