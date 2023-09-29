import { Guid } from 'guid-typescript';

export interface ResumeHeader {
  title: string;
  comments: string;
  id: Guid;
}
