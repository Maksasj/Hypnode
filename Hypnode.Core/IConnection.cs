namespace Hypnode.Core;

public interface IConnection
{
    bool HasData      { get; }
    bool IsClosed     { get; }
    bool HadActivity  { get; }
    void Close();
    void ResetActivity();
}
