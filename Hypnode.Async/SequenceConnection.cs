using Hypnode.Core;

namespace Hypnode.Async
{
    public class SequenceConnection<T> : Connection<T>
    {
        private Queue<T> Buffer { get; }
        private bool IsClosed { get; set; }

        public SequenceConnection()
        {
            Buffer = new Queue<T>();
            IsClosed = false;
        }

        public override T Receive()
        {
            if (Buffer.Count == 0)
                throw new InvalidOperationException("No data available in connection");

            return Buffer.Dequeue();
        }

        public override bool TryReceive(out T packet)
        {
            if (Buffer.Count > 0)
            {
                packet = Buffer.Dequeue();
                return true;
            }

            packet = default!;
            return false;
        }

        public override void Send(T packet)
        {
            if (IsClosed)
                throw new InvalidOperationException("Connection is closed");

            Buffer.Enqueue(packet);
        }

        public override void Close()
        {
            IsClosed = true;
        }
    }
}
