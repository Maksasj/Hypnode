using Hypnode.Logic;
using Hypnode.Logic.Gates;
using Hypnode.Runtime;
using Hypnode.System.Common;
using Hypnode.System.IO;
using Hypnode.System.Math;

namespace Hypnode.Example
{
    class Program
    {
        private static void TestCircuit()
        {
            var graph = new CoroutineNodeGraph();
            var conn1 = graph.CreateConnection<int>();
            var conn2 = graph.CreateConnection<int>();

            graph.AddNode(new Generator())
                .SetPort("OUT", conn1);

            graph.AddNode(new Squarer())
                .SetPort("IN", conn1)
                .SetPort("OUT", conn2);

            graph.AddNode(new Printer<int>())
                .SetPort("IN", conn2);

            graph.Evaluate();
        }

        private static void Adder()
        {
            var graph = new CoroutineNodeGraph();
            var AtoDemux1 = graph.CreateConnection<LogicValue>();
            var Demux1toXor1 = graph.CreateConnection<LogicValue>();
            var BtoDemux2 = graph.CreateConnection<LogicValue>();
            var Demux2toXor1 = graph.CreateConnection<LogicValue>();
            var CtoDemux3 = graph.CreateConnection<LogicValue>();
            var Demux3toXor2 = graph.CreateConnection<LogicValue>();
            var Xor1toDemux4 = graph.CreateConnection<LogicValue>();
            var Demux4toXor2 = graph.CreateConnection<LogicValue>();
            var Demux4toAnd1 = graph.CreateConnection<LogicValue>();
            var Demux3toAnd1 = graph.CreateConnection<LogicValue>();
            var Demux1toAnd2 = graph.CreateConnection<LogicValue>();
            var Demux2toAnd2 = graph.CreateConnection<LogicValue>();
            var And1toOr = graph.CreateConnection<LogicValue>();
            var And2toOr = graph.CreateConnection<LogicValue>();
            var toCarryOut = graph.CreateConnection<LogicValue>();
            var toSum = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<LogicValue>(LogicValue.True)).SetPort("OUT", AtoDemux1);
            graph.AddNode(new Splitter<LogicValue>()).SetPort("IN", AtoDemux1).SetPort("OUT", Demux1toXor1).SetPort("OUT", Demux1toAnd2);
            graph.AddNode(new PulseValue<LogicValue>(LogicValue.True)).SetPort("OUT", BtoDemux2);
            graph.AddNode(new Splitter<LogicValue>()).SetPort("IN", BtoDemux2).SetPort("OUT", Demux2toXor1).SetPort("OUT", Demux2toAnd2);
            graph.AddNode(new PulseValue<LogicValue>(LogicValue.True)).SetPort("OUT", CtoDemux3);
            graph.AddNode(new Splitter<LogicValue>()).SetPort("IN", CtoDemux3).SetPort("OUT", Demux3toXor2).SetPort("OUT", Demux3toAnd1);
            graph.AddNode(new XorGate()).SetPort("INA", Demux1toXor1).SetPort("INB", Demux2toXor1).SetPort("OUT", Xor1toDemux4);
            graph.AddNode(new XorGate()).SetPort("INA", Demux3toXor2).SetPort("INB", Demux4toXor2).SetPort("OUT", toSum);

            var sumCell = graph.AddNode(new Register<LogicValue>()).SetPort("IN", toSum);

            graph.AddNode(new Splitter<LogicValue>()).SetPort("IN", Xor1toDemux4).SetPort("OUT", Demux4toXor2).SetPort("OUT", Demux4toAnd1);
            graph.AddNode(new AndGate()).SetPort("INA", Demux4toAnd1).SetPort("INB", Demux3toAnd1).SetPort("OUT", And1toOr);
            graph.AddNode(new AndGate()).SetPort("INA", Demux1toAnd2).SetPort("INB", Demux2toAnd2).SetPort("OUT", And2toOr);
            graph.AddNode(new OrGate()).SetPort("INA", And2toOr).SetPort("INB", And1toOr).SetPort("OUT", toCarryOut);

            graph.AddNode(new Printer<LogicValue>()).SetPort("IN", toCarryOut);

            graph.Evaluate();
        }

        private static void TestSome()
        {
            var graph = new CoroutineNodeGraph();
            var connection = graph.CreateConnection<LogicValue>();

            graph.AddNode(new PulseValue<LogicValue>(LogicValue.False)).SetPort("OUT", connection);
            graph.AddNode(new Register<LogicValue>()).SetPort("IN", connection);

            graph.Evaluate();
        }

        public static void Main()
        {
            Adder();
        }
    }
}
