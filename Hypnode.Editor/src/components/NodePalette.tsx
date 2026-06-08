import { NODE_TYPES, CATEGORIES, CATEGORY_COLORS } from '../data/nodeTypes';

export function NodePalette() {
  function onDragStart(e: React.DragEvent, typeName: string) {
    e.dataTransfer.setData('application/hypnode-type', typeName);
    e.dataTransfer.effectAllowed = 'move';
  }

  return (
    <aside className="w-56 shrink-0 bg-card border-r border-border flex flex-col overflow-hidden">
      {/* Header */}
      <div className="px-4 py-3 border-b border-border">
        <p className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
          Nodes
        </p>
      </div>

      {/* Node list */}
      <div className="flex-1 overflow-y-auto py-2 scrollbar-thin">
        {CATEGORIES.map(cat => (
          <div key={cat} className="mb-1">
            {/* Category label */}
            <div className="flex items-center gap-2 px-4 py-1.5">
              <div className="w-1.5 h-1.5 rounded-full shrink-0" style={{ background: CATEGORY_COLORS[cat] }} />
              <span
                className="text-[10px] font-semibold uppercase tracking-widest"
                style={{ color: CATEGORY_COLORS[cat] }}
              >
                {cat}
              </span>
            </div>

            {/* Items */}
            {Object.entries(NODE_TYPES)
              .filter(([, def]) => def.category === cat)
              .map(([typeName, def]) => (
                <div
                  key={typeName}
                  className="mx-2 px-3 py-1.5 rounded-md text-[12px] text-muted-foreground
                             cursor-grab select-none flex items-center gap-2
                             hover:bg-muted hover:text-foreground transition-colors duration-150
                             active:cursor-grabbing active:opacity-80"
                  draggable
                  onDragStart={e => onDragStart(e, typeName)}
                >
                  {def.label}
                </div>
              ))}
          </div>
        ))}
      </div>
    </aside>
  );
}
