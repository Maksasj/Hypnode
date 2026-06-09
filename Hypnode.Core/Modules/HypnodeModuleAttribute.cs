namespace Hypnode.Core.Modules;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class HypnodeModuleAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public string Version { get; }

    public HypnodeModuleAttribute(string name, string description, string version)
    {
        Name = name;
        Description = description;
        Version = version;
    }
}
