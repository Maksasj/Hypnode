using System.Reflection;
using Hypnode.Core.Graph;
using Hypnode.Core.Modules;

namespace Hypnode.Runtime;

public record NodeEntry(string Name, string Description);

public record LoadedModule(
    HypnodeModuleAttribute Meta,
    IReadOnlyList<NodeEntry> Nodes,
    IReadOnlyList<string> ConnectionTypes);

public static class ModuleLoader
{
    public static List<LoadedModule> LoadFromFile(string dllPath, NodeFactory factory)
    {
        Assembly assembly;
        try { assembly = Assembly.LoadFrom(Path.GetFullPath(dllPath)); }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"error: could not load module '{dllPath}': {ex.Message}");
            return [];
        }

        return LoadFromAssembly(assembly, factory);
    }

    public static List<LoadedModule> LoadFromAssembly(Assembly assembly, NodeFactory factory)
    {
        var loaded = new List<LoadedModule>();

        foreach (var type in assembly.GetTypes())
        {
            var attr = type.GetCustomAttribute<HypnodeModuleAttribute>();
            if (attr is null) continue;
            if (!typeof(IHypnodeModule).IsAssignableFrom(type)) continue;
            if (type.GetConstructor(Type.EmptyTypes) is null) continue;

            var nodesBefore = factory.NodeTypes.ToHashSet();
            var connsBefore = factory.ConnectionTypes.ToHashSet();

            var module = (IHypnodeModule)Activator.CreateInstance(type)!;
            module.Register(factory);

            var newNodes = factory.NodeTypes
                .Except(nodesBefore)
                .Order()
                .Select(name => new NodeEntry(name, factory.NodeDescriptions.GetValueOrDefault(name, "")))
                .ToList();

            var newConns = factory.ConnectionTypes
                .Except(connsBefore)
                .Order()
                .ToList();

            loaded.Add(new LoadedModule(attr, newNodes, newConns));
        }

        return loaded;
    }

    public static List<LoadedModule> LoadAll(IEnumerable<string> dllPaths, NodeFactory factory)
    {
        var all = new List<LoadedModule>();

        if (!dllPaths.Any())
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                all.AddRange(LoadFromAssembly(assembly, factory));
        }
        else
        {
            foreach (var path in dllPaths)
                all.AddRange(LoadFromFile(path, factory));
        }

        return all;
    }
}
