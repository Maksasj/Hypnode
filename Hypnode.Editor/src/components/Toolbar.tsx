import { Download, Upload, Trash2, Workflow } from 'lucide-react';
import type { Node, Edge } from '@xyflow/react';
import { exportXml, importXml, downloadXml } from '../utils/xml';

interface Props {
  nodes: Node[];
  edges: Edge[];
  onImport: (nodes: Node[], edges: Edge[]) => void;
  onClear: () => void;
}

export function Toolbar({ nodes, edges, onImport, onClear }: Props) {
  function handleExport() {
    downloadXml(exportXml(nodes, edges));
  }

  function handleImport() {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = '.xml';
    input.onchange = async () => {
      const file = input.files?.[0];
      if (!file) return;
      try {
        const { nodes: n, edges: e } = importXml(await file.text());
        onImport(n, e);
      } catch (err) {
        alert('Import failed: ' + (err instanceof Error ? err.message : String(err)));
      }
    };
    input.click();
  }

  return (
    <header className="h-12 px-4 flex items-center justify-between bg-white border-b border-zinc-200 shrink-0">
      {/* Brand */}
      <div className="flex items-center gap-2.5">
        <div className="w-6 h-6 rounded-md bg-zinc-900 flex items-center justify-center">
          <Workflow size={13} className="text-white" strokeWidth={2.5} />
        </div>
        <span className="text-sm font-semibold text-zinc-900 tracking-tight">Hypnode</span>
        <span className="text-sm text-zinc-400 font-normal">Editor</span>
      </div>

      {/* Actions */}
      <div className="flex items-center gap-1">
        <button
          onClick={handleImport}
          className="inline-flex items-center gap-1.5 h-8 px-3 rounded-md text-[12px] font-medium
                     text-zinc-600 hover:text-zinc-900 hover:bg-zinc-100
                     transition-colors duration-100 focus-visible:outline-none
                     focus-visible:ring-2 focus-visible:ring-zinc-900"
        >
          <Upload size={13} strokeWidth={2} />
          Import
        </button>

        <button
          onClick={handleExport}
          className="inline-flex items-center gap-1.5 h-8 px-3 rounded-md text-[12px] font-medium
                     bg-zinc-900 text-white hover:bg-zinc-700
                     transition-colors duration-100 focus-visible:outline-none
                     focus-visible:ring-2 focus-visible:ring-zinc-900"
        >
          <Download size={13} strokeWidth={2} />
          Export XML
        </button>

        <div className="w-px h-5 bg-zinc-200 mx-1" />

        <button
          onClick={onClear}
          className="inline-flex items-center justify-center w-8 h-8 rounded-md
                     text-zinc-400 hover:text-red-500 hover:bg-red-50
                     transition-colors duration-100 focus-visible:outline-none
                     focus-visible:ring-2 focus-visible:ring-zinc-900"
          title="Clear canvas"
        >
          <Trash2 size={14} strokeWidth={2} />
        </button>
      </div>
    </header>
  );
}
