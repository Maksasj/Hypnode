using Hypnode.Core.Types;

namespace Hypnode.Core;

public abstract class Connection : IConnection
{
    public abstract bool HasData { get; }
    public abstract bool IsClosed { get; }
    public abstract bool HadActivity { get; }
    public virtual bool IsFull => false;

    public abstract HypnodeValue Receive();
    public abstract void Send(HypnodeValue packet);
    public abstract bool TryReceive(out HypnodeValue packet);
    public abstract void Close();
    public abstract void ResetActivity();
}
