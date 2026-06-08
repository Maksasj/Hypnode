using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Gates;

public class XorGate : INode
{
    private Connection<LogicValue>? inputPortA = null;
    private Connection<LogicValue>? inputPortB = null;
    private Connection<LogicValue>? outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        var result = portName switch
        {
            "INA" => NodeExtensions.TryAttach(ref inputPortA, connection),
            "INB" => NodeExtensions.TryAttach(ref inputPortB, connection),
            "OUT" => NodeExtensions.TryAttach(ref outputPort, connection),
            _ => throw new InvalidOperationException($"Port {portName} is invalid"),
        };

        if (!result)
            throw new InvalidOperationException($"Port {portName} is already set or type is invalid");

        return this;
    }

    public IEnumerator Execute()
    {
        if (inputPortA is null)
            throw new InvalidOperationException("Input port A is not set");

        if (inputPortB is null)
            throw new InvalidOperationException("Input port B is not set");

        while (true)
        {
            if ((inputPortA.IsClosed && !inputPortA.HasData) ||
                (inputPortB.IsClosed && !inputPortB.HasData))
                break;

            if (!inputPortA.HasData || !inputPortB.HasData)
            {
                yield return null;
                continue;
            }

            var a = inputPortA.Receive();
            var b = inputPortB.Receive();
            outputPort?.Send(a != b ? LogicValue.True : LogicValue.False);
        }

        outputPort?.Close();
    }
}
