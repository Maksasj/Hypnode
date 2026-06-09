namespace Hypnode.Core.Connections;

public class QueueConnection<T> : Connection<T>
{
    private readonly Queue<T> _buffer = new();
    private bool _closed = false;
    private bool _hadActivity = false;

    public override bool HasData => _buffer.Count > 0;
    public override bool IsClosed => _closed;
    public override bool HadActivity => _hadActivity;

    public override T Receive()
    {
        if (_buffer.Count == 0)
            throw new InvalidOperationException("No data available in connection");
        return _buffer.Dequeue();
    }

    public override bool TryReceive(out T packet)
    {
        if (_buffer.Count > 0) { packet = _buffer.Dequeue(); return true; }
        packet = default!;
        return false;
    }

    public override void Send(T packet)
    {
        if (_closed) throw new InvalidOperationException("Cannot send to a closed connection");
        _hadActivity = true;
        _buffer.Enqueue(packet);
    }

    public override void Close() => _closed = true;
    public override void ResetActivity() => _hadActivity = false;
}
