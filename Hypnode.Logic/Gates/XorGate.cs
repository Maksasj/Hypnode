using Hypnode.Core.Modules;

namespace Hypnode.Logic.Gates;

[HypnodeNode("xor-gate", "Logical XOR of two LogicValue inputs")]
public class XorGate : BinaryLogicGate
{
    protected override LogicValue Compute(LogicValue a, LogicValue b) =>
        a != b ? LogicValue.True : LogicValue.False;
}
