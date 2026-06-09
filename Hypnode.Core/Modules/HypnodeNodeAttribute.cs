namespace Hypnode.Core.Modules;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class HypnodeNodeAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public HypnodeNodeAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
