import type { NodeTypeDef } from '../types/hypnode';

export const NODE_TYPES: Record<string, NodeTypeDef> = {
  // Sources
  'pulse-int':    { label: 'Pulse Int',    category: 'Sources', params: [{ name: 'value', default: '0' }],   ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'pulse-bool':   { label: 'Pulse Bool',   category: 'Sources', params: [{ name: 'value', default: 'true' }], ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'bool' }] },
  'pulse-byte':   { label: 'Pulse Byte',   category: 'Sources', params: [{ name: 'value', default: '0' }],   ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'byte' }] },
  'pulse-string': { label: 'Pulse String', category: 'Sources', params: [{ name: 'value', default: '' }],    ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'string' }] },
  'pulse-logic':  { label: 'Pulse Logic',  category: 'Sources', params: [{ name: 'value', default: 'False' }], ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'multi-int':    { label: 'Multi Int',    category: 'Sources', params: [{ name: 'values', default: '1,2,3' }], ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'generator':    { label: 'Generator',    category: 'Sources', ports: [{ id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },

  // Sinks
  'printer-int':    { label: 'Printer Int',    category: 'Sinks', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }] },
  'printer-bool':   { label: 'Printer Bool',   category: 'Sinks', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'bool' }] },
  'printer-string': { label: 'Printer String', category: 'Sinks', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'string' }] },
  'printer-logic':  { label: 'Printer Logic',  category: 'Sinks', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'LogicValue' }] },
  'register-int':   { label: 'Register Int',   category: 'Sinks', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }] },
  'void-int':       { label: 'Void Int',       category: 'Sinks', ports: [{ id: '_',  label: 'IN', direction: 'input', dataType: 'int' }] },
  'void-logic':     { label: 'Void Logic',     category: 'Sinks', ports: [{ id: '_',  label: 'IN', direction: 'input', dataType: 'LogicValue' }] },

  // Routing
  'splitter-int':   { label: 'Splitter Int',   category: 'Routing', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'splitter-logic': { label: 'Splitter Logic', category: 'Routing', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'if-even':        { label: 'If Even',        category: 'Routing', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'THEN', label: 'THEN', direction: 'output', dataType: 'int' }, { id: 'ELSE', label: 'ELSE', direction: 'output', dataType: 'int' }] },
  'if-positive':    { label: 'If Positive',    category: 'Routing', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'THEN', label: 'THEN', direction: 'output', dataType: 'int' }, { id: 'ELSE', label: 'ELSE', direction: 'output', dataType: 'int' }] },

  // Transform
  'squarer':         { label: 'Squarer',          category: 'Transform', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'fold-sum':        { label: 'Fold Sum',          category: 'Transform', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'fold-product':    { label: 'Fold Product',      category: 'Transform', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'fold-count':      { label: 'Fold Count',        category: 'Transform', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'filter-even':     { label: 'Filter Even',       category: 'Transform', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'filter-odd':      { label: 'Filter Odd',        category: 'Transform', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },
  'filter-positive': { label: 'Filter Positive',   category: 'Transform', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'int' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'int' }] },

  // Logic Gates
  'and-gate': { label: 'AND',  category: 'Logic', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'LogicValue' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'or-gate':  { label: 'OR',   category: 'Logic', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'LogicValue' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'xor-gate': { label: 'XOR',  category: 'Logic', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'LogicValue' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },
  'not-gate': { label: 'NOT',  category: 'Logic', ports: [{ id: 'IN',  label: 'IN', direction: 'input', dataType: 'LogicValue' }, { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'LogicValue' }] },

  // Compound Logic
  'full-adder':        { label: 'Full Adder',      category: 'Compound', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'LogicValue' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'LogicValue' }, { id: 'INC', label: 'Cin', direction: 'input', dataType: 'LogicValue' }, { id: 'OUTSUM', label: 'SUM', direction: 'output', dataType: 'LogicValue' }, { id: 'OUTC', label: 'Cout', direction: 'output', dataType: 'LogicValue' }] },
  'full-adder-byte':   { label: 'Full Adder Byte', category: 'Compound', ports: [{ id: 'INA', label: 'A', direction: 'input', dataType: 'byte' }, { id: 'INB', label: 'B', direction: 'input', dataType: 'byte' }, { id: 'OUTSUM', label: 'SUM', direction: 'output', dataType: 'byte' }] },
  'byte-splitter-in':  { label: 'Byte → Bits',     category: 'Compound', ports: [{ id: 'IN', label: 'IN', direction: 'input', dataType: 'byte' }, ...Array.from({ length: 8 }, (_, i) => ({ id: String(i), label: String(i), direction: 'output' as const, dataType: 'LogicValue' as const }))] },
  'byte-splitter-out': { label: 'Bits → Byte',     category: 'Compound', ports: [...Array.from({ length: 8 }, (_, i) => ({ id: String(i), label: String(i), direction: 'input' as const, dataType: 'LogicValue' as const })), { id: 'OUT', label: 'OUT', direction: 'output', dataType: 'byte' }] },
};

export const CATEGORIES = [...new Set(Object.values(NODE_TYPES).map(t => t.category))];

export const PORT_COLORS: Record<string, string> = {
  int:        '#60a5fa',
  bool:       '#34d399',
  byte:       '#f59e0b',
  string:     '#a78bfa',
  float:      '#fb923c',
  double:     '#f87171',
  LogicValue: '#e879f9',
};

export const CATEGORY_COLORS: Record<string, string> = {
  Sources:   '#3b82f6',
  Sinks:     '#6b7280',
  Routing:   '#8b5cf6',
  Transform: '#10b981',
  Logic:     '#f59e0b',
  Compound:  '#ef4444',
};
