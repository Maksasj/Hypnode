using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class MultiPulseValue<T> : INode
{
    private readonly IEnumerable<T> _value;
    private Connection<T>? _outputPort = null;

    public MultiPulseValue(IEnumerable<T> value) { _value = value; }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output && connection is Connection<T> con) _outputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        foreach (var item in _value)
            _outputPort?.Send(item);
        _outputPort?.Close();
        yield break;
    }
}
