using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class PulseValue<T> : INode
{
    private T Value { get; set; }
    private Connection<T>? _outputPort = null;

    public PulseValue(T value) { Value = value; }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output && connection is Connection<T> con) _outputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        _outputPort?.Send(Value);
        _outputPort?.Close();
        yield break;
    }
}
