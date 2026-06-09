using Hypnode.Core;
using Hypnode.Core.Graph;
using Hypnode.Core.Modules;
using Hypnode.Logic.Gates;
using Hypnode.System.Common;
using System.Collections;

namespace Hypnode.Logic.Compound;

[HypnodeNode("full-adder", "1-bit full adder (INA, INB, INC → OUTSUM, OUTC)")]
public class FullAdder : INode
{
    public const string InputA = "INA";
    public const string InputB = "INB";
    public const string InputC = "INC";
    public const string OutputSum = "OUTSUM";
    public const string OutputCarry = "OUTC";

    private Connection? _aPort;
    private Connection? _bPort;
    private Connection? _carryIn;
    private Connection? _sum;
    private Connection? _carryOut;

    public INode SetPort(string portName, IConnection connection)
    {
        var result = portName switch
        {
            InputA => NodeExtensions.TryAttach(ref _aPort, connection),
            InputB => NodeExtensions.TryAttach(ref _bPort, connection),
            InputC => NodeExtensions.TryAttach(ref _carryIn, connection),
            OutputSum => NodeExtensions.TryAttach(ref _sum, connection),
            OutputCarry => NodeExtensions.TryAttach(ref _carryOut, connection),
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
        if (_carryIn is null) throw new InvalidOperationException("Input port C is not set");

        while (true)
        {
            if ((_aPort.IsClosed && !_aPort.HasData) ||
                (_bPort.IsClosed && !_bPort.HasData) ||
                (_carryIn.IsClosed && !_carryIn.HasData))
                break;

            if (!_aPort.HasData || !_bPort.HasData || !_carryIn.HasData) { yield return null; continue; }

            var a = _aPort.Receive().AsLogic();
            var b = _bPort.Receive().AsLogic();
            var c = _carryIn.Receive().AsLogic();

            var graph = new CoroutineNodeGraph();
            var aToDemux = graph.CreateConnection();
            var bToDemux = graph.CreateConnection();
            var cToDemux = graph.CreateConnection();
            var demux1ToXor1 = graph.CreateConnection();
            var demux1ToAnd2 = graph.CreateConnection();
            var demux2ToXor1 = graph.CreateConnection();
            var demux2ToAnd2 = graph.CreateConnection();
            var demux3ToXor2 = graph.CreateConnection();
            var demux3ToAnd1 = graph.CreateConnection();
            var xor1ToDemux4 = graph.CreateConnection();
            var demux4ToXor2 = graph.CreateConnection();
            var demux4ToAnd1 = graph.CreateConnection();
            var and1ToOr = graph.CreateConnection();
            var and2ToOr = graph.CreateConnection();
            var toSum = graph.CreateConnection();
            var toCarryOut = graph.CreateConnection();

            graph.AddNode(new PulseValue(new LogicPacket(a))).SetPort(Ports.Output, aToDemux);
            graph.AddNode(new Splitter()).SetPort(Ports.Input, aToDemux).SetPort(Ports.Output, demux1ToXor1).SetPort(Ports.Output, demux1ToAnd2);
            graph.AddNode(new PulseValue(new LogicPacket(b))).SetPort(Ports.Output, bToDemux);
            graph.AddNode(new Splitter()).SetPort(Ports.Input, bToDemux).SetPort(Ports.Output, demux2ToXor1).SetPort(Ports.Output, demux2ToAnd2);
            graph.AddNode(new PulseValue(new LogicPacket(c))).SetPort(Ports.Output, cToDemux);
            graph.AddNode(new Splitter()).SetPort(Ports.Input, cToDemux).SetPort(Ports.Output, demux3ToXor2).SetPort(Ports.Output, demux3ToAnd1);
            graph.AddNode(new XorGate()).SetPort(BinaryLogicGate.InputA, demux1ToXor1).SetPort(BinaryLogicGate.InputB, demux2ToXor1).SetPort(BinaryLogicGate.Output, xor1ToDemux4);
            graph.AddNode(new XorGate()).SetPort(BinaryLogicGate.InputA, demux3ToXor2).SetPort(BinaryLogicGate.InputB, demux4ToXor2).SetPort(BinaryLogicGate.Output, toSum);
            graph.AddNode(new Splitter()).SetPort(Ports.Input, xor1ToDemux4).SetPort(Ports.Output, demux4ToXor2).SetPort(Ports.Output, demux4ToAnd1);
            graph.AddNode(new AndGate()).SetPort(BinaryLogicGate.InputA, demux4ToAnd1).SetPort(BinaryLogicGate.InputB, demux3ToAnd1).SetPort(BinaryLogicGate.Output, and1ToOr);
            graph.AddNode(new AndGate()).SetPort(BinaryLogicGate.InputA, demux1ToAnd2).SetPort(BinaryLogicGate.InputB, demux2ToAnd2).SetPort(BinaryLogicGate.Output, and2ToOr);
            graph.AddNode(new OrGate()).SetPort(BinaryLogicGate.InputA, and2ToOr).SetPort(BinaryLogicGate.InputB, and1ToOr).SetPort(BinaryLogicGate.Output, toCarryOut);

            var sumCell = new Register();
            var carryCell = new Register();
            graph.AddNode(sumCell).SetPort(Ports.Input, toSum);
            graph.AddNode(carryCell).SetPort(Ports.Input, toCarryOut);

            yield return graph;

            _sum?.Send(sumCell.GetValue()!);
            _carryOut?.Send(carryCell.GetValue()!);
        }

        _sum?.Close();
        _carryOut?.Close();
    }
}
