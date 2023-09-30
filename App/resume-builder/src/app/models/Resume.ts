import { Guid } from 'guid-typescript';

export interface ResumeHeader {
  content: string;
  comments: string;
  id: Guid;
}

export interface ResumeTreeNode {
  children: ResumeTreeNode[];
  comments: string;
  id: Guid;
  active: boolean;
  userId: Guid;
  parentId: Guid | null;
  content: string;
  sectionType: SectionType;
  depth: number;
  order: number;
}


export interface ResumeTreeNodeJson extends Omit<ResumeTreeNode, 'id'|'userId'|'parentId'|'children'> {
  id: string;
  userId: string;
  parentId: string;
  children: ResumeTreeNodeJson[];
}

export function newResumeTreeNodeJson(node: ResumeTreeNode): ResumeTreeNodeJson {
  return {
    children: node.children.map(newResumeTreeNodeJson),
    comments: node.comments,
    id: node.id.toString(),
    active: node.active,
    userId: node.userId.toString(),
    parentId: node.parentId?.toString() ?? null,
    content: node.content,
    sectionType: node.sectionType,
    depth: node.depth,
    order: node.order
  };
}

export enum SectionType {
  Resume = 'Resume',
  Section = 'Section',
  Separator = 'Separator',
  Paragraph = 'Paragraph',
  Title = 'Title',
  Line = 'Line',
  List = 'List',
  ListItem = 'ListItem',
  SectionItem = 'SectionItem',
}
