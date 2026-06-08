using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Gates;

public class XorGate : INode
{
    private Connection<LogicValue>? _inputPortA = null;
    private Connection<LogicValue>? _inputPortB = null;
    private Connection<LogicValue>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        var result = portName switch
        {
            "INA" => NodeExtensions.TryAttach(ref _inputPortA, connection),
            "INB" => NodeExtensions.TryAttach(ref _inputPortB, connection),
            "OUT" => NodeExtensions.TryAttach(ref _outputPort, connection),
            _ => throw new InvalidOperationException($"Port {portName} is invalid"),
        };

        if (!result)
            throw new InvalidOperationException($"Port {portName} is already set or type is invalid");

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
            _outputPort?.Send(a != b ? LogicValue.True : LogicValue.False);
        }

        _outputPort?.Close();
    }
}
