using Hypnode.Core;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.Logic.Utils;

[HypnodeNode("byte-splitter-in", "Splits a byte into 8 LogicValue bits (IN → 0..7)")]
public class ByteSplitterIn : INode
{
    private Connection<HypnodeValue>? _inputPort;
    private readonly Connection<HypnodeValue>?[] _outputPorts = new Connection<HypnodeValue>?[8];

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input)
            NodeExtensions.TryAttach(ref _inputPort, connection);

        if (int.TryParse(portName, out int idx) && idx >= 0 && idx < 8
            && connection is Connection<HypnodeValue> connBit)
            _outputPorts[idx] = connBit;

        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }

            var packet = _inputPort.Receive().AsByte();
            for (int i = 0; i < 8; i++)
            {
                var bit = (packet & (1 << i)) != 0 ? LogicValue.True : LogicValue.False;
                _outputPorts[i]?.Send(new LogicPacket(bit));
            }
        }

        foreach (var conn in _outputPorts) conn?.Close();
    }
}
