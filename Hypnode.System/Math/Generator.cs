using Hypnode.Core;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Math;

[HypnodeNode("generator", "Emits incrementing int packets indefinitely")]
public class Generator : INode
{
    private Connection? _outputPort;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output) NodeExtensions.TryAttach(ref _outputPort, connection);
        return this;
    }

    public IEnumerator Execute()
    {
        var i = 0;
        while (true) { _outputPort?.Send(new IntValue(i++)); yield return null; }
    }
}
