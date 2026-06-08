import type { Node, Edge } from '@xyflow/react';
import type { HypnodeNodeData } from '../types/hypnode';
import { NODE_TYPES, PORT_COLORS } from '../data/nodeTypes';

export function exportXml(nodes: Node[], edges: Edge[]): string {
  const lines: string[] = ['<?xml version="1.0" encoding="utf-8"?>', '<Graph>'];

  lines.push('  <Nodes>');
  for (const node of nodes) {
    const data = node.data as HypnodeNodeData;
    const params = Object.entries(data.params ?? {}).filter(([, v]) => v !== '');
    if (params.length > 0) {
      lines.push(`    <Node id="${node.id}" type="${data.typeName}">`);
      lines.push('      <Parameters>');
      for (const [k, v] of params)
        lines.push(`        <Parameter name="${k}">${escapeXml(v)}</Parameter>`);
      lines.push('      </Parameters>');
      lines.push('    </Node>');
    } else {
      lines.push(`    <Node id="${node.id}" type="${data.typeName}" />`);
    }
  }
  lines.push('  </Nodes>');

  lines.push('  <Connections>');
  for (const edge of edges) {
    const sourceNode = nodes.find(n => n.id === edge.source);
    const connType = resolveConnectionType(sourceNode, edge.sourceHandle ?? '');
    lines.push(`  <Connection type="${connType}">`);
    lines.push(`    <From node="${edge.source}" port="${edge.sourceHandle}" />`);
    lines.push(`    <To   node="${edge.target}" port="${edge.targetHandle}" />`);
    lines.push(`  </Connection>`);
  }
  lines.push('  </Connections>');

  lines.push('</Graph>');
  return lines.join('\n');
}

export function importXml(xml: string): { nodes: Node[]; edges: Edge[] } {
  const parser = new DOMParser();
  const doc = parser.parseFromString(xml, 'application/xml');

  const error = doc.querySelector('parsererror');
  if (error) throw new Error('Invalid XML: ' + error.textContent);

  const nodes: Node[] = [];
  const edges: Edge[] = [];
  let nodeIndex = 0;

  for (const el of Array.from(doc.querySelectorAll('Graph > Nodes > Node'))) {
    const id = el.getAttribute('id') ?? `n${nodeIndex}`;
    const typeName = el.getAttribute('type') ?? '';
    const typeDef = NODE_TYPES[typeName];

    const params: Record<string, string> = {};
    for (const p of Array.from(el.querySelectorAll('Parameters > Parameter'))) {
      const name = p.getAttribute('name');
      if (name) params[name] = p.textContent ?? '';
    }

    nodes.push({
      id,
      type: 'hypnodeNode',
      position: { x: 120 + (nodeIndex % 4) * 220, y: 80 + Math.floor(nodeIndex / 4) * 180 },
      data: {
        typeName,
        label: typeDef?.label ?? typeName,
        ports: typeDef?.ports ?? [],
        params: { ...Object.fromEntries((typeDef?.params ?? []).map(p => [p.name, p.default])), ...params },
      } satisfies HypnodeNodeData,
    });
    nodeIndex++;
  }

  let edgeIndex = 0;
  for (const el of Array.from(doc.querySelectorAll('Graph > Connections > Connection'))) {
    const from = el.querySelector('From');
    const to   = el.querySelector('To');
    if (!from || !to) continue;
    edges.push({
      id: `e${edgeIndex++}`,
      source: from.getAttribute('node') ?? '',
      sourceHandle: from.getAttribute('port') ?? '',
      target: to.getAttribute('node') ?? '',
      targetHandle: to.getAttribute('port') ?? '',
    });
  }

  return { nodes, edges };
}

function resolveConnectionType(node: Node | undefined, portId: string): string {
  if (!node) return 'int';
  const data = node.data as HypnodeNodeData;
  const port = data.ports?.find(p => p.id === portId);
  return port?.dataType ?? 'int';
}

function escapeXml(s: string): string {
  return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

export function downloadXml(xml: string, filename = 'graph.xml') {
  const blob = new Blob([xml], { type: 'application/xml' });
  const url  = URL.createObjectURL(blob);
  const a    = document.createElement('a');
  a.href = url; a.download = filename; a.click();
  URL.revokeObjectURL(url);
}
