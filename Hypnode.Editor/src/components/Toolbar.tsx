import type { Node, Edge } from '@xyflow/react';
import { exportXml, importXml, downloadXml } from '../utils/xml';
import './Toolbar.css';

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
        const text = await file.text();
        const { nodes: n, edges: e } = importXml(text);
        onImport(n, e);
      } catch (err) {
        alert('Failed to import: ' + (err instanceof Error ? err.message : String(err)));
      }
    };
    input.click();
  }

  return (
    <header className="toolbar">
      <span className="toolbar__brand">Hypnode Editor</span>
      <div className="toolbar__actions">
        <button className="toolbar__btn" onClick={handleImport}>Import XML</button>
        <button className="toolbar__btn toolbar__btn--primary" onClick={handleExport}>Export XML</button>
        <button className="toolbar__btn toolbar__btn--danger" onClick={onClear}>Clear</button>
      </div>
    </header>
  );
}
