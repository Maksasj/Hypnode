using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Gates;

public abstract class BinaryLogicGate : INode
{
    public const string InputA = "INA";
    public const string InputB = "INB";
    public const string Output = "OUT";

    private Connection? _inputPortA;
    private Connection? _inputPortB;
    private Connection? _outputPort;

    protected abstract LogicValue Compute(LogicValue a, LogicValue b);

    public INode SetPort(string portName, IConnection connection)
    {
        var result = portName switch
        {
            InputA => NodeExtensions.TryAttach(ref _inputPortA, connection),
            InputB => NodeExtensions.TryAttach(ref _inputPortB, connection),
            Output => NodeExtensions.TryAttach(ref _outputPort, connection),
            _ => throw new InvalidOperationException($"Unknown port '{portName}'"),
        };

        if (!result)
            throw new InvalidOperationException($"Port '{portName}' is already set or type mismatch");

        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPortA is null) throw new InvalidOperationException("Input port A is not set");
        if (_inputPortB is null) throw new InvalidOperationException("Input port B is not set");

        while (true)
        {
            if ((_inputPortA.IsClosed && !_inputPortA.HasData) ||
                (_inputPortB.IsClosed && !_inputPortB.HasData))
                break;

            if (!_inputPortA.HasData || !_inputPortB.HasData) { yield return null; continue; }

            var a = _inputPortA.Receive().AsLogic();
            var b = _inputPortB.Receive().AsLogic();
            _outputPort?.Send(new LogicPacket(Compute(a, b)));
        }

        _outputPort?.Close();
    }
}
