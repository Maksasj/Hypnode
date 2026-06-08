using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class ConstValue<T> : INode
{
    public const string Output = "OUT";

    private T Value { get; set; }
    private Connection<T>? _outputPort = null;

    public ConstValue(T value) { Value = value; }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Output && connection is Connection<T> con) _outputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        while (true)
        {
            _outputPort?.Send(Value);
            yield return null;
        }
    }
}
