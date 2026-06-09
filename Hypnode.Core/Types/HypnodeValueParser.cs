namespace Hypnode.Core.Types;

public static class HypnodeValueParser
{
    public static HypnodeValue Parse(HypnodeTypeRegistry registry, string typeName, string valueStr)
    {
        if (!registry.TryResolve(typeName, out var type))
            throw new InvalidOperationException($"Unknown type '{typeName}'");

        return type switch
        {
            HypnodeType.Primitive p => p.Name switch
            {
                "int"    => new IntValue(int.Parse(valueStr)),
                "float"  => new FloatValue(float.Parse(valueStr)),
                "double" => new DoubleValue(double.Parse(valueStr)),
                "bool"   => new BoolValue(bool.Parse(valueStr)),
                "byte"   => new ByteValue(byte.Parse(valueStr)),
                "string" => new StringValue(valueStr),
                _ => throw new InvalidOperationException($"No parser registered for primitive type '{p.Name}'"),
            },
            _ => throw new InvalidOperationException($"Cannot parse complex type '{typeName}' from a string parameter"),
        };
    }
}
