import { screen } from '@testing-library/angular';
import { Joke, PageNotFoundComponent } from './page-not-found.component';
import { renderRootComponent } from '../common/RenderRootComponent';

describe('PageNotFoundComponent', () => {
  const jokes: Joke[] = [{ title: 'joke title', body: 'joke body' }];

  beforeEach(() => {});

  it('should select a joke from the list', async () => {
    const { fixture } = await Render(jokes);
    expect(fixture.componentInstance.jokes).toContain(jokes[0]);
  });

  it('should display joke', async () => {
    await Render(jokes);
    expect(screen.getByText(jokes[0].title)).toBeTruthy();
    expect(screen.getByText(jokes[0].body)).toBeTruthy();
  });
});

async function Render(jokes: Joke[]) {
  return await renderRootComponent(PageNotFoundComponent, {
    componentProperties: {
      jokes: jokes,
    },
  });
}
