using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class ConstValue<T> : INode
{
    private readonly T _value;
    private Connection<T>? _outputPort = null;

    public ConstValue(T value) { _value = value; }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output && connection is Connection<T> con) _outputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        while (true) { _outputPort?.Send(_value); yield return null; }
    }
}
