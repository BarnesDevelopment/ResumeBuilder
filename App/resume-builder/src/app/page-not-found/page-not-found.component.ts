import {Component, OnInit} from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-page-not-found',
    templateUrl: './page-not-found.component.html',
    styleUrls: ['./page-not-found.component.scss'],
    standalone: true,
})
export class PageNotFoundComponent implements OnInit{
  jokes: Joke[] = [
    {
      title: 'Knock knock',
      body:
        "Who's there?\n" +
        'Error.\n' +
        'Error Who?\n' +
        'Error 404: Punchline not found',
    },
    {
      title: 'Took an IQ test today.',
      body: "Got 404, guess I'm a genius",
    },
    {
      title: 'Error 404',
      body: 'Your Haiku could not be found\n' + '\n' + 'Try again later',
    },
    {
      title: '99 bugs in my code,\n99 bugs in my code...',
      body: "Take one down, fix em' around,\n404 bugs in my code.",
    },
    {
      title: 'What did one broken computer say to the other?',
      body: 'Error 404: response not found.',
    },
    {
      title: 'My Social Life',
      body: '404',
    },
    {
      title: 'Net neutrality is overrated.\nI can still access all the sites.',
      body: 'Besides, most of them are the same anyway,\nalways showing only 404 and the like.',
    },
  ];

  joke: Joke;

  constructor(private titleService: Title) {}

  getRandom(max: number) {
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - 1 + 1) + 1);
  }

  ngOnInit() {
    this.titleService.setTitle('404');
    const jokeIndex = this.getRandom(this.jokes.length);
    this.joke = this.jokes[jokeIndex - 1];
  }
}

export interface Joke {
  title: string;
  body: string;
}
