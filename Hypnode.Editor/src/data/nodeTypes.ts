import type { NodeTypeDef } from '../types/hypnode';

export const NODE_TYPES: Record<string, NodeTypeDef> = {
  // System.Common — pulse sources
  'pulse-int':    { label: 'Pulse Int',    category: 'System.Common', params: [{ name: 'value', default: '0' }],      ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'pulse-bool':   { label: 'Pulse Bool',   category: 'System.Common', params: [{ name: 'value', default: 'true' }],   ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'bool' }] },
  'pulse-byte':   { label: 'Pulse Byte',   category: 'System.Common', params: [{ name: 'value', default: '0' }],      ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'byte' }] },
  'pulse-string': { label: 'Pulse String', category: 'System.Common', params: [{ name: 'value', default: '' }],       ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'string' }] },
  'pulse-logic':  { label: 'Pulse Logic',  category: 'System.Common', params: [{ name: 'value', default: 'False' }],  ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'multi-int':    { label: 'Multi Int',    category: 'System.Common', params: [{ name: 'values', default: '1,2,3' }], ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },

  // System.Common — routing / control
  'splitter-int':   { label: 'Splitter Int',   category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'splitter-logic': { label: 'Splitter Logic', category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'delay-int':      { label: 'Delay Int',      category: 'System.Common', params: [{ name: 'value', default: '0' }], ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'register-int':   { label: 'Register Int',   category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }] },
  'void-int':       { label: 'Void Int',       category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }] },
  'void-logic':     { label: 'Void Logic',     category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'LogicValue' }] },
  'if-even':        { label: 'If Even',        category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'THEN', label: 'THEN', direction: 'output', dataType: 'int' }, { id: 'ELSE', label: 'ELSE', direction: 'output', dataType: 'int' }] },
  'if-positive':    { label: 'If Positive',    category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'THEN', label: 'THEN', direction: 'output', dataType: 'int' }, { id: 'ELSE', label: 'ELSE', direction: 'output', dataType: 'int' }] },
  'fold-sum':       { label: 'Fold Sum',       category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'fold-product':   { label: 'Fold Product',   category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'fold-count':     { label: 'Fold Count',     category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'filter-even':    { label: 'Filter Even',    category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'filter-odd':     { label: 'Filter Odd',     category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'filter-positive':{ label: 'Filter Positive',category: 'System.Common', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },

  // System.IO
  'printer-int':    { label: 'Printer Int',    category: 'System.IO', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }] },
  'printer-bool':   { label: 'Printer Bool',   category: 'System.IO', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'bool' }] },
  'printer-string': { label: 'Printer String', category: 'System.IO', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'string' }] },
  'printer-logic':  { label: 'Printer Logic',  category: 'System.IO', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'LogicValue' }] },

  // System.Math
  'generator': { label: 'Generator', category: 'System.Math', ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'squarer':   { label: 'Squarer',   category: 'System.Math', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'add-int':   { label: 'Add Int',   category: 'System.Math', ports: [{ id: 'IN1', label: 'A', direction: 'input', dataType: 'int' }, { id: 'IN2', label: 'B', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },

  // Logic.Gates
  'and-gate': { label: 'AND', category: 'Logic.Gates', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'LogicValue' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'or-gate':  { label: 'OR',  category: 'Logic.Gates', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'LogicValue' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'xor-gate': { label: 'XOR', category: 'Logic.Gates', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'LogicValue' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'not-gate': { label: 'NOT', category: 'Logic.Gates', ports: [{ id: 'IN',  label: 'IN', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },

  // Logic.Compound
  'full-adder':        { label: 'Full Adder',      category: 'Logic.Compound', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'LogicValue' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'LogicValue' }, { id: 'INC', label: 'Cin', direction: 'input', dataType: 'LogicValue' }, { id: 'OUTSUM', label: 'SUM', direction: 'output', dataType: 'LogicValue' }, { id: 'OUTC', label: 'Cout', direction: 'output', dataType: 'LogicValue' }] },
  'full-adder-byte':   { label: 'Full Adder Byte', category: 'Logic.Compound', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'byte' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'byte' }, { id: 'OUTSUM', label: 'SUM', direction: 'output', dataType: 'byte' }] },
  'byte-splitter-in':  { label: 'Byte → Bits',     category: 'Logic.Compound', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'byte' }, ...Array.from({ length: 8 }, (_, i) => ({ id: String(i), label: String(i), direction: 'output' as const, dataType: 'LogicValue' as const }))] },
  'byte-splitter-out': { label: 'Bits → Byte',     category: 'Logic.Compound', ports: [...Array.from({ length: 8 }, (_, i) => ({ id: String(i), label: String(i), direction: 'input' as const, dataType: 'LogicValue' as const })), { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'byte' }] },
};

export const CATEGORIES = [...new Set(Object.values(NODE_TYPES).map(t => t.category))];

export const PORT_COLORS: Record<string, string> = {
  int:        '#3b82f6',
  bool:       '#10b981',
  byte:       '#f59e0b',
  string:     '#8b5cf6',
  float:      '#f97316',
  double:     '#ef4444',
  LogicValue: '#ec4899',
};

export const CATEGORY_COLORS: Record<string, string> = {
  'System.Common':   '#6366f1',
  'System.IO':       '#06b6d4',
  'System.Math':     '#10b981',
  'Logic.Gates':     '#f59e0b',
  'Logic.Compound':  '#ef4444',
};
