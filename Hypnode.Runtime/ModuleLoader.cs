using Hypnode.Core.Graph;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Reflection;

namespace Hypnode.Runtime;

public record NodeEntry(string Name, string Description);
public record TypeEntry(string Name, HypnodeType Type);

public record LoadedModule(
    HypnodeModuleAttribute Meta,
    IReadOnlyList<NodeEntry> Nodes,
    IReadOnlyList<string> ConnectionTypes,
    IReadOnlyList<TypeEntry> Types);

public record ModuleLoadResult(IReadOnlyList<LoadedModule> Modules, HypnodeTypeRegistry TypeRegistry);

public static class ModuleLoader
{
    public static ModuleLoadResult LoadAll(IEnumerable<string> dllPaths, NodeFactory factory)
    {
        var registry = new HypnodeTypeRegistry();
        var all = new List<LoadedModule>();
        var paths = dllPaths.ToList();

        if (paths.Count == 0)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                all.AddRange(LoadFromAssembly(assembly, factory, registry));
        }
        else
        {
            foreach (var path in paths)
                all.AddRange(LoadFromFile(path, factory, registry));
        }

        return new ModuleLoadResult(all, registry);
    }

    static List<LoadedModule> LoadFromFile(string dllPath, NodeFactory factory, HypnodeTypeRegistry registry)
    {
        Assembly assembly;
        try { assembly = Assembly.LoadFrom(Path.GetFullPath(dllPath)); }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"error: could not load module '{dllPath}': {ex.Message}");
            return [];
        }

        return LoadFromAssembly(assembly, factory, registry);
    }

    static List<LoadedModule> LoadFromAssembly(Assembly assembly, NodeFactory factory, HypnodeTypeRegistry registry)
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
            var typesBefore = registry.Types.Keys.ToHashSet();

            var module = (IHypnodeModule)Activator.CreateInstance(type)!;
            module.Register(factory, registry);

            var newNodes = factory.NodeTypes
                .Except(nodesBefore).Order()
                .Select(name => new NodeEntry(name, factory.NodeDescriptions.GetValueOrDefault(name, "")))
                .ToList();

            var newConns = factory.ConnectionTypes
                .Except(connsBefore).Order()
                .ToList();

            var newTypes = registry.Types
                .Where(kv => !typesBefore.Contains(kv.Key))
                .OrderBy(kv => kv.Key)
                .Select(kv => new TypeEntry(kv.Key, kv.Value))
                .ToList();

            loaded.Add(new LoadedModule(attr, newNodes, newConns, newTypes));
        }

        return loaded;
    }
}
