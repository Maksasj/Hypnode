using Hypnode.Core;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.Logic.Utils;

[HypnodeNode("byte-splitter-out", "Assembles 8 LogicValue bits into a byte (0..7 → OUT)")]
public class ByteSplitterOut : INode
{
    private readonly Connection<HypnodeValue>?[] _inputPorts = new Connection<HypnodeValue>?[8];
    private Connection<HypnodeValue>? _outputPort;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output)
            NodeExtensions.TryAttach(ref _outputPort, connection);

        if (int.TryParse(portName, out int idx) && idx >= 0 && idx < 8
            && connection is Connection<HypnodeValue> connBit)
            _inputPorts[idx] = connBit;

        return this;
    }

    public IEnumerator Execute()
    {
        for (int i = 0; i < 8; i++)
            if (_inputPorts[i] is null)
                throw new InvalidOperationException($"Input port {i} is not set");

        while (true)
        {
            if (Enumerable.Range(0, 8).Any(i => _inputPorts[i]!.IsClosed && !_inputPorts[i]!.HasData))
                break;

            if (!Enumerable.Range(0, 8).All(i => _inputPorts[i]!.HasData)) { yield return null; continue; }

            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                if (_inputPorts[i]!.Receive().AsLogic() == LogicValue.True)
                    result |= (byte)(1 << i);
            }

            _outputPort?.Send(new ByteValue(result));
        }

        _outputPort?.Close();
    }
}
