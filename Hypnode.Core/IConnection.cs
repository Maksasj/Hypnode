namespace Hypnode.Core
{
    public interface IConnection
    {
        bool HasData { get; }
        bool IsClosed { get; }
        void Close();
    }
}
