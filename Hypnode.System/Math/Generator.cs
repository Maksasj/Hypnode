using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Math;

public class Generator : INode
{
    private Connection<int>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == "OUT" && connection is Connection<int> con) _outputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        var i = 0;
        while (true)
        {
            _outputPort?.Send(i++);
            yield return null;
        }
    }
}
