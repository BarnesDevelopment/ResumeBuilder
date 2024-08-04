import { Component, Input, OnInit } from '@angular/core';
import { ResumeTreeNode } from '../../../../../models/Resume';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UpsertSignal } from '../upsert-signal/upsert-signal';

@Component({
  selector: 'app-section-paragraph',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './section-paragraph.component.html',
  styleUrl: './section-paragraph.component.scss',
})
export class SectionParagraphComponent extends UpsertSignal implements OnInit {
  @Input() node: ResumeTreeNode;

  form: FormGroup;

  ngOnInit() {
    this.form = new FormGroup({
      paragraph: new FormControl(this.node.content),
    });

    this.form.valueChanges.subscribe(value => {
      this.node.content = value.paragraph;
    });
  }
}
