import { Guid } from 'guid-typescript';

export interface ResumeHeader {
  content: string;
  comments: string;
  id: Guid;
}
