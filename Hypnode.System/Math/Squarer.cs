using Hypnode.Core;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Math;

[HypnodeNode("squarer", "Squares each incoming int packet")]
public class Squarer : INode
{
    private Connection<HypnodeValue>? _inputPort;
    private Connection<HypnodeValue>? _outputPort;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input) NodeExtensions.TryAttach(ref _inputPort, connection);
        if (portName == Ports.Output) NodeExtensions.TryAttach(ref _outputPort, connection);
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }
            var n = _inputPort.Receive().AsInt();
            _outputPort?.Send(new IntValue(n * n));
        }

        _outputPort?.Close();
    }
}
