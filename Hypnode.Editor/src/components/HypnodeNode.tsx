import { Handle, Position, type NodeProps } from '@xyflow/react';
import type { HypnodeNodeData, PortDef } from '../types/hypnode';
import { NODE_TYPES, CATEGORY_COLORS, PORT_COLORS } from '../data/nodeTypes';

function PortRow({ port, side }: { port: PortDef; side: 'left' | 'right' }) {
  const isLeft = side === 'left';
  return (
    <div className={`relative flex items-center gap-2 py-1 ${isLeft ? 'pl-3 pr-4' : 'pl-4 pr-3 justify-end'}`}>
      {isLeft && (
        <Handle
          type="target"
          position={Position.Left}
          id={port.id}
          style={{ background: PORT_COLORS[port.dataType], left: -5 }}
        />
      )}
      <span className="text-[11px] text-muted-foreground leading-none select-none">{port.label}</span>
      {!isLeft && (
        <Handle
          type="source"
          position={Position.Right}
          id={port.id}
          style={{ background: PORT_COLORS[port.dataType], right: -5 }}
        />
      )}
    </div>
  );
}

export function HypnodeNode({ data, selected }: NodeProps) {
  const d = data as HypnodeNodeData;
  const typeDef = NODE_TYPES[d.typeName];
  const accentColor = CATEGORY_COLORS[typeDef?.category ?? ''] ?? '#52525b';

  const inputs  = d.ports.filter(p => p.direction === 'input');
  const outputs = d.ports.filter(p => p.direction === 'output');
  const hasParams = d.params && Object.keys(d.params).length > 0;

  return (
    <div
      className="bg-card rounded-lg shadow-md transition-shadow duration-150 min-w-[160px]"
      style={{
        border: `1px solid ${selected ? accentColor : 'var(--color-border)'}`,
        boxShadow: selected
          ? `0 0 0 1px ${accentColor}40, 0 4px 16px rgba(0,0,0,0.5)`
          : '0 2px 8px rgba(0,0,0,0.35)',
      }}
    >
      {/* Accent bar */}
      <div className="h-[3px] rounded-t-lg w-full" style={{ background: accentColor }} />

      {/* Header */}
      <div className="px-3 pt-2 pb-1.5 flex items-center justify-between gap-3">
        <span className="text-[11px] font-semibold text-foreground tracking-wide uppercase leading-none">
          {d.label}
        </span>
        {typeDef?.category && (
          <span className="text-[9px] font-medium uppercase tracking-widest text-muted-foreground leading-none">
            {typeDef.category}
          </span>
        )}
      </div>

      {/* Divider */}
      <div className="h-px bg-border mx-0" />

      {/* Ports */}
      <div className="py-1.5">
        <div className="flex justify-between">
          {/* Inputs */}
          <div className="flex flex-col flex-1">
            {inputs.map(p => <PortRow key={p.id} port={p} side="left" />)}
          </div>
          {/* Outputs */}
          <div className="flex flex-col flex-1 items-end">
            {outputs.map(p => <PortRow key={p.id} port={p} side="right" />)}
          </div>
        </div>

        {/* Params */}
        {hasParams && (
          <div className="mx-2.5 mt-1.5 mb-0.5 rounded-md bg-muted px-2.5 py-2 flex flex-col gap-1">
            {Object.entries(d.params).map(([k, v]) => (
              <div key={k} className="flex items-baseline justify-between gap-3">
                <span className="text-[10px] text-muted-foreground">{k}</span>
                <span className="text-[10px] font-mono text-foreground/80 truncate max-w-[80px]">{v || '—'}</span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
