import { Handle, Position, type NodeProps } from '@xyflow/react';
import type { HypnodeNodeData, PortDef } from '../types/hypnode';
import { NODE_TYPES, CATEGORY_COLORS, PORT_COLORS } from '../data/nodeTypes';

function PortRow({ port, side }: { port: PortDef; side: 'left' | 'right' }) {
  const isLeft = side === 'left';
  const color = PORT_COLORS[port.dataType] ?? '#a1a1aa';
  return (
    <div className={`relative flex items-center gap-1.5 py-[3px] ${isLeft ? 'pl-4 pr-3' : 'pl-3 pr-4 justify-end'}`}>
      {isLeft && (
        <Handle
          type="target"
          position={Position.Left}
          id={port.id}
          style={{ background: color, left: -5, border: '2px solid #fff' }}
        />
      )}
      <span className="text-[11px] text-zinc-500 leading-none select-none">{port.label}</span>
      {!isLeft && (
        <Handle
          type="source"
          position={Position.Right}
          id={port.id}
          style={{ background: color, right: -5, border: '2px solid #fff' }}
        />
      )}
    </div>
  );
}

export function HypnodeNode({ data, selected }: NodeProps) {
  const d = data as HypnodeNodeData;
  const typeDef = NODE_TYPES[d.typeName];
  const accentColor = CATEGORY_COLORS[typeDef?.category ?? ''] ?? '#a1a1aa';

  const inputs  = d.ports.filter(p => p.direction === 'input');
  const outputs = d.ports.filter(p => p.direction === 'output');
  const hasParams = d.params && Object.keys(d.params).length > 0;
  const ns = typeDef?.category ?? '';

  return (
    <div
      className="bg-white rounded-lg min-w-[160px] transition-shadow duration-100"
      style={{
        border: `1px solid ${selected ? accentColor : '#e4e4e7'}`,
        boxShadow: selected
          ? `0 0 0 2px ${accentColor}30, 0 2px 8px rgba(0,0,0,0.10)`
          : '0 1px 4px rgba(0,0,0,0.07)',
      }}
    >
      {/* Header */}
      <div className="px-3 pt-2 pb-1.5 flex items-center gap-2">
        <div className="w-1.5 h-1.5 rounded-full shrink-0" style={{ background: accentColor }} />
        <span className="text-[12px] font-semibold text-zinc-800 leading-none flex-1 truncate">
          {d.label}
        </span>
        <span className="text-[9px] text-zinc-400 leading-none font-mono shrink-0">
          {ns.replace('System.', '').replace('Logic.', '')}
        </span>
      </div>

      {/* Divider */}
      <div className="h-px bg-zinc-100" />

      {/* Ports */}
      <div className="py-1">
        <div className="flex">
          <div className="flex flex-col flex-1">
            {inputs.map(p => <PortRow key={p.id} port={p} side="left" />)}
          </div>
          <div className="flex flex-col flex-1 items-end">
            {outputs.map(p => <PortRow key={p.id} port={p} side="right" />)}
          </div>
        </div>

        {hasParams && (
          <div className="mx-2.5 mt-1 mb-0.5 rounded-md bg-zinc-50 border border-zinc-100 px-2.5 py-1.5 flex flex-col gap-0.5">
            {Object.entries(d.params).map(([k, v]) => (
              <div key={k} className="flex items-baseline justify-between gap-3">
                <span className="text-[10px] text-zinc-400">{k}</span>
                <span className="text-[10px] font-mono text-zinc-600 truncate max-w-[72px]">{v || '—'}</span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
