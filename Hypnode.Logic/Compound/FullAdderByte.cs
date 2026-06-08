using Hypnode.Core;
using Hypnode.Logic.Utils;
using Hypnode.Runtime;
using Hypnode.System.Common;
using System.Collections;

namespace Hypnode.Logic.Compound;

public class FullAdderByte(INodeGraph nodeGraph) : CompoundNode(nodeGraph)
{
    public const string InputA = "INA";
    public const string InputB = "INB";
    public const string OutputSum = "OUTSUM";

    private Connection<byte>? _aPort = null;
    private Connection<byte>? _bPort = null;
    private Connection<byte>? _sum = null;

    public override INode SetPort(string portName, IConnection connection)
    {
        if (portName == InputA && connection is Connection<byte> conn0) _aPort = conn0;
        if (portName == InputB && connection is Connection<byte> conn1) _bPort = conn1;
        if (portName == OutputSum && connection is Connection<byte> conn2) _sum = conn2;
        return this;
    }

    public override IEnumerator Execute()
    {
        if (_aPort is null) throw new InvalidOperationException("Input port A is not set");
        if (_bPort is null) throw new InvalidOperationException("Input port B is not set");

        while (true)
        {
            if ((_aPort.IsClosed && !_aPort.HasData) || (_bPort.IsClosed && !_bPort.HasData))
                break;
            if (!_aPort.HasData || !_bPort.HasData) { yield return null; continue; }

            var a = _aPort.Receive();
            var b = _bPort.Receive();

            var graph = new CoroutineNodeGraph();
            var aIn = graph.CreateConnection<byte>();
            var bIn = graph.CreateConnection<byte>();
            var cIn = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<byte>(a)).SetPort(PulseValue<byte>.Output, aIn);
            graph.AddNode(new PulseValue<byte>(b)).SetPort(PulseValue<byte>.Output, bIn);
            graph.AddNode(new PulseValue<LogicValue>(LogicValue.False)).SetPort(PulseValue<LogicValue>.Output, cIn);

            var aDemux = graph.AddNode(new ByteSplitterIn()).SetPort(ByteSplitterIn.Input, aIn);
            var bDemux = graph.AddNode(new ByteSplitterIn()).SetPort(ByteSplitterIn.Input, bIn);

            Connection<LogicValue>? carry = null;
            var sumWires = new Connection<LogicValue>[8];

            for (int i = 0; i < 8; ++i)
            {
                var adder = graph.AddNode(new FullAdder(new CoroutineNodeGraph()));
                var aWire = graph.CreateConnection<LogicValue>();
                var bWire = graph.CreateConnection<LogicValue>();

                aDemux.SetPort(i.ToString(), aWire);
                bDemux.SetPort(i.ToString(), bWire);

                adder.SetPort(FullAdder.InputA, aWire);
                adder.SetPort(FullAdder.InputB, bWire);
                adder.SetPort(FullAdder.InputC, i == 0 ? cIn : carry!);

                carry = graph.CreateConnection<LogicValue>();
                var sumWire = graph.CreateConnection<LogicValue>();
                sumWires[i] = sumWire;

                adder.SetPort(FullAdder.OutputSum, sumWire);
                adder.SetPort(FullAdder.OutputCarry, carry);
            }

            var sumMux = graph.AddNode(new ByteSplitterOut());
            var resultWire = graph.CreateConnection<byte>();

            for (int i = 0; i < 8; ++i)
                sumMux.SetPort(i.ToString(), sumWires[i]);

            sumMux.SetPort(ByteSplitterOut.Output, resultWire);

            var result = new Register<byte>();
            graph.AddNode(result).SetPort(Register<byte>.Input, resultWire);

            graph.AddNode(new VoidSink<LogicValue>()).SetPort(VoidSink<LogicValue>.Input, carry!);

            yield return graph;

            _sum?.Send(result.GetValue());
        }

        _sum?.Close();
    }
}
