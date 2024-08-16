import {
  renderRootComponent,
  fireEvent,
  screen,
  NodeType,
} from '../../../common/testing-imports';
import { CreateResumeComponent } from './create-resume.component';
import { ResumeService } from '../../services/resume.service';
import { Guid } from 'guid-typescript';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('CreateResumeComponent', () => {
  let serviceSpy;
  beforeEach(() => {
    serviceSpy = jest.spyOn(ResumeService.prototype, 'updateResume');
    jest.spyOn(Guid, 'create').mockReturnValue(Guid.createEmpty());
  });
  it('should send upsert request on submit button click', async () => {
    await render();
    const titleInput = screen.getByTitle('title');
    const commentsInput = screen.getByTitle('comments');
    const nameInput = screen.getByTitle('name');
    const emailInput = screen.getByTitle('email');
    const phoneInput = screen.getByTitle('phone');
    const submitButton = screen.getByText('Submit');

    fireEvent.input(titleInput, { target: { value: 'Test Title' } });
    fireEvent.input(commentsInput, { target: { value: 'Test comments' } });
    fireEvent.input(nameInput, { target: { value: 'Test Name' } });
    fireEvent.input(emailInput, { target: { value: 'test@resume.com' } });
    fireEvent.input(phoneInput, { target: { value: '123-456-7890' } });
    fireEvent.click(submitButton);

    expect(serviceSpy).toBeCalledWith({
      content: 'Test Title',
      comments: 'Test comments',
      depth: 0,
      id: Guid.createEmpty(),
      order: 0,
      nodeType: NodeType.Resume,
      parentId: null,
      userId: Guid.createEmpty(),
      active: true,
      children: [
        {
          content: 'Test Name',
          comments: '',
          depth: 1,
          id: Guid.createEmpty(),
          order: 0,
          nodeType: NodeType.Title,
          parentId: Guid.createEmpty(),
          userId: Guid.createEmpty(),
          active: true,
          children: [
            {
              content: 'test@resume.com',
              comments: '',
              depth: 2,
              id: Guid.create(),
              order: 0,
              nodeType: NodeType.ListItem,
              parentId: Guid.createEmpty(),
              userId: Guid.createEmpty(),
              active: true,
              children: [],
            },
            {
              content: '123-456-7890',
              comments: '',
              depth: 2,
              id: Guid.create(),
              order: 1,
              nodeType: NodeType.ListItem,
              parentId: Guid.createEmpty(),
              userId: Guid.createEmpty(),
              active: true,
              children: [],
            },
          ],
        },
      ],
    });
  });
  it('should send upsert request with website if provided', async () => {
    await render();
    const titleInput = screen.getByTitle('title');
    const commentsInput = screen.getByTitle('comments');
    const nameInput = screen.getByTitle('name');
    const emailInput = screen.getByTitle('email');
    const phoneInput = screen.getByTitle('phone');
    const websiteInput = screen.getByTitle('website');
    const submitButton = screen.getByText('Submit');

    fireEvent.input(titleInput, { target: { value: 'Test Title' } });
    fireEvent.input(commentsInput, { target: { value: 'Test comments' } });
    fireEvent.input(nameInput, { target: { value: 'Test Name' } });
    fireEvent.input(emailInput, { target: { value: 'test@resume.com' } });
    fireEvent.input(phoneInput, { target: { value: '123-456-7890' } });
    fireEvent.input(websiteInput, { target: { value: 'www.resume.com' } });
    fireEvent.click(submitButton);

    expect(serviceSpy).toBeCalledWith({
      content: 'Test Title',
      comments: 'Test comments',
      depth: 0,
      id: Guid.createEmpty(),
      order: 0,
      nodeType: NodeType.Resume,
      parentId: null,
      userId: Guid.createEmpty(),
      active: true,
      children: [
        {
          content: 'Test Name',
          comments: '',
          depth: 1,
          id: Guid.createEmpty(),
          order: 0,
          nodeType: NodeType.Title,
          parentId: Guid.createEmpty(),
          userId: Guid.createEmpty(),
          active: true,
          children: [
            {
              content: 'test@resume.com',
              comments: '',
              depth: 2,
              id: Guid.create(),
              order: 0,
              nodeType: NodeType.ListItem,
              parentId: Guid.createEmpty(),
              userId: Guid.createEmpty(),
              active: true,
              children: [],
            },
            {
              content: '123-456-7890',
              comments: '',
              depth: 2,
              id: Guid.create(),
              order: 1,
              nodeType: NodeType.ListItem,
              parentId: Guid.createEmpty(),
              userId: Guid.createEmpty(),
              active: true,
              children: [],
            },
            {
              content: 'www.resume.com',
              comments: '',
              depth: 2,
              id: Guid.create(),
              order: 2,
              nodeType: NodeType.ListItem,
              parentId: Guid.createEmpty(),
              userId: Guid.createEmpty(),
              active: true,
              children: [],
            },
          ],
        },
      ],
    });
  });
});

const render = async () => {
  return await renderRootComponent(CreateResumeComponent, {
    providers: [
      {
        provide: ActivatedRoute,
        useValue: {
          queryParams: of({ next: 0 }),
        },
      },
    ],
  });
};
