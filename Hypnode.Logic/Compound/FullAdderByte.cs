using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using Hypnode.Logic.Utils;
using Hypnode.System.Common;
using System.Collections;

namespace Hypnode.Logic.Compound;

[HypnodeNode("full-adder-byte", "8-bit full adder (INA, INB → OUTSUM)")]
public class FullAdderByte : INode
{
    public const string InputA    = "INA";
    public const string InputB    = "INB";
    public const string OutputSum = "OUTSUM";

    private Connection<HypnodeValue>? _aPort;
    private Connection<HypnodeValue>? _bPort;
    private Connection<HypnodeValue>? _sum;

    public INode SetPort(string portName, IConnection connection)
    {
        var result = portName switch
        {
            InputA    => NodeExtensions.TryAttach(ref _aPort, connection),
            InputB    => NodeExtensions.TryAttach(ref _bPort, connection),
            OutputSum => NodeExtensions.TryAttach(ref _sum, connection),
            _ => throw new InvalidOperationException($"Unknown port '{portName}'"),
        };

        if (!result)
            throw new InvalidOperationException($"Port '{portName}' is already set or type mismatch");

        return this;
    }

    public IEnumerator Execute()
    {
        if (_aPort is null) throw new InvalidOperationException("Input port A is not set");
        if (_bPort is null) throw new InvalidOperationException("Input port B is not set");

        while (true)
        {
            if ((_aPort.IsClosed && !_aPort.HasData) || (_bPort.IsClosed && !_bPort.HasData))
                break;

            if (!_aPort.HasData || !_bPort.HasData) { yield return null; continue; }

            var a = _aPort.Receive().AsByte();
            var b = _bPort.Receive().AsByte();

            var graph = new CoroutineNodeGraph();
            var aIn = graph.CreateConnection();
            var bIn = graph.CreateConnection();
            var cIn = graph.CreateConnection();

            graph.AddNode(new PulseValue(new ByteValue(a))).SetPort(Ports.Output, aIn);
            graph.AddNode(new PulseValue(new ByteValue(b))).SetPort(Ports.Output, bIn);
            graph.AddNode(new PulseValue(new LogicPacket(LogicValue.False))).SetPort(Ports.Output, cIn);

            var aDemux = graph.AddNode(new ByteSplitterIn()).SetPort(Ports.Input, aIn);
            var bDemux = graph.AddNode(new ByteSplitterIn()).SetPort(Ports.Input, bIn);

            Connection<HypnodeValue>? carry = null;
            var sumWires = new Connection<HypnodeValue>[8];

            for (int i = 0; i < 8; ++i)
            {
                var adder = graph.AddNode(new FullAdder());
                var aWire = graph.CreateConnection();
                var bWire = graph.CreateConnection();

                aDemux.SetPort(i.ToString(), aWire);
                bDemux.SetPort(i.ToString(), bWire);

                adder.SetPort(FullAdder.InputA, aWire);
                adder.SetPort(FullAdder.InputB, bWire);
                adder.SetPort(FullAdder.InputC, i == 0 ? cIn : carry!);

                carry  = graph.CreateConnection();
                var sumWire = graph.CreateConnection();
                sumWires[i] = sumWire;

                adder.SetPort(FullAdder.OutputSum, sumWire);
                adder.SetPort(FullAdder.OutputCarry, carry);
            }

            var sumMux    = graph.AddNode(new ByteSplitterOut());
            var resultWire = graph.CreateConnection();

            for (int i = 0; i < 8; ++i)
                sumMux.SetPort(i.ToString(), sumWires[i]);

            sumMux.SetPort(Ports.Output, resultWire);

            var result = new Register();
            graph.AddNode(result).SetPort(Ports.Input, resultWire);

            graph.AddNode(new VoidSink()).SetPort(VoidSink.Input, carry!);

            yield return graph;

            _sum?.Send(result.GetValue()!);
        }

        _sum?.Close();
    }
}
