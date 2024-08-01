import { SectionDisplayType } from '../../../../../models/Resume';
import { ResumeSectionComponent } from './resume-section.component';
import '@testing-library/jest-dom';
import { SectionListComponent } from '../section-list/section-list.component';
import {
  screen,
  Guid,
  renderRootComponent,
  NodeType,
  ResumeTreeNode,
  SectionListComponentStub,
} from '../../../../../common/testing-imports';
import { By } from '@angular/platform-browser';

describe('ResumeSectionComponent', () => {
  let section: ResumeTreeNode;

  beforeEach(() => {
    section = {
      active: true,
      depth: 0,
      order: 0,
      parentId: Guid.create(),
      userId: Guid.create(),
      id: Guid.create(),
      content: '',
      nodeType: NodeType.Section,
      children: [],
      comments: '',
    };
  });

  describe('Init', () => {
    it('should show a single empty input', async () => {
      await render(section);

      expect(screen.queryByTitle('name')).toBeInTheDocument();
      expect(screen.queryByTitle('name')).toHaveValue('');
    });
    it.each([
      '',
      SectionDisplayType.List,
      SectionDisplayType.Education,
      SectionDisplayType.Paragraph,
      SectionDisplayType.WorkExperience,
    ])(
      'should show a select list with section option %s',
      async (option: string) => {
        await render(section);

        const options = screen
          .getAllByRole('option')
          .map(x => x.getAttribute('value'));
        expect(options).toContain(option);
      },
    );
  });
  describe('Selection', () => {
    it.each([
      SectionDisplayType.List,
      SectionDisplayType.Education,
      SectionDisplayType.Paragraph,
      SectionDisplayType.WorkExperience,
    ])(
      'should load correct body when list option %s selected',
      async (option: string) => {
        let selector: string;
        switch (option) {
          case SectionDisplayType.List:
            selector = 'app-section-list';
            break;
          case SectionDisplayType.Education:
            selector = 'app-section-education';
            break;
          case SectionDisplayType.Paragraph:
            selector = 'app-section-paragraph';
            break;
          case SectionDisplayType.WorkExperience:
            selector = 'app-section-work-experience';
            break;
        }

        const component = await render(section);

        expect(component.debugElement.query(By.css(selector))).toBeFalsy();

        const select = component.debugElement.query(By.css('select'));
        select.nativeElement.value = option;
        select.nativeElement.dispatchEvent(new Event('change'));
        component.fixture.detectChanges();

        expect(component.debugElement.query(By.css(selector))).toBeTruthy();
      },
    );
    it.each([
      SectionDisplayType.List,
      SectionDisplayType.Education,
      SectionDisplayType.Paragraph,
      SectionDisplayType.WorkExperience,
    ])(
      'should should create correct child when %s is selected',
      async (option: string) => {
        const component = await render(section);

        const select = component.debugElement.query(By.css('select'));
        select.nativeElement.value = option;
        select.nativeElement.dispatchEvent(new Event('change'));
        component.fixture.detectChanges();

        expect(
          component.fixture.componentInstance.section.children.length,
        ).toBe(1);
        expect(
          component.fixture.componentInstance.section.children[0].content,
        ).toBe('');
        expect(
          component.fixture.componentInstance.section.children[0].nodeType,
        ).toBe(NodeType[option as keyof typeof SectionDisplayType]);
      },
    );
  });
  describe('Load Data', () => {
    it('should load titles', async () => {
      section.content = 'TestTitle';
      await render(section);

      expect(screen.queryByTitle('name')).toHaveValue('TestTitle');
    });
    it.each([
      SectionDisplayType.List,
      SectionDisplayType.Education,
      SectionDisplayType.Paragraph,
      SectionDisplayType.WorkExperience,
    ])('should load correct section bodies for %s', async (option: string) => {
      const nodeType = NodeType[option as keyof typeof SectionDisplayType];
      let selector: string;
      switch (option) {
        case SectionDisplayType.List:
          selector = 'app-section-list';
          break;
        case SectionDisplayType.Education:
          selector = 'app-section-education';
          break;
        case SectionDisplayType.Paragraph:
          selector = 'app-section-paragraph';
          break;
        case SectionDisplayType.WorkExperience:
          selector = 'app-section-work-experience';
          break;
      }

      section.children = [
        {
          active: true,
          depth: 1,
          order: 0,
          parentId: section.id,
          userId: Guid.create(),
          id: Guid.create(),
          content: 'TestBody',
          nodeType: nodeType,
          children: [],
          comments: '',
        },
      ];

      const component = await render(section);

      expect(component.debugElement.query(By.css(selector))).toBeTruthy();
    });
  });
  describe('Save', () => {});
});

const render = async (section: ResumeTreeNode) => {
  return await renderRootComponent(ResumeSectionComponent, {
    componentProperties: {
      section,
    },
    imports: [],
    importOverrides: [
      { remove: SectionListComponent, add: SectionListComponentStub },
    ],
  });
};
