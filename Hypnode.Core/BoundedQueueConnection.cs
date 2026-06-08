namespace Hypnode.Core;

public class BoundedQueueConnection<T> : Connection<T>
{
    private readonly Queue<T> _buffer = new();
    private readonly int _capacity;
    private bool _closed = false;
    private bool _hadActivity = false;

    public BoundedQueueConnection(int capacity)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive");
        _capacity = capacity;
    }

    public override bool HasData => _buffer.Count > 0;
    public override bool IsClosed => _closed;
    public override bool HadActivity => _hadActivity;
    public override bool IsFull => _buffer.Count >= _capacity;

    public override void Send(T packet)
    {
        if (_closed) throw new InvalidOperationException("Cannot send to a closed connection");
        if (_buffer.Count >= _capacity) throw new InvalidOperationException($"Connection is full (capacity {_capacity})");
        _hadActivity = true;
        _buffer.Enqueue(packet);
    }

    public override T Receive()
    {
        if (_buffer.Count == 0) throw new InvalidOperationException("No data available in connection");
        return _buffer.Dequeue();
    }

    public override bool TryReceive(out T packet)
    {
        if (_buffer.Count > 0) { packet = _buffer.Dequeue(); return true; }
        packet = default!;
        return false;
    }

    public override void Close() => _closed = true;
    public override void ResetActivity() => _hadActivity = false;
}
