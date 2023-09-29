import { ComponentFixture } from '@angular/core/testing';
import { screen } from '@testing-library/angular';
import { HomeComponent } from './home.component';
import { renderRootComponent } from '../common/RenderRootComponent';
import { ResumeHeader } from '../models/Resume';
import { Guid } from 'guid-typescript';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;

  it('should show create resume card', async () => {
    await render();
    expect(screen.getByText('Create Resume')).toBeTruthy();
  });

  it('should show resume cards', async () => {
    await render([
      {
        title: 'Resume 1',
        comments: 'This is my first resume',
        id: Guid.create(),
      },
      {
        title: 'Resume 2',
        comments: 'This is my second resume',
        id: Guid.create(),
      },
      {
        title: 'Resume 3',
        comments: 'This is my third resume',
        id: Guid.create(),
      },
    ]);
    expect(screen.getByText('Resume 1')).toBeTruthy();
    expect(screen.getByText('This is my first resume')).toBeTruthy();
    expect(screen.getByText('Resume 2')).toBeTruthy();
    expect(screen.getByText('This is my second resume')).toBeTruthy();
    expect(screen.getByText('Resume 3')).toBeTruthy();
    expect(screen.getByText('This is my third resume')).toBeTruthy();
  });

  it('should show dumb cards if login is false', async () => {
    await render(
      [
        {
          title: 'Resume 1',
          comments: 'This is my first resume',
          id: Guid.create(),
        },
      ],
      false,
    );
    expect(screen.getByTitle('dumb cards')).toBeTruthy();
  });

  it('should show login splash screen if login is false', async () => {
    await render([], false);
    expect(screen.getByTitle('login splash')).toBeTruthy();
  });
});

const render = async (
  resumes: ResumeHeader[] = [],
  loggedIn: boolean = true,
) => {
  return renderRootComponent(HomeComponent, {
    componentProperties: {
      resumes,
      environment: {
        production: false,
        loggedIn,
        domain: '',
        apiBasePath: '',
      },
    },
  });
};
