using System.Xml.Linq;

namespace Hypnode.Core;

public static class GraphSerializer
{
    public static string Serialize(GraphDefinition definition)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Graph",
                new XElement("Nodes",
                    definition.Nodes.Select(n =>
                        new XElement("Node",
                            new XAttribute("id",   n.Id),
                            new XAttribute("type", n.TypeName),
                            n.Parameters.Count > 0
                                ? new XElement("Parameters",
                                    n.Parameters.Select(p =>
                                        new XElement("Parameter",
                                            new XAttribute("name", p.Key),
                                            p.Value)))
                                : null))),
                new XElement("Connections",
                    definition.Connections.Select(c =>
                        new XElement("Connection",
                            new XAttribute("type", c.TypeAlias),
                            new XElement("From",
                                new XAttribute("node", c.FromNodeId),
                                new XAttribute("port", c.FromPort)),
                            new XElement("To",
                                new XAttribute("node", c.ToNodeId),
                                new XAttribute("port", c.ToPort)))))));

        return doc.ToString();
    }

    public static GraphDefinition Deserialize(string xml)
    {
        var root       = XDocument.Parse(xml).Root!;
        var definition = new GraphDefinition();

        foreach (var el in root.Element("Nodes")?.Elements("Node") ?? [])
        {
            var parameters = el.Element("Parameters")
                ?.Elements("Parameter")
                .Select(p => (p.Attribute("name")!.Value, p.Value))
                .ToArray()
                ?? [];

            definition.AddNode(
                el.Attribute("id")!.Value,
                el.Attribute("type")!.Value,
                parameters);
        }

        foreach (var el in root.Element("Connections")?.Elements("Connection") ?? [])
        {
            var from = el.Element("From")!;
            var to   = el.Element("To")!;
            definition.Connect(
                el.Attribute("type")!.Value,
                from.Attribute("node")!.Value,
                from.Attribute("port")!.Value,
                to.Attribute("node")!.Value,
                to.Attribute("port")!.Value);
        }

        return definition;
    }
}
