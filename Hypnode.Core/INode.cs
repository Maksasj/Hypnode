using System.Collections;

namespace Hypnode.Core;

public interface INode
{
    INode SetPort(string portName, IConnection connection);
    IEnumerator Execute();
}
