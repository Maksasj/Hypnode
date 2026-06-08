import { useCallback, useRef } from 'react';
import {
  ReactFlow,
  Background,
  Controls,
  MiniMap,
  useNodesState,
  useEdgesState,
  addEdge,
  type OnConnect,
  type Node,
  type Edge,
  type ReactFlowInstance,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';

import { NODE_TYPES as HYPNODE_NODE_TYPES } from './data/nodeTypes';
import type { HypnodeNodeData } from './types/hypnode';
import { HypnodeNode } from './components/HypnodeNode';
import { NodePalette } from './components/NodePalette';
import { Toolbar } from './components/Toolbar';
import './App.css';

const RF_NODE_TYPES = { hypnodeNode: HypnodeNode };

let nodeIdCounter = 1;
function nextId() { return `n${nodeIdCounter++}`; }

export default function App() {
  const [nodes, setNodes, onNodesChange] = useNodesState<Node>([]);
  const [edges, setEdges, onEdgesChange] = useEdgesState<Edge>([]);
  const rfInstance = useRef<ReactFlowInstance | null>(null);
  const canvasRef = useRef<HTMLDivElement>(null);

  const onConnect: OnConnect = useCallback(
    params => setEdges(es => addEdge({ ...params, animated: false }, es)),
    [setEdges],
  );

  function onDragOver(e: React.DragEvent) {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
  }

  function onDrop(e: React.DragEvent) {
    e.preventDefault();
    const typeName = e.dataTransfer.getData('application/hypnode-type');
    if (!typeName || !rfInstance.current || !canvasRef.current) return;

    const bounds = canvasRef.current.getBoundingClientRect();
    const position = rfInstance.current.screenToFlowPosition({
      x: e.clientX - bounds.left,
      y: e.clientY - bounds.top,
    });

    const typeDef = HYPNODE_NODE_TYPES[typeName];
    if (!typeDef) return;

    const newNode: Node = {
      id: nextId(),
      type: 'hypnodeNode',
      position,
      data: {
        typeName,
        label: typeDef.label,
        ports: typeDef.ports,
        params: Object.fromEntries((typeDef.params ?? []).map(p => [p.name, p.default])),
      } satisfies HypnodeNodeData,
    };

    setNodes(ns => [...ns, newNode]);
  }

  function handleImport(importedNodes: Node[], importedEdges: Edge[]) {
    nodeIdCounter = importedNodes.length + 1;
    setNodes(importedNodes);
    setEdges(importedEdges);
  }

  return (
    <div className="app">
      <Toolbar
        nodes={nodes}
        edges={edges}
        onImport={handleImport}
        onClear={() => { setNodes([]); setEdges([]); }}
      />
      <div className="app__workspace">
        <NodePalette />
        <div className="app__canvas" ref={canvasRef} onDrop={onDrop} onDragOver={onDragOver}>
          <ReactFlow
            nodes={nodes}
            edges={edges}
            onNodesChange={onNodesChange}
            onEdgesChange={onEdgesChange}
            onConnect={onConnect}
            onInit={inst => { rfInstance.current = inst; }}
            nodeTypes={RF_NODE_TYPES}
            deleteKeyCode="Delete"
            fitView
            colorMode="dark"
          >
            <Background color="#313149" gap={20} size={1} />
            <Controls />
            <MiniMap nodeColor="#1e1e2e" maskColor="rgba(17,17,27,0.7)" />
          </ReactFlow>
        </div>
      </div>
    </div>
  );
}
