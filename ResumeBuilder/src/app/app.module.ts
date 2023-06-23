import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {ResumeBuilderModule} from "./resume-builder/resume-builder.module";
import {BuildPdfService} from "./resume-builder/build-pdf.service";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ResumeBuilderModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
