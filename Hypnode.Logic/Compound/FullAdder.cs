using Hypnode.Core;
using Hypnode.Logic.Gates;
using Hypnode.Runtime;
using Hypnode.System.Common;
using System.Collections;

namespace Hypnode.Logic.Compound;

public class FullAdder(INodeGraph nodeGraph) : CompoundNode(nodeGraph)
{
    public const string InputA = "INA";
    public const string InputB = "INB";
    public const string InputC = "INC";
    public const string OutputSum = "OUTSUM";
    public const string OutputCarry = "OUTC";

    private Connection<LogicValue>? _aPort = null;
    private Connection<LogicValue>? _bPort = null;
    private Connection<LogicValue>? _carryIn = null;
    private Connection<LogicValue>? _sum = null;
    private Connection<LogicValue>? _carryOut = null;

    public override INode SetPort(string portName, IConnection connection)
    {
        if (portName == InputA && connection is Connection<LogicValue> con0) _aPort = con0;
        if (portName == InputB && connection is Connection<LogicValue> con1) _bPort = con1;
        if (portName == InputC && connection is Connection<LogicValue> con2) _carryIn = con2;
        if (portName == OutputSum && connection is Connection<LogicValue> con3) _sum = con3;
        if (portName == OutputCarry && connection is Connection<LogicValue> con4) _carryOut = con4;
        return this;
    }

    public override IEnumerator Execute()
    {
        if (_aPort is null) throw new InvalidOperationException("Input port A is not set");
        if (_bPort is null) throw new InvalidOperationException("Input port B is not set");
        if (_carryIn is null) throw new InvalidOperationException("Input port C is not set");

        while (true)
        {
            if ((_aPort.IsClosed && !_aPort.HasData) || (_bPort.IsClosed && !_bPort.HasData) || (_carryIn.IsClosed && !_carryIn.HasData))
                break;
            if (!_aPort.HasData || !_bPort.HasData || !_carryIn.HasData) { yield return null; continue; }

            var a = _aPort.Receive();
            var b = _bPort.Receive();
            var c = _carryIn.Receive();

            var graph = new CoroutineNodeGraph();
            var aToDemux1 = graph.CreateConnection<LogicValue>();
            var demux1ToXor1 = graph.CreateConnection<LogicValue>();
            var bToDemux2 = graph.CreateConnection<LogicValue>();
            var demux2ToXor1 = graph.CreateConnection<LogicValue>();
            var cToDemux3 = graph.CreateConnection<LogicValue>();
            var demux3ToXor2 = graph.CreateConnection<LogicValue>();
            var xor1ToDemux4 = graph.CreateConnection<LogicValue>();
            var demux4ToXor2 = graph.CreateConnection<LogicValue>();
            var demux4ToAnd1 = graph.CreateConnection<LogicValue>();
            var demux3ToAnd1 = graph.CreateConnection<LogicValue>();
            var demux1ToAnd2 = graph.CreateConnection<LogicValue>();
            var demux2ToAnd2 = graph.CreateConnection<LogicValue>();
            var and1ToOr = graph.CreateConnection<LogicValue>();
            var and2ToOr = graph.CreateConnection<LogicValue>();
            var toCarryOut = graph.CreateConnection<LogicValue>();
            var toSum = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<LogicValue>(a)).SetPort(PulseValue<LogicValue>.Output, aToDemux1);
            graph.AddNode(new Splitter<LogicValue>()).SetPort(Splitter<LogicValue>.Input, aToDemux1).SetPort(Splitter<LogicValue>.Output, demux1ToXor1).SetPort(Splitter<LogicValue>.Output, demux1ToAnd2);
            graph.AddNode(new PulseValue<LogicValue>(b)).SetPort(PulseValue<LogicValue>.Output, bToDemux2);
            graph.AddNode(new Splitter<LogicValue>()).SetPort(Splitter<LogicValue>.Input, bToDemux2).SetPort(Splitter<LogicValue>.Output, demux2ToXor1).SetPort(Splitter<LogicValue>.Output, demux2ToAnd2);
            graph.AddNode(new PulseValue<LogicValue>(c)).SetPort(PulseValue<LogicValue>.Output, cToDemux3);
            graph.AddNode(new Splitter<LogicValue>()).SetPort(Splitter<LogicValue>.Input, cToDemux3).SetPort(Splitter<LogicValue>.Output, demux3ToXor2).SetPort(Splitter<LogicValue>.Output, demux3ToAnd1);
            graph.AddNode(new XorGate()).SetPort(XorGate.InputA, demux1ToXor1).SetPort(XorGate.InputB, demux2ToXor1).SetPort(XorGate.Output, xor1ToDemux4);
            graph.AddNode(new XorGate()).SetPort(XorGate.InputA, demux3ToXor2).SetPort(XorGate.InputB, demux4ToXor2).SetPort(XorGate.Output, toSum);

            var sumCell = new Register<LogicValue>();
            graph.AddNode(sumCell).SetPort(Register<LogicValue>.Input, toSum);

            graph.AddNode(new Splitter<LogicValue>()).SetPort(Splitter<LogicValue>.Input, xor1ToDemux4).SetPort(Splitter<LogicValue>.Output, demux4ToXor2).SetPort(Splitter<LogicValue>.Output, demux4ToAnd1);
            graph.AddNode(new AndGate()).SetPort(AndGate.InputA, demux4ToAnd1).SetPort(AndGate.InputB, demux3ToAnd1).SetPort(AndGate.Output, and1ToOr);
            graph.AddNode(new AndGate()).SetPort(AndGate.InputA, demux1ToAnd2).SetPort(AndGate.InputB, demux2ToAnd2).SetPort(AndGate.Output, and2ToOr);
            graph.AddNode(new OrGate()).SetPort(OrGate.InputA, and2ToOr).SetPort(OrGate.InputB, and1ToOr).SetPort(OrGate.Output, toCarryOut);

            var carryCell = new Register<LogicValue>();
            graph.AddNode(carryCell).SetPort(Register<LogicValue>.Input, toCarryOut);

            yield return graph;

            _sum?.Send(sumCell.GetValue());
            _carryOut?.Send(carryCell.GetValue());
        }

        _sum?.Close();
        _carryOut?.Close();
    }
}
