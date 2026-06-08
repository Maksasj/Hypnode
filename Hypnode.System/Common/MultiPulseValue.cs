using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class MultiPulseValue<T> : INode
{
    private IEnumerable<T> Value { get; set; }
    private Connection<T>? _outputPort = null;

    public MultiPulseValue(IEnumerable<T> value)
    {
        Value = value;
    }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == "OUT" && connection is Connection<T> con) _outputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        foreach (var item in Value)
            _outputPort?.Send(item);

        _outputPort?.Close();
        yield break;
    }
}
