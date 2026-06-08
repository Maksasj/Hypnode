using Hypnode.Core;
using Hypnode.Logic.Utils;
using Hypnode.Runtime;
using Hypnode.System.Common;
using System.Collections;

namespace Hypnode.Logic.Compound;

public class FullAdderByte(INodeGraph nodeGraph) : ICompoundNode(nodeGraph)
{
    private Connection<byte>? aPort = null;
    private Connection<byte>? bPort = null;
    private Connection<byte>? sum = null;

    public override INode SetPort(string portName, IConnection connection)
    {
        if (portName == "INA" && connection is Connection<byte> conn0) aPort = conn0;
        if (portName == "INB" && connection is Connection<byte> conn1) bPort = conn1;
        if (portName == "OUTSUM" && connection is Connection<byte> conn2) sum = conn2;

        return this;
    }

    public override IEnumerator Execute()
    {
        if (aPort is null)
            throw new InvalidOperationException("Input port A is not set");

        if (bPort is null)
            throw new InvalidOperationException("Input port B is not set");

        while (true)
        {
            if ((aPort.IsClosed && !aPort.HasData) ||
                (bPort.IsClosed && !bPort.HasData))
                break;

            if (!aPort.HasData || !bPort.HasData)
            {
                yield return null;
                continue;
            }

            var a = aPort.Receive();
            var b = bPort.Receive();

            var graph = new CoroutineNodeGraph();
            var aIn = graph.CreateConnection<byte>();
            var bIn = graph.CreateConnection<byte>();
            var cIn = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<byte>(a)).SetPort("OUT", aIn);
            graph.AddNode(new PulseValue<byte>(b)).SetPort("OUT", bIn);
            graph.AddNode(new PulseValue<LogicValue>(LogicValue.False)).SetPort("OUT", cIn);

            var aDemux = graph.AddNode(new ByteSplitterIn()).SetPort("IN", aIn);
            var bDemux = graph.AddNode(new ByteSplitterIn()).SetPort("IN", bIn);

            Connection<LogicValue>? carry = null;
            var sumWires = new Connection<LogicValue>[8];

            for (int i = 0; i < 8; ++i)
            {
                var adder = graph.AddNode(new FullAdder(new CoroutineNodeGraph()));

                var aWire = graph.CreateConnection<LogicValue>();
                var bWire = graph.CreateConnection<LogicValue>();

                aDemux.SetPort(i.ToString(), aWire);
                bDemux.SetPort(i.ToString(), bWire);

                adder.SetPort("INA", aWire);
                adder.SetPort("INB", bWire);
                adder.SetPort("INC", i == 0 ? cIn : carry!);

                carry = graph.CreateConnection<LogicValue>();
                var sumWire = graph.CreateConnection<LogicValue>();
                sumWires[i] = sumWire;

                adder.SetPort("OUTSUM", sumWire);
                adder.SetPort("OUTC", carry);
            }

            var sumMux = graph.AddNode(new ByteSplitterOut());
            var resultWire = graph.CreateConnection<byte>();

            for (int i = 0; i < 8; ++i)
                sumMux.SetPort(i.ToString(), sumWires[i]);

            sumMux.SetPort("OUT", resultWire);

            var result = new Register<byte>();
            graph.AddNode(result).SetPort("IN", resultWire);

            graph.AddNode(new VoidSink<LogicValue>()).SetPort("_", carry!);

            yield return graph;

            sum?.Send(result.GetValue());
        }

        sum?.Close();
    }
}
