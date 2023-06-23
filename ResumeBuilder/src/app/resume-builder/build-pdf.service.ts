import { Injectable } from '@angular/core';
import * as PDFDocument from 'pdfkit';

@Injectable({
  providedIn: 'root'
})
export class BuildPdfService {

  constructor() { }

  public async BuildPdf() : Promise<string> {
    let url = '';

    const doc = new PDFDocument()
    doc.text("samBob");

    doc.end();

    return url;
  }
}
