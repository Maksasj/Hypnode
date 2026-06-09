using Hypnode.Core.Modules;

namespace Hypnode.Logic.Gates;

[HypnodeNode("or-gate", "Logical OR of two LogicValue inputs")]
public class OrGate : BinaryLogicGate
{
    protected override LogicValue Compute(LogicValue a, LogicValue b) =>
        a == LogicValue.True || b == LogicValue.True ? LogicValue.True : LogicValue.False;
}
