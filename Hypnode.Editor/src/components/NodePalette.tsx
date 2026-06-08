import { NODE_TYPES, CATEGORIES, CATEGORY_COLORS } from '../data/nodeTypes';
import './NodePalette.css';

export function NodePalette() {
  function onDragStart(e: React.DragEvent, typeName: string) {
    e.dataTransfer.setData('application/hypnode-type', typeName);
    e.dataTransfer.effectAllowed = 'move';
  }

  return (
    <aside className="palette">
      <div className="palette__title">Nodes</div>
      {CATEGORIES.map(cat => (
        <div key={cat} className="palette__group">
          <div className="palette__group-label" style={{ color: CATEGORY_COLORS[cat] }}>
            {cat}
          </div>
          {Object.entries(NODE_TYPES)
            .filter(([, def]) => def.category === cat)
            .map(([typeName, def]) => (
              <div
                key={typeName}
                className="palette__item"
                draggable
                onDragStart={e => onDragStart(e, typeName)}
              >
                <span
                  className="palette__item-dot"
                  style={{ background: CATEGORY_COLORS[cat] }}
                />
                {def.label}
              </div>
            ))}
        </div>
      ))}
    </aside>
  );
}
