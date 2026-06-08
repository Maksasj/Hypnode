using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Gates;

public class NotGate : INode
{
    private Connection<LogicValue>? _inputPort = null;
    private Connection<LogicValue>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == "IN" && connection is Connection<LogicValue> con0) _inputPort = con0;
        if (portName == "OUT" && connection is Connection<LogicValue> con1) _outputPort = con1;

        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null)
            throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData)
                break;

            if (!_inputPort.HasData)
            {
                yield return null;
                continue;
            }

            var packet = _inputPort.Receive();
            _outputPort?.Send(packet == LogicValue.True ? LogicValue.False : LogicValue.True);
        }

        _outputPort?.Close();
    }
}
