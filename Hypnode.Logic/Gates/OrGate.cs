namespace Hypnode.Logic.Gates;

public class OrGate : BinaryLogicGate
{
    protected override LogicValue Compute(LogicValue a, LogicValue b) =>
        a == LogicValue.True || b == LogicValue.True ? LogicValue.True : LogicValue.False;
}
