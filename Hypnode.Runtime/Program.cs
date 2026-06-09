using Hypnode.Core.Graph;

namespace Hypnode.Runtime;

internal static class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            PrintHelp();
            return 0;
        }

        var subcommand = args[0];
        var rest = args[1..];

        return subcommand switch
        {
            "run"     => RunCommand(rest),
            "modules" => ModulesCommand(rest),
            _         => UnknownCommand(subcommand),
        };
    }

    static int RunCommand(string[] args)
    {
        var modulePaths = new List<string>();
        string? graphFile = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--module" && i + 1 < args.Length)
                modulePaths.Add(args[++i]);
            else
                graphFile = args[i];
        }

        if (graphFile is null)
        {
            Console.Error.WriteLine("error: no graph file specified");
            Console.Error.WriteLine("usage: hypnode run [--module <dll>]... <graph.xml>");
            return 1;
        }

        if (!File.Exists(graphFile))
        {
            Console.Error.WriteLine($"error: file not found: {graphFile}");
            return 1;
        }

        var factory = new NodeFactory();
        ModuleLoader.LoadAll(modulePaths, factory);

        string xml;
        try { xml = File.ReadAllText(graphFile); }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"error: could not read file: {ex.Message}");
            return 1;
        }

        GraphDefinition definition;
        try { definition = GraphSerializer.Deserialize(xml); }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"error: invalid graph XML: {ex.Message}");
            return 1;
        }

        CoroutineNodeGraph graph;
        try { graph = factory.Build(definition); }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"error: could not build graph: {ex.Message}");
            return 1;
        }

        try { graph.Evaluate(); }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"error: graph evaluation failed: {ex.Message}");
            return 1;
        }

        return 0;
    }

    static int ModulesCommand(string[] args)
    {
        var modulePaths = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--module" && i + 1 < args.Length)
                modulePaths.Add(args[++i]);
        }

        var factory = new NodeFactory();
        var loaded = ModuleLoader.LoadAll(modulePaths, factory);

        if (loaded.Count == 0)
        {
            Console.WriteLine("No modules loaded.");
            return 0;
        }

        Console.WriteLine($"Loaded {loaded.Count} module(s):\n");

        foreach (var m in loaded)
        {
            Console.WriteLine($"  {m.Meta.Name} v{m.Meta.Version}");
            Console.WriteLine($"  {m.Meta.Description}");

            if (m.Nodes.Count > 0)
            {
                Console.WriteLine($"\n  Nodes ({m.Nodes.Count}):");
                foreach (var node in m.Nodes)
                    Console.WriteLine($"    {node.Name,-24} {node.Description}");
            }

            if (m.ConnectionTypes.Count > 0)
            {
                Console.WriteLine($"\n  Connection types ({m.ConnectionTypes.Count}):");
                foreach (var conn in m.ConnectionTypes)
                    Console.WriteLine($"    {conn}");
            }

            Console.WriteLine();
        }

        return 0;
    }

    static int UnknownCommand(string cmd)
    {
        Console.Error.WriteLine($"error: unknown command '{cmd}'");
        PrintHelp();
        return 1;
    }

    static void PrintHelp()
    {
        Console.WriteLine("""
            Hypnode Runtime
            ===============
            Usage:
              hypnode run [--module <dll>]... <graph.xml>
              hypnode modules [--module <dll>]...
              hypnode --help

            Commands:
              run        Load modules and evaluate a graph XML file
              modules    List all loaded modules

            Options:
              --module <dll>   Load a module DLL (may be repeated).
                               If omitted, all modules in the current process are used.
            """);
    }
}
