using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Gates;

public class OrGate : INode
{
    public const string InputA = "INA";
    public const string InputB = "INB";
    public const string Output = "OUT";

    private Connection<LogicValue>? _inputPortA = null;
    private Connection<LogicValue>? _inputPortB = null;
    private Connection<LogicValue>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == InputA && connection is Connection<LogicValue> con0) _inputPortA = con0;
        if (portName == InputB && connection is Connection<LogicValue> con1) _inputPortB = con1;
        if (portName == Output && connection is Connection<LogicValue> con2) _outputPort = con2;
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPortA is null) throw new InvalidOperationException("Input port A is not set");
        if (_inputPortB is null) throw new InvalidOperationException("Input port B is not set");

        while (true)
        {
            if ((_inputPortA.IsClosed && !_inputPortA.HasData) || (_inputPortB.IsClosed && !_inputPortB.HasData))
                break;
            if (!_inputPortA.HasData || !_inputPortB.HasData) { yield return null; continue; }

            var a = _inputPortA.Receive();
            var b = _inputPortB.Receive();
            _outputPort?.Send(a == LogicValue.True || b == LogicValue.True ? LogicValue.True : LogicValue.False);
        }

        _outputPort?.Close();
    }
}
