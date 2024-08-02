import {
  fireEvent,
  Guid,
  NodeType,
  renderRootComponent,
  ResumeTreeNode,
  screen,
} from '../../../../../common/testing-imports';
import { PersonalInfoComponent } from './personal-info.component';
import { newResumeTreeNode } from '../../../../../models/Resume';

describe('PersonalInfoComponent', () => {
  let node: ResumeTreeNode;

  beforeEach(() => {
    node = {
      id: Guid.create(),
      nodeType: NodeType.Title,
      userId: Guid.create(),
      parentId: Guid.create(),
      content: 'My Professional Dev Name',
      active: true,
      order: 0,
      depth: 1,
      children: [
        {
          id: Guid.create(),
          nodeType: NodeType.ListItem,
          userId: Guid.create(),
          parentId: Guid.create(),
          content: 'my.professional.dev@email.dev',
          active: true,
          order: 0,
          depth: 2,
          children: [],
          comments: '',
        },
        {
          id: Guid.create(),
          nodeType: NodeType.ListItem,
          userId: Guid.create(),
          parentId: Guid.create(),
          content: '123-456-7890',
          active: true,
          order: 1,
          depth: 2,
          children: [],
          comments: '',
        },
      ],
      comments: '',
    };
  });
  describe('Load Data', () => {
    describe('Personal Info', () => {
      it('should load name, email, and phone by default', async () => {
        const component = await render(node);

        expect((screen.getByTitle('name') as HTMLInputElement).value).toBe(
          'My Professional Dev Name',
        );
        expect((screen.getByTitle('email') as HTMLInputElement).value).toBe(
          'my.professional.dev@email.dev',
        );
        expect((screen.getByTitle('phone') as HTMLInputElement).value).toBe(
          '123-456-7890',
        );
      });
    });
    describe('Website', () => {
      it('should show add website button by default', async () => {
        await render(node);

        expect(screen.queryByText('Add Website')).toBeTruthy();
        expect(screen.queryByTitle('website')).toBeFalsy();
        expect(screen.queryByTestId('minusButton')).toBeFalsy();
      });
      it("should show website if it's already added", async () => {
        node.children.push(
          newResumeTreeNode(
            NodeType.ListItem,
            2,
            node,
            'https://mywebsite.dev',
          ),
        );
        await render(node);

        expect(screen.queryByText('Add Website')).toBeFalsy();
        expect((screen.queryByTitle('website') as HTMLInputElement).value).toBe(
          'https://mywebsite.dev',
        );
        expect(screen.findByTestId('minusButton')).toBeTruthy();
      });
    });
  });
  describe('Edit', () => {
    describe('Personal Info', () => {
      it('should update name on change', async () => {
        const component = await render(node);
        const nameInput = screen.getByTitle('name') as HTMLInputElement;

        fireEvent.input(nameInput, { target: { value: 'New Name' } });

        expect(component.fixture.componentInstance.node.content).toBe(
          'New Name',
        );
      });
      it('should update email on change', async () => {
        const component = await render(node);
        const emailInput = screen.getByTitle('email') as HTMLInputElement;

        fireEvent.input(emailInput, { target: { value: 'new@email.com' } });

        expect(
          component.fixture.componentInstance.node.children[0].content,
        ).toBe('new@email.com');
      });
      it('should update phone on change', async () => {
        const component = await render(node);
        const phoneInput = screen.getByTitle('phone') as HTMLInputElement;

        fireEvent.input(phoneInput, { target: { value: '098-765-4321' } });

        expect(
          component.fixture.componentInstance.node.children[1].content,
        ).toBe('098-765-4321');
      });
    });
    describe('Website', () => {
      it('should add website to node children', async () => {
        const component = await render(node);
        const addWebsiteButton = screen.queryByText('Add Website');

        fireEvent.click(addWebsiteButton);

        component.detectChanges();

        expect(
          component.fixture.componentInstance.node.children[2].content,
        ).toBe('');
      });
      it('should remove website from node children', async () => {
        node.children.push(
          newResumeTreeNode(
            NodeType.ListItem,
            2,
            node,
            'https://mywebsite.dev',
          ),
        );
        const component = await render(node);
        const removeWebsiteButton = screen.getByTitle('minusButton');

        fireEvent.click(removeWebsiteButton);

        expect(component.fixture.componentInstance.node.children.length).toBe(
          2,
        );
      });
      it('should update website on change', async () => {
        node.children.push(
          newResumeTreeNode(
            NodeType.ListItem,
            2,
            node,
            'https://mywebsite.dev',
          ),
        );
        const component = await render(node);
        const websiteInput = screen.getByTitle('website') as HTMLInputElement;

        fireEvent.input(websiteInput, {
          target: { value: 'https://newwebsite.dev' },
        });

        expect(
          component.fixture.componentInstance.node.children[2].content,
        ).toBe('https://newwebsite.dev');
      });
    });
  });
  describe('Save', () => {});
});

const render = async (node: ResumeTreeNode) => {
  return await renderRootComponent(PersonalInfoComponent, {
    componentProperties: { node },
  });
};
