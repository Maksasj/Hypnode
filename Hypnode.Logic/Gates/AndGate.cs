using Hypnode.Core.Modules;

namespace Hypnode.Logic.Gates;

[HypnodeNode("and-gate", "Logical AND of two LogicValue inputs")]
public class AndGate : BinaryLogicGate
{
    protected override LogicValue Compute(LogicValue a, LogicValue b) =>
        a == LogicValue.True && b == LogicValue.True ? LogicValue.True : LogicValue.False;
}
