using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Gates;

public class AndGate : INode
{
    private Connection<LogicValue>? _inputPortA = null;
    private Connection<LogicValue>? _inputPortB = null;
    private Connection<LogicValue>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == "INA" && connection is Connection<LogicValue> con0) _inputPortA = con0;
        if (portName == "INB" && connection is Connection<LogicValue> con1) _inputPortB = con1;
        if (portName == "OUT" && connection is Connection<LogicValue> con2) _outputPort = con2;

        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPortA is null)
            throw new InvalidOperationException("Input port A is not set");

        if (_inputPortB is null)
            throw new InvalidOperationException("Input port B is not set");

        while (true)
        {
            if ((_inputPortA.IsClosed && !_inputPortA.HasData) ||
                (_inputPortB.IsClosed && !_inputPortB.HasData))
                break;

            if (!_inputPortA.HasData || !_inputPortB.HasData)
            {
                yield return null;
                continue;
            }

            var a = _inputPortA.Receive();
            var b = _inputPortB.Receive();
            var result = (a == LogicValue.True && b == LogicValue.True) ? LogicValue.True : LogicValue.False;
            _outputPort?.Send(result);
        }

        _outputPort?.Close();
    }
}
