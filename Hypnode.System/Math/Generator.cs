using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Math;

public class Generator : INode
{
    private Connection<int>? outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == "OUT" && connection is Connection<int> con) outputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        var i = 0;
        while (true)
        {
            outputPort?.Send(i++);
            yield return null;
        }
    }
}
