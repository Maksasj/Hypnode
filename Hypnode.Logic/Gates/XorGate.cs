namespace Hypnode.Logic.Gates;

public class XorGate : BinaryLogicGate
{
    protected override LogicValue Compute(LogicValue a, LogicValue b) =>
        a != b ? LogicValue.True : LogicValue.False;
}
