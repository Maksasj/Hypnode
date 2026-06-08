export type PortType = 'int' | 'bool' | 'byte' | 'string' | 'float' | 'double' | 'LogicValue';

export interface PortDef {
  id: string;
  label: string;
  direction: 'input' | 'output';
  dataType: PortType;
}

export interface ParamDef {
  name: string;
  default: string;
}

export interface NodeTypeDef {
  label: string;
  category: string;
  ports: PortDef[];
  params?: ParamDef[];
}

export interface HypnodeNodeData extends Record<string, unknown> {
  typeName: string;
  label: string;
  ports: PortDef[];
  params: Record<string, string>;
}
