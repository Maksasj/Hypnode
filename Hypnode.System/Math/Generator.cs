using Hypnode.Core;
using System.Collections;

using Hypnode.Core.Modules;

namespace Hypnode.System.Math;

[HypnodeNode("generator", "Emits incrementing int packets indefinitely")]
public class Generator : INode
{
    private Connection<int>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output && connection is Connection<int> con) _outputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        var i = 0;
        while (true) { _outputPort?.Send(i++); yield return null; }
    }
}
