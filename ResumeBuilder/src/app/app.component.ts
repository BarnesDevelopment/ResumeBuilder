import {Component, OnInit} from '@angular/core';
import {BuildPdfService} from "./resume-builder/build-pdf.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [BuildPdfService]
})
export class AppComponent implements OnInit{
  title = 'ResumeBuilder';

  constructor(private service: BuildPdfService) {}

  ngOnInit() {

  }

  async build(){
    let pdf = await this.service.BuildPdf();
    console.log(pdf);
  }
}
