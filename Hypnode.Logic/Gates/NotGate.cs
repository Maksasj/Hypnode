using Hypnode.Core;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.Logic.Gates;

[HypnodeNode("not-gate", "Logical NOT of one LogicValue input")]
public class NotGate : INode
{
    private Connection<HypnodeValue>? _inputPort;
    private Connection<HypnodeValue>? _outputPort;

    public INode SetPort(string portName, IConnection connection)
    {
        var result = portName switch
        {
            Ports.Input  => NodeExtensions.TryAttach(ref _inputPort, connection),
            Ports.Output => NodeExtensions.TryAttach(ref _outputPort, connection),
            _ => throw new InvalidOperationException($"Unknown port '{portName}'"),
        };

        if (!result)
            throw new InvalidOperationException($"Port '{portName}' is already set or type mismatch");

        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }
            var v = _inputPort.Receive().AsLogic();
            _outputPort?.Send(new LogicPacket(v == LogicValue.True ? LogicValue.False : LogicValue.True));
        }

        _outputPort?.Close();
    }
}
