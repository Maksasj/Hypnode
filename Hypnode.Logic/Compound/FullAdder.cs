using Hypnode.Core;
using Hypnode.Logic.Gates;
using Hypnode.Runtime;
using Hypnode.System.Common;
using System.Collections;

namespace Hypnode.Logic.Compound;

public class FullAdder(INodeGraph nodeGraph) : CompoundNode(nodeGraph)
{
    private Connection<LogicValue>? _aPort = null;
    private Connection<LogicValue>? _bPort = null;
    private Connection<LogicValue>? _carryIn = null;

    private Connection<LogicValue>? _sum = null;
    private Connection<LogicValue>? _carryOut = null;

    public override INode SetPort(string portName, IConnection connection)
    {
        if (portName == "INA" && connection is Connection<LogicValue> con0) _aPort = con0;
        if (portName == "INB" && connection is Connection<LogicValue> con1) _bPort = con1;
        if (portName == "INC" && connection is Connection<LogicValue> con2) _carryIn = con2;
        if (portName == "OUTSUM" && connection is Connection<LogicValue> con3) _sum = con3;
        if (portName == "OUTC" && connection is Connection<LogicValue> con4) _carryOut = con4;

        return this;
    }

    public override IEnumerator Execute()
    {
        if (_aPort is null)
            throw new InvalidOperationException("Input port A is not set");

        if (_bPort is null)
            throw new InvalidOperationException("Input port B is not set");

        if (_carryIn is null)
            throw new InvalidOperationException("Input port _carryIn is not set");

        while (true)
        {
            if ((_aPort.IsClosed && !_aPort.HasData) ||
                (_bPort.IsClosed && !_bPort.HasData) ||
                (_carryIn.IsClosed && !_carryIn.HasData))
                break;

            if (!_aPort.HasData || !_bPort.HasData || !_carryIn.HasData)
            {
                yield return null;
                continue;
            }

            var a = _aPort.Receive();
            var b = _bPort.Receive();
            var c = _carryIn.Receive();

            var graph = new CoroutineNodeGraph();
            var AtoDemux1 = graph.CreateConnection<LogicValue>();
            var Demux1toXor1 = graph.CreateConnection<LogicValue>();
            var BtoDemux2 = graph.CreateConnection<LogicValue>();
            var Demux2toXor1 = graph.CreateConnection<LogicValue>();
            var CtoDemux3 = graph.CreateConnection<LogicValue>();
            var Demux3toXor2 = graph.CreateConnection<LogicValue>();
            var Xor1toDemux4 = graph.CreateConnection<LogicValue>();
            var Demux4toXor2 = graph.CreateConnection<LogicValue>();
            var Demux4toAnd1 = graph.CreateConnection<LogicValue>();
            var Demux3toAnd1 = graph.CreateConnection<LogicValue>();
            var Demux1toAnd2 = graph.CreateConnection<LogicValue>();
            var Demux2toAnd2 = graph.CreateConnection<LogicValue>();
            var And1toOr = graph.CreateConnection<LogicValue>();
            var And2toOr = graph.CreateConnection<LogicValue>();
            var toCarryOut = graph.CreateConnection<LogicValue>();
            var toSum = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<LogicValue>(a)).SetPort("OUT", AtoDemux1);
            graph.AddNode(new Splitter<LogicValue>()).SetPort("IN", AtoDemux1).SetPort("OUT", Demux1toXor1).SetPort("OUT", Demux1toAnd2);
            graph.AddNode(new PulseValue<LogicValue>(b)).SetPort("OUT", BtoDemux2);
            graph.AddNode(new Splitter<LogicValue>()).SetPort("IN", BtoDemux2).SetPort("OUT", Demux2toXor1).SetPort("OUT", Demux2toAnd2);
            graph.AddNode(new PulseValue<LogicValue>(c)).SetPort("OUT", CtoDemux3);
            graph.AddNode(new Splitter<LogicValue>()).SetPort("IN", CtoDemux3).SetPort("OUT", Demux3toXor2).SetPort("OUT", Demux3toAnd1);
            graph.AddNode(new XorGate()).SetPort("INA", Demux1toXor1).SetPort("INB", Demux2toXor1).SetPort("OUT", Xor1toDemux4);
            graph.AddNode(new XorGate()).SetPort("INA", Demux3toXor2).SetPort("INB", Demux4toXor2).SetPort("OUT", toSum);

            var sumCell = new Register<LogicValue>();
            graph.AddNode(sumCell).SetPort("IN", toSum);

            graph.AddNode(new Splitter<LogicValue>()).SetPort("IN", Xor1toDemux4).SetPort("OUT", Demux4toXor2).SetPort("OUT", Demux4toAnd1);
            graph.AddNode(new AndGate()).SetPort("INA", Demux4toAnd1).SetPort("INB", Demux3toAnd1).SetPort("OUT", And1toOr);
            graph.AddNode(new AndGate()).SetPort("INA", Demux1toAnd2).SetPort("INB", Demux2toAnd2).SetPort("OUT", And2toOr);
            graph.AddNode(new OrGate()).SetPort("INA", And2toOr).SetPort("INB", And1toOr).SetPort("OUT", toCarryOut);

            var carryCell = new Register<LogicValue>();
            graph.AddNode(carryCell).SetPort("IN", toCarryOut);

            yield return graph;

            _sum?.Send(sumCell.GetValue());
            _carryOut?.Send(carryCell.GetValue());
        }

        _sum?.Close();
        _carryOut?.Close();
    }
}
