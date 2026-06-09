using Hypnode.Core;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Math;

[HypnodeNode("add-int", "Sums two int streams (IN1, IN2 → OUT)")]
public class AddIntNode : INode
{
    public const string Input1 = "IN1";
    public const string Input2 = "IN2";

    private Connection? _inputPort1;
    private Connection? _inputPort2;
    private Connection? _outputPort;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Input1) NodeExtensions.TryAttach(ref _inputPort1, connection);
        if (portName == Input2) NodeExtensions.TryAttach(ref _inputPort2, connection);
        if (portName == Ports.Output) NodeExtensions.TryAttach(ref _outputPort, connection);
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort1 is null) throw new InvalidOperationException("Input port 1 is not set");
        if (_inputPort2 is null) throw new InvalidOperationException("Input port 2 is not set");

        while (true)
        {
            bool p1Done = _inputPort1.IsClosed && !_inputPort1.HasData;
            bool p2Done = _inputPort2.IsClosed && !_inputPort2.HasData;
            if (p1Done || p2Done) break;

            if (!_inputPort1.HasData || !_inputPort2.HasData) { yield return null; continue; }

            _outputPort?.Send(new IntValue(_inputPort1.Receive().AsInt() + _inputPort2.Receive().AsInt()));
        }

        _outputPort?.Close();
    }
}
