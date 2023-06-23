import { Injectable } from '@angular/core';
import * as PDFDocument from 'pdfkit';
import * as blobStream from 'blob-stream';

@Injectable({
  providedIn: 'root'
})
export class BuildPdfService {

  constructor() { }

  public async BuildPdf() : Promise<string> {
    let url = '';

    const doc = new PDFDocument()
    const stream = doc.pipe(blobStream());
    doc.text("samBob");

    doc.end();

    stream.on('finish', function() {

      // or get a blob URL for display in the browser
      url = stream.toBlobURL('application/pdf');
    });

    return url;
  }
}
