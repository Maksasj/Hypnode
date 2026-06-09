using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Math;

public class AddIntNode : INode
{
    public const string Input1 = "IN1";
    public const string Input2 = "IN2";

    private Connection<int>? _inputPort1 = null;
    private Connection<int>? _inputPort2 = null;
    private Connection<int>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Input1 && connection is Connection<int> con0) _inputPort1 = con0;
        if (portName == Input2 && connection is Connection<int> con1) _inputPort2 = con1;
        if (portName == Ports.Output && connection is Connection<int> con2) _outputPort = con2;
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

            _outputPort?.Send(_inputPort1.Receive() + _inputPort2.Receive());
        }

        _outputPort?.Close();
    }
}
