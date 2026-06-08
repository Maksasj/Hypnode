using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Gates;

public class NotGate : INode
{
    private Connection<LogicValue>? _inputPort = null;
    private Connection<LogicValue>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        var result = portName switch
        {
            Ports.Input => NodeExtensions.TryAttach(ref _inputPort, connection),
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
            _outputPort?.Send(_inputPort.Receive() == LogicValue.True ? LogicValue.False : LogicValue.True);
        }

        _outputPort?.Close();
    }
}
