import { Handle, Position, type NodeProps } from '@xyflow/react';
import type { HypnodeNodeData } from '../types/hypnode';
import { CATEGORY_COLORS, PORT_COLORS, NODE_TYPES } from '../data/nodeTypes';
import './HypnodeNode.css';

export function HypnodeNode({ data, selected }: NodeProps) {
  const d = data as HypnodeNodeData;
  const typeDef = NODE_TYPES[d.typeName];
  const accentColor = CATEGORY_COLORS[typeDef?.category ?? ''] ?? '#6b7280';

  const inputs  = d.ports.filter(p => p.direction === 'input');
  const outputs = d.ports.filter(p => p.direction === 'output');

  return (
    <div className={`hn-node ${selected ? 'hn-node--selected' : ''}`} style={{ '--accent': accentColor } as React.CSSProperties}>
      <div className="hn-node__header">{d.label}</div>

      <div className="hn-node__body">
        <div className="hn-node__ports hn-node__ports--inputs">
          {inputs.map(port => (
            <div key={port.id} className="hn-node__port">
              <Handle
                type="target"
                position={Position.Left}
                id={port.id}
                style={{ background: PORT_COLORS[port.dataType] }}
              />
              <span className="hn-node__port-label">{port.label}</span>
            </div>
          ))}
        </div>

        {d.params && Object.keys(d.params).length > 0 && (
          <div className="hn-node__params">
            {Object.entries(d.params).map(([k, v]) => (
              <div key={k} className="hn-node__param">
                <span className="hn-node__param-key">{k}</span>
                <span className="hn-node__param-val">{v}</span>
              </div>
            ))}
          </div>
        )}

        <div className="hn-node__ports hn-node__ports--outputs">
          {outputs.map(port => (
            <div key={port.id} className="hn-node__port hn-node__port--output">
              <span className="hn-node__port-label">{port.label}</span>
              <Handle
                type="source"
                position={Position.Right}
                id={port.id}
                style={{ background: PORT_COLORS[port.dataType] }}
              />
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
