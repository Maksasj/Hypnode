import { NODE_TYPES, CATEGORIES, CATEGORY_COLORS } from '../data/nodeTypes';

export function NodePalette() {
  function onDragStart(e: React.DragEvent, typeName: string) {
    e.dataTransfer.setData('application/hypnode-type', typeName);
    e.dataTransfer.effectAllowed = 'move';
  }

  return (
    <aside className="w-56 shrink-0 bg-white border-r border-zinc-200 flex flex-col overflow-hidden">
      {/* Header */}
      <div className="px-4 py-3 border-b border-zinc-100">
        <p className="text-[11px] font-semibold uppercase tracking-widest text-zinc-400">
          Node Library
        </p>
      </div>

      {/* Scrollable list */}
      <div className="flex-1 overflow-y-auto py-2">
        {CATEGORIES.map(cat => (
          <div key={cat} className="mb-3">
            {/* Namespace header */}
            <div className="flex items-center gap-2 px-4 py-1 mb-0.5">
              <div
                className="w-1.5 h-1.5 rounded-full shrink-0"
                style={{ background: CATEGORY_COLORS[cat] ?? '#a1a1aa' }}
              />
              <span className="text-[10px] font-semibold text-zinc-400 tracking-wider uppercase">
                {cat}
              </span>
            </div>

            {/* Items */}
            {Object.entries(NODE_TYPES)
              .filter(([, def]) => def.category === cat)
              .map(([typeName, def]) => (
                <div
                  key={typeName}
                  className="mx-2 px-3 py-1.5 rounded-md text-[12px] text-zinc-600
                             cursor-grab select-none
                             hover:bg-zinc-50 hover:text-zinc-900
                             transition-colors duration-100
                             active:cursor-grabbing active:opacity-60"
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
