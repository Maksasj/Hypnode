using Hypnode.Core;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.IO;

[HypnodeNode("printer", "Prints each received value to stdout")]
public class Printer : INode
{
    private Connection<HypnodeValue>? _inputPort;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input) NodeExtensions.TryAttach(ref _inputPort, connection);
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }
            Console.WriteLine(_inputPort.Receive());
        }
    }
}
