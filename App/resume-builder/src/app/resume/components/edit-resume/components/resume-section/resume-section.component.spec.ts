import {
  newResumeTreeNode,
  SectionDisplayType,
} from '../../../../../models/Resume';
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
  fireEvent,
  SectionEducationComponentStub,
  SectionWorkExperienceComponentStub,
} from '../../../../../common/testing-imports';
import { By } from '@angular/platform-browser';
import { SectionEducationComponent } from '../section-education/section-education.component';
import { SectionWorkExperienceComponent } from '../section-work-experience/section-work-experience.component';

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
    describe.each([
      SectionDisplayType.Education,
      SectionDisplayType.WorkExperience,
    ])('Multiple Entry %s', (option: string) => {
      async function selectOption(optionToSelect: string) {
        const component = await render(section);
        const select = component.debugElement.query(By.css('select'));
        select.nativeElement.value = optionToSelect;
        select.nativeElement.dispatchEvent(new Event('change'));
        component.fixture.detectChanges();
        return component;
      }

      it('should show plus button', async () => {
        await selectOption(option);

        expect(screen.queryByTitle('sectionPlusButton')).toBeInTheDocument();
      });
      it('should not show minus button by default', async () => {
        await selectOption(option);

        expect(
          screen.queryByTitle('sectionMinusButton'),
        ).not.toBeInTheDocument();
      });
      it('should add a new child when plus button is clicked', async () => {
        const component = await selectOption(option);

        const plusButton = screen.queryByTitle('sectionPlusButton');
        fireEvent.click(plusButton);

        expect(
          component.fixture.componentInstance.section.children.length,
        ).toBe(2);
      });
      it('should queue save when a new child is added', async () => {
        const component = await selectOption(option);
        const saveSpy = jest.spyOn(
          component.fixture.componentInstance,
          'queueSave',
        );
        const plusButton = screen.queryByTitle('sectionPlusButton');
        fireEvent.click(plusButton);

        expect(saveSpy).toHaveBeenCalledTimes(1);
      });
      it('should show minus button after adding', async () => {
        await selectOption(option);

        const plusButton = screen.queryByTitle('sectionPlusButton');
        fireEvent.click(plusButton);

        expect(screen.queryAllByTitle('sectionMinusButton').length).toBe(2);
      });
      it('should remove a section when minus button is clicked', async () => {
        const component = await selectOption(option);

        const plusButton = screen.queryByTitle('sectionPlusButton');
        fireEvent.click(plusButton);

        const minusButton = screen.queryAllByTitle('sectionMinusButton')[1];
        fireEvent.click(minusButton);

        expect(
          component.fixture.componentInstance.section.children.length,
        ).toBe(1);
      });
      it('should queue delete when minus button is clicked', async () => {
        const component = await selectOption(option);
        const deleteSpy = jest.spyOn(
          component.fixture.componentInstance,
          'queueDeleteRecursive',
        );

        const plusButton = screen.queryByTitle('sectionPlusButton');
        fireEvent.click(plusButton);

        const minusButton = screen.queryAllByTitle('sectionMinusButton')[1];
        fireEvent.click(minusButton);

        expect(deleteSpy).toHaveBeenCalledTimes(1);
      });
      it('should reorder children when removing', async () => {
        const component = await selectOption(option);

        fireEvent.click(screen.queryByTitle('sectionPlusButton'));
        fireEvent.click(screen.queryByTitle('sectionPlusButton'));

        const minusButton = screen.queryAllByTitle('sectionMinusButton')[0];
        fireEvent.click(minusButton);

        component.fixture.componentInstance.section.children.forEach(
          (child, index) => {
            expect(child.order).toBe(index);
          },
        );
      });
    });
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
  describe('Save', () => {
    let saveSpy, deleteSpy;
    beforeEach(async () => {
      const { fixture } = await render(section);
      saveSpy = jest.spyOn(fixture.componentInstance.onSave, 'emit');
      deleteSpy = jest.spyOn(fixture.componentInstance.onDelete, 'emit');
    });
    it('should emit a save when section title is changed', async () => {
      const newSection = { ...section, content: 'TestTitle' };

      const input = screen.queryByTitle('name');
      fireEvent.input(input, { target: { value: 'TestTitle' } });

      expect(saveSpy).toHaveBeenCalledTimes(1);
      expect(saveSpy).toHaveBeenCalledWith(newSection);
    });
    it('should emit a save when section type is changed', async () => {
      const select = screen.queryByTitle('type');
      fireEvent.change(select, { target: { value: SectionDisplayType.List } });

      expect(saveSpy).toHaveBeenCalledTimes(1);
      const functionArg = saveSpy.mock.calls[0][0];
      expect(functionArg.content).toEqual('');
      expect(functionArg.nodeType).toEqual(NodeType.List);
      expect(functionArg.parentId).toEqual(section.id);
    });
    it('should strip children from node when saving', async () => {
      const newSection = { ...section, children: [], content: 'TestTitle' };

      const select = screen.queryByTitle('type');
      fireEvent.change(select, { target: { value: SectionDisplayType.List } });
      const input = screen.queryByTitle('name');
      fireEvent.input(input, { target: { value: 'TestTitle' } });

      expect(saveSpy).toHaveBeenCalledTimes(2);
      const functionArg = saveSpy.mock.calls[0][0];
      expect(functionArg.content).toEqual('');
      expect(functionArg.nodeType).toEqual(NodeType.List);
      expect(functionArg.parentId).toEqual(section.id);
      expect(functionArg.children).toEqual([]);
      expect(saveSpy).toHaveBeenCalledWith(newSection);
    });
    it('should not emit a delete on initial section change', async () => {
      const select = screen.queryByTitle('type');
      fireEvent.change(select, { target: { value: SectionDisplayType.List } });

      expect(deleteSpy).not.toHaveBeenCalled();
    });
    it('should emit a delete when section type is changed', async () => {
      const select = screen.queryByTitle('type');
      fireEvent.change(select, { target: { value: SectionDisplayType.List } });
      fireEvent.change(select, {
        target: { value: SectionDisplayType.Paragraph },
      });

      expect(deleteSpy).toHaveBeenCalledTimes(1);
    });
  });
});

const render = async (section: ResumeTreeNode) => {
  return await renderRootComponent(ResumeSectionComponent, {
    componentProperties: {
      section,
    },
    imports: [],
    importOverrides: [
      { remove: SectionListComponent, add: SectionListComponentStub },
      { remove: SectionEducationComponent, add: SectionEducationComponentStub },
      {
        remove: SectionWorkExperienceComponent,
        add: SectionWorkExperienceComponentStub,
      },
    ],
  });
};
