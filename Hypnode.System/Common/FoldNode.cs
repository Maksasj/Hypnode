using Hypnode.Core;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Common;

public class FoldNode : INode
{
    private readonly HypnodeValue _seed;
    private readonly Func<HypnodeValue, HypnodeValue, HypnodeValue> _folder;
    private Connection<HypnodeValue>? _inputPort;
    private Connection<HypnodeValue>? _outputPort;

    public FoldNode(HypnodeValue seed, Func<HypnodeValue, HypnodeValue, HypnodeValue> folder)
    {
        _seed = seed;
        _folder = folder;
    }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input) NodeExtensions.TryAttach(ref _inputPort, connection);
        if (portName == Ports.Output) NodeExtensions.TryAttach(ref _outputPort, connection);
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        var acc = _seed;

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }

            acc = _folder(acc, _inputPort.Receive());
            _outputPort?.Send(acc);
        }

        _outputPort?.Close();
    }
}
