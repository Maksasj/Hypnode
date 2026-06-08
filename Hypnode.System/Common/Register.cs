using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class Register<T> : INode
{
    private T? value;
    private Connection<T>? inputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == "IN" && connection is Connection<T> con) inputPort = con;
        return this;
    }

    public T? GetValue() => value;

    public IEnumerator Execute()
    {
        if (inputPort is null)
            throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (inputPort.IsClosed && !inputPort.HasData)
                break;

            if (!inputPort.HasData)
            {
                yield return null;
                continue;
            }

            value = inputPort.Receive();
        }
    }
}
