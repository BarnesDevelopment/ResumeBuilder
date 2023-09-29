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
